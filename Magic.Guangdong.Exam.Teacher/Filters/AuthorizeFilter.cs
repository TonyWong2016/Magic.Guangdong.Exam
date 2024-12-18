﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using EasyCaching.Core;
using DotNetCore.CAP;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.DbServices.Entities;

namespace Magic.Guangdong.Exam.Teacher.Filters
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

            
            //cookie身份验证
            Guid adminId;
            if (!CookieCheck(context, out adminId))
            {
                return;
            }

            //非get请求，请求头验证
            if (!HeaderCheck(context))
            {
                await _capPublisher.PublishAsync(CapConsts.TeacherPrefix + "AddKeyAction", new KeyAction()
                {
                    AdminId = adminId,
                    Action = "header认证失败",
                    Description = "header认证失败",
                    Router = $"{area}/{controller}/{action}",
                    CreatedAt = DateTime.Now
                });
                return;
            }

            #region
            //用户权限验证
            //if(descriptor!=null && descriptor.MethodInfo.CustomAttributes.Where(c => c.AttributeType.Name.Contains("RouteMark")).Any())
            //{
            //    string router = $"/{area}/{controller}/{action}";
            //    if (string.IsNullOrEmpty(area))
            //        router = $"/{controller}/{action}";
            //    string permissionCheckResult = await PermissionCheck(adminId.ToString(), router);
            //    if (permissionCheckResult != "success")
            //    {
            //        //var item = new JsonResult(new { code = 401, msg = permissionCheckResult });
            //        //var item = new RedirectResult("/error?msg="+ permissionCheckResult);
            //        var item = new ContentResult();
            //        item.Content = permissionCheckResult + "," + router;
            //        context.Result = item;
            //        Assistant.Logger.Error("权限异常");
            //        await _capPublisher.PublishAsync(CapConsts.PREFIX + "AddKeyAction", new KeyAction()
            //        {
            //            AdminId = adminId,
            //            Action = "权限认证失败",
            //            Description = permissionCheckResult,
            //            Router = $"{area}/{controller}/{action}",
            //            CreatedAt = DateTime.Now,
            //            Type = 1
            //        });
            //        return;
            //    }

            //}
            #endregion

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
            if (!cookies.Where(u => u.Key == "teacherId").Any() || !cookies.Where(u => u.Key == "teacherToken").Any())
            {
                var item = new RedirectResult("/system/account/login?msg=notauth");
                context.Result = item;
                Assistant.Logger.Error("没登录！走你~");
                return false;
            }
            adminId = Guid.Parse(
                Assistant.Utils.FromBase64Str(
                    cookies.Where(u => u.Key == "teacherId").First().Value
                    ));
            var teacherToken = cookies.Where(u => u.Key == "teacherToken").FirstOrDefault().Value;
            //if (!await Assistant.JwtService.ValidateFilter(teacherToken))
            //{
            //    var item = new RedirectResult("/system/account/login?msg=invalidtoken");
            //    context.Result = item;
            //    Assistant.Logger.Error("token错辣！走你~");
            //    return false;
            //}
            var claim = Assistant.JwtService.ValidateJwt(teacherToken);
            if (claim == null)
            {
                var item = new RedirectResult("/system/account/login?msg=invalidtoken");
                context.Result = item;
                Assistant.Logger.Error("token错辣！走你~");
                return false;
            }
            if (claim.Sid != cookies.Where(u => u.Key == "teacherId").First().Value || claim.exp - Assistant.Utils.DateTimeToTimeStamp(DateTime.Now) < 0)
            {
                var item = new RedirectResult("/system/account/login?msg=invalidtoken");
                context.Result = item;
                Assistant.Logger.Error("token错辣或者超时辣！走你~");
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
                return "登录异常";//
            }
            if (await _redisCachingProvider.HExistsAsync(key, "super") || 
                await _redisCachingProvider.HExistsAsync(key, router))
                return "success";
            return "当前帐号没有执行此操作的权限";
        }


    }
}
