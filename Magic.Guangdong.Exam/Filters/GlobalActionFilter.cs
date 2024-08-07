using Magic.Guangdong.Assistant;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Magic.Guangdong.DbServices.Interfaces;
using DotNetCore.CAP;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.DbServices.Entities;
using Azure.Core;
using System;
using EasyCaching.Core;

namespace Magic.Guangdong.Exam.Filters
{
    /// <summary>
    /// 全局动作执行过滤器
    /// </summary>
    public class GlobalActionFilter : IActionFilter
    {
        private readonly ICapPublisher _capPublisher;
        private readonly IRedisCachingProvider _redisCachingProvider;
        public GlobalActionFilter(ICapPublisher capPublisher,IRedisCachingProvider redisCachingProvider)
        {
            _capPublisher = capPublisher;
            _redisCachingProvider = redisCachingProvider;
        }
        /// <summary>
        /// 动作执行后
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            string logResult = ConfigurationHelper.GetSectionValue("logResult");
            if (logResult == "yes")
                ResponseLog(context);
        }
        /// <summary>
        /// 动作执行时
        /// </summary>
        /// <param name="context"></param>
        public async void OnActionExecuting(ActionExecutingContext context)
        {
            RequestLog(context);
            Console.WriteLine("start:"+DateTime.Now.ToString());
            if (context.RouteData == null ||
                context.RouteData.Values == null ||
                !context.RouteData.Values.Where(u => u.Key == "controller").Any() ||
                !context.RouteData.Values.Where(u => u.Key == "action").Any()
                )
            {
                context.Result = new JsonResult(new { code = -1, msg = "请求地址错误" });
                return;
            }
            string userId = Guid.Empty.ToString();
            string requestMethod = context.HttpContext.Request.Method.ToLower();
            if (context.HttpContext.Request.Cookies.Where(u => u.Key == "userId").Any())
            {
                userId = Utils.FromBase64Str(context.HttpContext.Request.Cookies["userId"]);

            }
            else if (context.HttpContext.Request.Headers.Where(u => u.Key == "Authorization").Any())
            {
                string Authorization = context.HttpContext.Request.Headers.Where(h => h.Key == "Authorization").FirstOrDefault().Value;
                if (await _redisCachingProvider.KeyExistsAsync(Security.GenerateMD5Hash(Authorization)))
                {
                    userId = await _redisCachingProvider.StringGetAsync(Security.GenerateMD5Hash(Authorization));
                }
            }

            string area = "";
            string controller = Convert.ToString(context.RouteData.Values["controller"]).ToLower();
            string action = Convert.ToString(context.RouteData.Values["action"]).ToLower();
            if (context.RouteData.Values.ContainsKey("area"))
                area = context.RouteData.Values["area"].ToString().ToLower();
            //执行方法前先执行这
            var actionLog = $"{DateTime.Now} 开始调用 【{area}/{controller}/{action}】 api；参数为：{Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments)}";
            
            var descriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            if (descriptor == null)
            {
                return;
            }
            //标记为无需验证的操作直接跳过
            if (descriptor.MethodInfo.CustomAttributes.Any(u => u.AttributeType.Name == "AllowAnonymousAttribute"))
            {
                actionLog += ",（当前接口为开放类型）";
                Logger.Info(actionLog);
                return;
            }
            if (descriptor.MethodInfo.CustomAttributes.Any(u => u.AttributeType.Name == "RouteMark"))
            {
                actionLog = $"用户【{userId}】，于{DateTime.Now} 开始调用 【{area}/{controller}/{action}】 api；参数为：{Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments)}";

                Logger.Warning(actionLog);
            }
            else
                Logger.Info(actionLog);

            var routeMarks = descriptor.MethodInfo.CustomAttributes.Where(u => u.AttributeType.Name.Contains("RouteMark"));
            string actionMark = "";
            if (routeMarks != null && routeMarks.Any() && routeMarks.First().ConstructorArguments.Any())
            {
                actionMark = routeMarks.First().ConstructorArguments.First().Value.ToString();
            }
            if (requestMethod != "get")
            {                
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "AddKeyAction", new KeyAction()
                {
                    AdminId = Guid.Parse(userId),
                    Action = actionMark,
                    Description = actionLog.Length > 1800 ? actionLog.Substring(0, 1800) : actionLog,
                    Router = $"{area}/{controller}/{action}",
                    CreatedAt = DateTime.Now
                });
            }
           
