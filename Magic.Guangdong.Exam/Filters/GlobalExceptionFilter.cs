using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Magic.Guangdong.Assistant;

namespace Magic.Guangdong.Exam.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// 全局异常过滤器
        /// </summary>
        /// <param name="context"></param>
        public async void OnException(ExceptionContext context)
        {
            Exception ex = context.Exception;
            string errMsg = "GlobalExceptionFilter-OnException：" + ex.Message;            
            context.Result = new JsonResult(new { code = -1, msg = "系统错误，请稍后再试" });
            context.ExceptionHandled = true;
            Logger.Error("全局异常:"+errMsg);
            //await EmailKitHelper.SendEMailToDevExceptionAsync(ex);
            return;
        }
    }
}
