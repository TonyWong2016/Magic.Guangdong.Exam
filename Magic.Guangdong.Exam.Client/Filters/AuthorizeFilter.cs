using DotNetCore.CAP;
using EasyCaching.Core;
using Essensoft.Paylink.Alipay.Domain;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Magic.Guangdong.Exam.Client.Filters
{
    public class AuthorizeFilter : IAuthorizationFilter
    {
        private readonly ICapPublisher _capPublisher;
        private readonly IRedisCachingProvider _redisCachingProvider;
        public AuthorizeFilter(ICapPublisher capPublisher, IRedisCachingProvider redisCachingProvider)
        {
            _capPublisher = capPublisher;
            _redisCachingProvider = redisCachingProvider;
        }

        /// <summary>
        /// 请求验证，当前验证部分不要抛出异常，ExceptionFilter不会处理
        /// </summary>
        /// <param name="context"></param>
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            string page = "";
            var routeValues = context.RouteData.Values;
            if (routeValues.ContainsKey("page"))
                page = routeValues["page"].ToString().ToLower();
            if (routeValues.ContainsKey("controller"))
                page = (routeValues["controller"].ToString()+"/"+ routeValues["action"].ToString()).ToLower();
            if (routeValues.ContainsKey("controller") && routeValues.ContainsKey("area"))
                page = (routeValues["area"].ToString()+"/" +routeValues["controller"].ToString()+"/" + routeValues["action"].ToString()).ToLower();

            Assistant.Logger.Info("访问：" + page);
            page = page.ToLower();
            //var descriptor = context.ActionDescriptor;
            if (page.Contains("error") || page.Contains("open"))
            {
                Assistant.Logger.Debug("访问公开接口");
                return;
            }


            var cookies = context.HttpContext.Request.Cookies;
            
            if (page.Equals("/exam/verify") ||
                page.Equals("/exammobile/verify")||
                page.Equals("/account/me") ||
                page.Equals("/account/logout") ||
                page.Equals("account/logout"))
            {
                Assistant.Logger.Debug("免登录");
                return;
            }
            if (!cookies.Where(u => u.Key == "accountId").Any() 
                && !cookies.Where(u => u.Key == "idToken").Any()
                && !page.Contains("account") )
            {
                string redict = Utils.EncodeUrlParam(context.HttpContext.Request.Path + context.HttpContext.Request.QueryString.Value);
                contextHandle(context, "登录信息已注销~", "/account/me?redict="+ redict);
                return;
            }

            if(!cookies.Where(u=>u.Key== "clientsign").Any())
            {
                contextHandle(context, "非法登录");

                return;
            }

            string sign = cookies.Where(u => u.Key == "clientsign").FirstOrDefault().Value;
            string accountId = cookies.Where(u => u.Key == "accountId").FirstOrDefault().Value;
            string idToken = cookies.Where(u => u.Key == "idToken").FirstOrDefault().Value;
            if(sign != Assistant.Security.GenerateMD5Hash(accountId+idToken+ Assistant.ConfigurationHelper.GetSectionValue("SecretPwd")))
            {
                contextHandle(context, "未授权");
                return;
            }
            
            return;
            

        }

        public void contextHandle(AuthorizationFilterContext context,string msg="非法登录",string url="/error")
        {
            // 检查是否为 AJAX 请求
            bool isAjaxRequest = context.HttpContext.Request.Headers.ContainsKey("X-Requested-With") &&
                                 context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                                 context.HttpContext.Request.Headers.ContainsKey("Accept") &&
                                 context.HttpContext.Request.Headers["Accept"].ToString().Contains("application/json");
            string finalUrl = url;
            if (url.Contains("?"))
            {
                finalUrl = $"{url}&msg={Assistant.Utils.EncodeUrlParam(msg)}";
            }
            else
            {
                finalUrl = $"{url}?msg={Assistant.Utils.EncodeUrlParam(msg)}";
            }
            if (isAjaxRequest)
            {                
                // 对于 AJAX 请求，返回一个 JSON 格式的结果
                var result = new ContentResult
                {
                    Content = "{\"message\": \"" + msg+ "\", \"redirectTo\":\"" + finalUrl + "\"}",
                    ContentType = "application/json",
                    StatusCode = 401 // 可以设置状态码为 401 表示未授权
                };
                context.Result = result;
            }
            else
            {
                // 对于非 AJAX 请求，可以正常重定向
                var item = new RedirectResult(finalUrl);
                context.Result = item;
            }
            Assistant.Logger.Error(msg);

        }

        private bool HeaderCheck(AuthorizationFilterContext context)
        {
            string requestMethod = context.HttpContext.Request.Method;
            if (requestMethod == "GET")
            {
                return true;
            }
            var headers = context.HttpContext.Request.Headers;
            if (!headers.Where(h => h.Key == "Authorization").Any())
            {
                Assistant.Logger.Error("缺少token");
                var item = new JsonResult(new { code = 401, msg = "令牌丢失，请尝试重新登录" });
                //item.StatusCode = 401;
                //var item = new RedirectResult("/error?msg=登录异常");
                context.Result = item;
                return false;
            }
            string Authorization = Assistant.Utils.FromBase64Str(headers.Where(h => h.Key == "Authorization").FirstOrDefault().Value);
            if (!Authorization.StartsWith("GD.exam|"))
            {
                Assistant.Logger.Error("token错误");
                //var item = new JsonResult(new { code = 401, msg = "token错误" });
                //item.StatusCode = 401;
                var item = new RedirectResult("/error?msg=登录异常");

                context.Result = item;
                return false;
            }

            string[] parts = Authorization.Split('|');
            long exp = Convert.ToInt64(parts[1]);
            if (exp - Assistant.Utils.DateTimeToTimeStamp(DateTime.Now) < 0)
            {
                Assistant.Logger.Error("token过期");
                //var item = new JsonResult(new { code = 401, msg = "token过期" });
                //item.StatusCode = 401;
                var item = new RedirectResult("/error?msg=登录超时");
                context.Result = item;
                return false;
            }
            return true;
        }


        private bool CookieCheck(AuthorizationFilterContext context, out Guid adminId)
        {
            adminId = Guid.Empty;
            var cookies = context.HttpContext.Request.Cookies;
            if (!cookies.Where(u => u.Key == "accountId").Any() || !cookies.Where(u => u.Key == "examToken").Any())
            {
                var item = new RedirectResult("/error?msg=notauth");
                context.Result = item;
                Assistant.Logger.Error("没登录！走你~");
                return false;
            }
            adminId = Guid.Parse(
                Assistant.Utils.FromBase64Str(
                    cookies.Where(u => u.Key == "accountId").First().Value
                    ));
            var examToken = cookies.Where(u => u.Key == "examToken").FirstOrDefault().Value;
            //if (!await Assistant.JwtService.ValidateFilter(examToken))
            //{
            //    var item = new RedirectResult("/system/account/login?msg=invalidtoken");
            //    context.Result = item;
            //    Assistant.Logger.Error("token错辣！走你~");
            //    return false;
            //}
            var claim = Assistant.JwtService.ValidateJwt(examToken);
            if (claim == null)
            {
                var item = new RedirectResult("/error?msg=invalidtoken");
                context.Result = item;
                Assistant.Logger.Error("token错辣！走你~");
                return false;
            }
            if (claim.Sid != cookies.Where(u => u.Key == "accountId").First().Value || claim.exp - Assistant.Utils.DateTimeToTimeStamp(DateTime.Now) < 0)
            {
                var item = new RedirectResult("/error?msg=invalidtoken");
                context.Result = item;
                Assistant.Logger.Error("token错辣或者超时辣！走你~");
                return false;
            }

            return true;
        }

    }
}
