using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.DbServices.Entities;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Text;

namespace Magic.Guangdong.Exam.Filters
{
    /// <summary>
    /// 实现自定义授权
    /// </summary>
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
            string area = "";
            string controller = "";
            string action = "";
            if (context.RouteData.Values.ContainsKey("area"))
                area = context.RouteData.Values["area"].ToString().ToLower();
            if (context.RouteData.Values.ContainsKey("controller"))
                controller = context.RouteData.Values["controller"].ToString().ToLower();

            if (context.RouteData.Values.ContainsKey("action"))
                action = context.RouteData.Values["action"].ToString().ToLower();
            if (string.IsNullOrEmpty(area))
            {
                controller = Convert.ToString(context.RouteData.Values["controller"]).ToLower();
                action = Convert.ToString(context.RouteData.Values["action"]).ToLower();
            }

            var descriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;

            if (descriptor != null && descriptor.MethodInfo.CustomAttributes.Where(c => c.AttributeType.Name == "AllowAnonymousAttribute").Any())
            {
                Assistant.Logger.Debug("访问公开接口");
                return;
            }
            if (descriptor !=null 
                && descriptor.MethodInfo.CustomAttributes.Any(u => u.AttributeType.Name == "WebApiModule")
                )
            {
                Assistant.Logger.Debug("外部资源访问内部接口");
                await ApiCheck(context);

                return;
            }
            
            //cookie身份验证
            Guid adminId;
            if (!CookieCheck(context, out adminId))
            {
                //这里就不用记了，cookie都没了，也记录不上登录信息
                //await _capPublisher.PublishAsync(CapConsts.PREFIX + "AddKeyAction", new KeyAction()
                //{
                //    AdminId = adminId,
                //    Action = "cookie认证失败",
                //    Description = "cookie认证失败",
                //    Router = $"{area}/{controller}/{action}",
                //    CreatedAt = DateTime.Now
                //});

                Assistant.Logger.Error("Cookie认证失败 \r\n"+ HeadersToString(context.HttpContext.Request));
                return;
            }

            //非get请求，请求头验证
            if (!HeaderCheck(context))
            {
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "AddKeyAction", new KeyAction()
                {
                    AdminId = adminId,
                    Action = "header认证失败",
                    Description = "header认证失败",
                    Router = $"{area}/{controller}/{action}",
                    CreatedAt = DateTime.Now
                });

