using DotNetCore.CAP.Filter;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
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
            if (await _redis.HExistsAsync(CapConsts.MsgIdCacheOaName, msgKey))
            {
                Assistant.Logger.Warning(JsonHelper.JsonSerialize(context.MediumMessage));

                throw new Exception("消息重复");
                // return;
            }
            // 订阅方法执行前
            //Assistant.Logger.Debug(JsonHelper.JsonSerialize(context.DeliverMessage));
            await base.OnSubscribeExecutingAsync(context);
        }

        public override async Task OnSubscribeExecutedAsync(ExecutedContext context)
        {
            string msgKey = context.DeliverMessage.Headers["cap-msg-id"] ?? "";

            if (string.IsNullOrEmpty(msgKey))
                await _redis.HSetAsync(CapConsts.MsgIdCacheOaName, msgKey, "processed");

            // 订阅方法执行后
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(JsonHelper.JsonSerialize(context.DeliverMessage.Headers));
            Console.WriteLine($"CapWarning:消费完成....{DateTime.Now}");
            Console.ResetColor();
            await base.OnSubscribeExecutedAsync(context);
        }

        public override async Task OnSubscribeExceptionAsync(ExceptionContext context)
        {
            Assistant.Logger.Error($"消费异常");
            Logger.Error(JsonHelper.JsonSerialize(context.DeliverMessage));
            context.ExceptionHandled = true;
            await base.OnSubscribeExceptionAsync(context);
        }
    }
}
