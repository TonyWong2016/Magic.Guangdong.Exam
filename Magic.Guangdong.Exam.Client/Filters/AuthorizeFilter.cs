using DotNetCore.CAP;
using EasyCaching.Core;
using Essensoft.Paylink.Alipay.Domain;
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

            //var descriptor = context.ActionDescriptor;
            if (page.Contains("error") || page.Contains("open"))
            {
                Assistant.Logger.Debug("访问公开接口");
                return;
            }
            var cookies = context.HttpContext.Request.Cookies;
            if (!cookies.Where(u => u.Key == "accountId").Any() && !page.Contains("account") )
            {
                var item = new RedirectResult("/account/me");
                context.Result = item;
                Assistant.Logger.Error("登录信息已注销~");
                return;
            }
            return;
            ////cookie身份验证
            //Guid accountId;
            //if (!CookieCheck(context, out accountId))
            //{
            //    return;
            //}

            ////非get请求，请求头验证
            //if (!HeaderCheck(context))
            //{                
            //    return;
            //}

            ////string header = context.HttpContext.Request.Headers["Authorization"];
            //Assistant.Logger.Info("page:" + page);

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
