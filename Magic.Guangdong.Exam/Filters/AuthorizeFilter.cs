using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Magic.Guangdong.Exam.Filters
{
    /// <summary>
    /// 实现自定义授权
    /// </summary>
    public class AuthorizeFilter : IAuthorizationFilter
    {
        private readonly IAdminRoleRepo _adminRoleRepo;
        //private readonly IKeyActionRepo _keyActionRepo;
        public AuthorizeFilter(IAdminRoleRepo adminRoleRepo)
        {
            _adminRoleRepo = adminRoleRepo;
            //_keyActionRepo = keyActionRepo;
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
            Guid adminId;
            if(!HeaderCheck(context) || !CookieCheck(context, out adminId))
            {
                return;
            }

            //string header = context.HttpContext.Request.Headers["Authorization"];
            Assistant.Logger.Info("area:" + area + " controller:" + controller + " action:" + action);

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
                var item = new JsonResult(new { code = 401, msg = "缺少token" });
                item.StatusCode = 401;
                context.Result = item;
                return false;
            }
            string Authorization = Assistant.Utils.FromBase64Str(headers.Where(h => h.Key == "Authorization").FirstOrDefault().Value);
            if (!Authorization.StartsWith("GD.exam|"))
            {
                Assistant.Logger.Error("token错误");
                var item = new JsonResult(new { code = 401, msg = "token错误" });
                item.StatusCode = 401;
                context.Result = item;
                return false;
            }

            string[] parts = Authorization.Split('|');
            long exp = Convert.ToInt64(parts[1]);
            if (exp - Assistant.Utils.DateTimeToTimeStamp(DateTime.Now) < 0)
            {
                Assistant.Logger.Error("token过期");
                var item = new JsonResult(new { code = 401, msg = "token过期" });
                item.StatusCode = 401;
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
                var item = new RedirectResult("/system/admin/login?msg=notauth");
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
            //    var item = new RedirectResult("/system/admin/login?msg=invalidtoken");
            //    context.Result = item;
            //    Assistant.Logger.Error("token错辣！走你~");
            //    return false;
            //}
            var claim = Assistant.JwtService.ValidateJwt(examToken);
            if (claim == null)
            {
                var item = new RedirectResult("/system/admin/login?msg=invalidtoken");
                context.Result = item;
                Assistant.Logger.Error("token错辣！走你~");
                return false;
            }
            if (claim.Sid != cookies.Where(u => u.Key == "userId").First().Value || claim.exp - Assistant.Utils.DateTimeToTimeStamp(DateTime.Now) < 0)
            {
                var item = new RedirectResult("/system/admin/login?msg=invalidtoken");
                context.Result = item;
                Assistant.Logger.Error("token错辣或者超时辣！走你~");
                return false;
            }
            return true;
        }

        private async Task<bool> PermissionCheck(string adminId)
        {
            //to do..
            //主要逻辑是，获取用户的所有权限，如果用户有这个权限，则通过，否则不通过
            //注意不通过时，不要直接返回登录页，要返回一个jsonresult，提示用户没有权限
            //同时，根据一定情况，如非get类请求，组成keyaction模型，插入keyaction表中
            //插入的时候发布一个消息就行，不用调用仓库立即执行
            return true;
        }


    }
}
