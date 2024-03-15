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
            string controller = Convert.ToString(context.RouteData.Values["controller"]).ToLower();
            string action = Convert.ToString(context.RouteData.Values["action"]).ToLower();
            //执行方法前先执行这
            var actionLog = $"{DateTime.Now} 开始调用 【{controller}/{action}】 api；参数为：{Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments)}";

            var descriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            //标记为无需验证的操作直接跳过
            if (descriptor.MethodInfo.CustomAttributes.Any(u => u.AttributeType.Name == "AllowAnonymousAttribute"))
            {
                actionLog += ",（当前接口为开放类型）";
                Logger.Info(actionLog);
                return;
            }
            if (descriptor.MethodInfo.CustomAttributes.Count(u => u.AttributeType.Name == "Doorman") == 1 && (bool)(descriptor.MethodInfo.CustomAttributes.Where(u => u.AttributeType.Name == "Doorman").FirstOrDefault().ConstructorArguments.First().Value))
            {
                string requestMethod = context.HttpContext.Request.Method.ToLower();
                int userId = Convert.ToInt32(context.HttpContext.User.Claims.First().Value);
                actionLog = $"用户【{userId}】，于{DateTime.Now} 开始调用 【{controller}/{action}】 api；参数为：{Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments)}";
                //注意在授权的时候，要把关键的数据库写操作的方法设定成“post”,"put"等类型，不要设定成“get”！！
                if (!mayI(controller, action, userId, requestMethod))
                {
                    Logger.Info($"用户【{userId}】无权访问该地址【{controller}/{action}】");

                    if (requestMethod == "get")
                    {
                        var item = new ContentResult();
                        item.Content = "您无权访问当前地址或执行该操作";
                        item.StatusCode = 200;
                        context.Result = item;
                    }
                    else
                    {
                        var item2 = new JsonResult(new { code = -1, msg = "操作权限不足" });
                        context.Result = item2;
                    }
                }
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
        /// <summary>
        /// 执行权限判定
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="userId"></param>
        /// <param name="requestMethod"></param>
        /// <returns></returns>
        public bool mayI(string controller, string action, int userId, string requestMethod)
        {
           //根据路径检查是否具备访问权限
            return true;
        }
    }
}