                Assistant.Logger.Error("header认证失败 \r\n" + HeadersToString(context.HttpContext.Request));
                return;
            }

            //用户权限验证
            if(descriptor!=null && descriptor.MethodInfo.CustomAttributes.Where(c => c.AttributeType.Name.Contains("RouteMark")).Any())
            {
                string router = $"/{area}/{controller}/{action}";
                if (string.IsNullOrEmpty(area))
                    router = $"/{controller}/{action}";
                string permissionCheckResult = await PermissionCheck(adminId.ToString(), router);
                if (permissionCheckResult != "success")
                {
                    //var item = new ContentResult();
                    //item.Content = permissionCheckResult + "," + router;
                    //context.Result = item;

                    var item = new RedirectResult("/system/account/login?msg="+ permissionCheckResult);
                    context.Result = item;
                    Assistant.Logger.Error("权限异常");
                    await _capPublisher.PublishAsync(CapConsts.PREFIX + "AddKeyAction", new KeyAction()
                    {
                        AdminId = adminId,
                        Action = "权限认证失败",
                        Description = permissionCheckResult,
                        Router = $"{area}/{controller}/{action}",
                        CreatedAt = DateTime.Now,
                        Type = 1
                    });
                    return;
                }

            }
            

            //string header = context.HttpContext.Request.Headers["Authorization"];
            Assistant.Logger.Debug("area:" + area + " controller:" + controller + " action:" + action);

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


        private bool CookieCheck(AuthorizationFilterContext context,out Guid adminId)
        {
            adminId = Guid.Empty;
            var cookies = context.HttpContext.Request.Cookies;
            if (!cookies.Where(u => u.Key == "userId").Any() || !cookies.Where(u => u.Key == "examToken").Any())
            {
                var item = new RedirectResult("/system/account/login?msg=notauth");
                context.Result = item;
                Assistant.Logger.Error("没登录！走你~");
                return false;
            }
            adminId = Guid.Parse(
                Assistant.Utils.FromBase64Str(
                    cookies.Where(u => u.Key == "userId").First().Value
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
                var item = new RedirectResult("/system/account/login?msg=invalidtoken");
                context.Result = item;
                Assistant.Logger.Error("token错辣！走你~");
                //Assistant.Logger.Error(HeadersToString(context.HttpContext.Request));
                return false;
            }
            if (claim.Sid != cookies.Where(u => u.Key == "userId").First().Value || claim.exp - Assistant.Utils.DateTimeToTimeStamp(DateTime.Now) < 0)
            {
                var item = new RedirectResult("/system/account/login?msg=invalidtoken");
                context.Result = item;
                Assistant.Logger.Error("token错辣或者超时辣！走你~");
                //Assistant.Logger.Error(HeadersToString(context.HttpContext.Request));
                return false;
            }
            
            return true;
        }

        private async Task<string> PermissionCheck(string adminId,string router)
        {
            //todo..
            //主要逻辑是，获取用户的所有权限，如果用户有这个权限，则通过，否则不通过
            //注意不通过时，不要直接返回登录页，要返回一个jsonresult，提示用户没有权限
            //同时，根据一定情况，如非get类请求，组成keyaction模型，插入keyaction表中
            //插入的时候发布一个消息就行，不用调用仓库立即执行

            if (string.IsNullOrEmpty(router))
            {
                return "success";
            }
            if (!router.StartsWith("/"))
                router = ("/" + router).ToLower();
            string key = $"GD.Exam.Permissions_{adminId}";
            if(!_redisCachingProvider.KeyExists(key)) 
            {
                return "tokenlost";//服务端登录信息丢失
            }
            if (await _redisCachingProvider.HExistsAsync(key, "super") || 
                await _redisCachingProvider.HExistsAsync(key, router))
                return "success";
            return "当前帐号没有执行此操作的权限";
        }

        private async Task<AuthorizationFilterContext> ApiCheck(AuthorizationFilterContext context)
        {
            string requestMethod = context.HttpContext.Request.Method;
            
            var headers = context.HttpContext.Request.Headers;
            if (!headers.Where(h => h.Key == "Authorization").Any())
            {
                Assistant.Logger.Error("缺少token");
                var item = new JsonResult(new { code = 401, msg = "令牌丢失，请尝试重新登录" });
                //item.StatusCode = 401;
                //var item = new RedirectResult("/error?msg=登录异常");
                context.Result = item;
                return context;
                //return false;
            }
            string Authorization = headers.Where(h => h.Key == "Authorization").FirstOrDefault().Value;
            var claim = Assistant.JwtService.ValidateJwt(Authorization);
            if (claim == null)
            {
                var item = new JsonResult(new { code = 400, msg = "非法访问" });
                context.Result = item;
                Assistant.Logger.Error("token错辣！走你~");
                return context;
            }
            string sid = Assistant.Utils.FromBase64Str(claim.Sid).ToLower();
            var t = claim.exp - Assistant.Utils.DateTimeToTimeStamp(DateTime.Now);
            if (!await _redisCachingProvider.KeyExistsAsync($"GD.Exam.Permissions_{sid}")  || claim.exp - Assistant.Utils.DateTimeToTimeStamp(DateTime.Now) < 0)
            {
                var item = new JsonResult(new { code = 400, msg = "token错误" });
                context.Result = item;
                Assistant.Logger.Error("token错辣或者超时辣！走你~");
                return context;
            }
            await _capPublisher.PublishAsync(CapConsts.PREFIX + "AddKeyAction", new KeyAction()
            {
                AdminId = Guid.Parse(sid),
                Action = "api接口访问",
                Description = HeadersToString(context.HttpContext.Request),
                Router = context.HttpContext.Request.GetDisplayUrl(),
                CreatedAt = DateTime.Now
            });
            await _redisCachingProvider.StringSetAsync(Security.GenerateMD5Hash(Authorization), sid, DateTime.Now.AddMinutes(2) - DateTime.Now);
            return context;
        }

        private string HeadersToString(HttpRequest request)
        {
            StringBuilder headersBuilder = new StringBuilder();

            foreach (var header in request.Headers)
            {
                headersBuilder.AppendFormat("{0}: {1}\r\n", header.Key, header.Value);
            }


            return headersBuilder.ToString();
        }
    }
}
