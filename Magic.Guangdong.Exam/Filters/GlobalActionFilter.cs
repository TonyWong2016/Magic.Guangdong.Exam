using Magic.Guangdong.Assistant;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Magic.Guangdong.Exam.Filters
{
    /// <summary>
    /// 全局动作执行过滤器
    /// </summary>
    public class GlobalActionFilter : IActionFilter
    {
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
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData == null ||
                context.RouteData.Values == null ||
                !context.RouteData.Values.Where(u => u.Key == "controller").Any() ||
                !context.RouteData.Values.Where(u => u.Key == "action").Any()
                )
            {
                context.Result = new JsonResult(new { code = -1, msg = "请求地址错误" });
                return;
            }
            string controller = Convert.ToString(context.RouteData.Values["controller"]).ToLower();
            string action = Convert.ToString(context.RouteData.Values["action"]).ToLower();
            //执行方法前先执行这
            var actionLog = $"{DateTime.Now} 开始调用 【{controller}/{action}】 api；参数为：{Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments)}";

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
                string requestMethod = context.HttpContext.Request.Method.ToLower();
                string userId = "admin";
                actionLog = $"用户【{userId}】，于{DateTime.Now} 开始调用 【{controller}/{action}】 api；参数为：{Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments)}";
                
                Logger.Warning(actionLog);
            }
            else
                Logger.Info(actionLog);
            RequestLog(context);
        }

        private void RequestLog(ActionExecutingContext context)
        {
            string ip = "";
            if (context.HttpContext.Request.Headers["x-Forwarded-For"].Any())
            {
                ip = context.HttpContext.Request.Headers["x-Forwarded-For"].FirstOrDefault();
            }
            else if (context.HttpContext.Request.Headers["X-Real-IP"].Any())
            {
                ip = context.HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            }
            else if (context.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"].Any())
            {
                ip = context.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"].FirstOrDefault();
            }
            else
            {
                ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            string user = "freeViewer";
            if (context.HttpContext.User.Claims.Any())
            {
                user = context.HttpContext.User.Claims.First().Value;
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
            string ip = "";
            if (context.HttpContext.Request.Headers["x-Forwarded-For"].Any())
            {
                ip = context.HttpContext.Request.Headers["x-Forwarded-For"].FirstOrDefault();
            }
            else if (context.HttpContext.Request.Headers["X-Real-IP"].Any())
            {
                ip = context.HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            }
            else if (context.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"].Any())
            {
                ip = context.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"].FirstOrDefault();
            }
            else
            {
                ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            string user = "freeViewer";
            if (context.HttpContext.User.Claims.Any())
            {
                user = context.HttpContext.User.Claims.First().Value;
            }
            string method = context.HttpContext.Request.Method;
            string url = context.HttpContext.Request.Path.Value;
            string param = context.HttpContext.Request.QueryString.Value;
            string remark = "";
            if (context.Result != null)
            {
                if (context.Result is JsonResult)
                    remark = JsonConvert.SerializeObject(((JsonResult)context.Result).Value).Replace("\"", "'");
                else if (context.Result is ObjectResult)
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
