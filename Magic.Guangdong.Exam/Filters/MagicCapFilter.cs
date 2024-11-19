using DotNetCore.CAP.Filter;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using StackExchange.Redis;

namespace Magic.Guangdong.Exam.Filters
{
    public class MagicCapFilter : SubscribeFilter
    {
        private readonly IRedisCachingProvider _redis;

        public MagicCapFilter(IRedisCachingProvider redis)
        {
            _redis = redis;
        }

        public override async Task OnSubscribeExecutingAsync(ExecutingContext context)
        {
            string msgKey = context.DeliverMessage.Headers["cap-msg-id"];
            if (await _redis.HExistsAsync("capExamOaMsgs", msgKey))
            {
                return;
            }
            await _redis.HSetAsync("capExamOaMsgs", msgKey, "processed");
            // 订阅方法执行前
            Assistant.Logger.Debug(JsonHelper.JsonSerialize(context.MediumMessage));
            Assistant.Logger.Debug(JsonHelper.JsonSerialize(context.DeliverMessage));
            await base.OnSubscribeExecutingAsync(context);
        }

        //public override Task OnSubscribeExecutedAsync(ExecutedContext context)
        //{
        //    // 订阅方法执行后
        //}

        //public override Task OnSubscribeExceptionAsync(ExceptionContext context)
        //{
        //    // 订阅方法执行异常
        //}
    }
}