            Console.WriteLine("end:" + DateTime.Now.ToString());
        }

        private void RequestLog(ActionExecutingContext context)
        {
            if (context.HttpContext == null || context.HttpContext.Request == null)
            {
                return;
            }
            string ip = "";
            
            var headers = context.HttpContext.Request.Headers;
            if (!headers.Any())
            {
                ip = "未知";
            }
            else if (headers.ContainsKey("x-Forwarded-For"))
            {
                ip = context.HttpContext.Request.Headers["x-Forwarded-For"].FirstOrDefault();
            }
            else if (headers.ContainsKey("X-Real-IP"))
            {
                ip = context.HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            }
            else if (headers.ContainsKey("HTTP_X_FORWARDED_FOR"))
            {
                ip = context.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"].FirstOrDefault();
            }
            else
            {
                ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            string user = Guid.Empty.ToString();
            //if (context.HttpContext.User.Claims.Any())
            //{
            //    user = context.HttpContext.User.Claims.First().Value;
            //}
            if(context.HttpContext.Request.Cookies.Where(u=>u.Key=="userId").Any())
            {
                user = Utils.FromBase64Str(context.HttpContext.Request.Cookies["userId"]);
            }
            string method = context.HttpContext.Request.Method;
            string url = context.HttpContext.Request.Path.Value;
            string param = Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments);
            string remark = context.HttpContext.Request.Headers["User-Agent"];
            
            string brower = "UnKnown";
            if (remark.Contains("FireFox"))
                brower = "FireFox";
            else if (remark.Contains("Chrome"))
                brower = "Chrome";
            else if (remark.Contains("Safari"))
                brower = "Safari";
            string logMode = ConfigurationHelper.GetSectionValue("LogMode");
            if (logMode == "es")
                Task.Run(() => Logger.writeLogToRedis($"{DateTime.Now.ToString("HH:mm:ss")} {user} {method} {url} \"{param}\" {ip} {brower} \"{remark}\"", "info"));
            else
            {
                string msg = $"{DateTime.Now.ToString("HH:mm:ss")} {user} {method} {url} \"{param}\" {ip} {brower} \"{remark}\"";
                Logger.Verbose(msg);
            }
        }

        private void ResponseLog(ActionExecutedContext context)
        {
            if (context.HttpContext == null || context.HttpContext.Request == null)
            {
                return;
            }
            string ip = "";
            
            var headers = context.HttpContext.Request.Headers;
            if (!headers.Any())
            {
                ip = "未知";
            }
            else if (headers.ContainsKey("x-Forwarded-For"))
            {
                ip = context.HttpContext.Request.Headers["x-Forwarded-For"].FirstOrDefault();
            }
            else if (headers.ContainsKey("X-Real-IP"))
            {
                ip = context.HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            }
            else if (headers.ContainsKey("HTTP_X_FORWARDED_FOR"))
            {
                ip = context.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"].FirstOrDefault();
            }
            else
            {
                ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            string user = "freeViewer";
            //if (context.HttpContext.User.Claims.Any())
            //{
            //    user = context.HttpContext.User.Claims.First().Value;
            //}
            if (context.HttpContext.Request.Cookies.Where(u => u.Key == "userId").Any())
            {
                user = context.HttpContext.Request.Cookies["userId"];
            }
            string method = context.HttpContext.Request.Method;
            string url = context.HttpContext.Request.Path.Value;
            string param = context.HttpContext.Request.QueryString.Value;
            string remark = "";
            if (context.Result != null)
            {
                if (context.Result is JsonResult && ((JsonResult)context.Result).Value != null)
                    remark = JsonConvert.SerializeObject(((JsonResult)context.Result).Value).Replace("\"", "'");
                else if (context.Result is ObjectResult && ((ObjectResult)context.Result).Value != null)
                    remark = JsonConvert.SerializeObject(((ObjectResult)context.Result).Value);
                if (remark.Length > 1000)
                {
                    remark = remark.Substring(0, 1000);
                }
            }
            string brower = "Response";
            string logMode = ConfigurationHelper.GetSectionValue("LogMode");
            if (logMode == "es")
                Task.Run(() => Logger.writeLogToRedis($"{DateTime.Now.ToString("HH:mm:ss")} {user} {method} {url} \"{param}\" {ip} {brower} \"{remark}\"", "info"));
            else
            {
                string msg = $"{DateTime.Now.ToString("HH:mm:ss")} {user} {method} {url} \"{param}\" {ip} {brower} \"{remark}\"";
                Logger.Verbose(msg);
            }
        }
        

    }
}
