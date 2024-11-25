using DotNetCore.CAP.Filter;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using StackExchange.Redis;

namespace Magic.Guangdong.Exam.Client.Filters
{
    
    public class MagicClientCapFilter: SubscribeFilter
    {
        private readonly IRedisCachingProvider _redis;

        public MagicClientCapFilter(IRedisCachingProvider redis)
        {
            _redis = redis;
        }
        public override async Task OnSubscribeExecutingAsync(ExecutingContext context)
        {
            // 订阅方法执行前
            string msgKey = context.DeliverMessage.Headers["cap-msg-id"];
            if (await _redis.HExistsAsync("capExamClientMsgs", msgKey))
            {
                Assistant.Logger.Warning("防止重发："+JsonHelper.JsonSerialize(context.MediumMessage));

                //throw new Exception("消息重复");
                return;
            }
            await _redis.HSetAsync("capExamClientMsgs", msgKey, "processed");
            Assistant.Logger.Debug(JsonHelper.JsonSerialize(context.DeliverMessage.Headers));
            await base.OnSubscribeExecutingAsync(context);
        }

        public override async Task OnSubscribeExecutedAsync(ExecutedContext context)
        {
            //Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(JsonHelper.JsonSerialize(context.DeliverMessage.Headers));
            Console.WriteLine($"CapWarning:消费完成....{DateTime.Now}");
            Console.ResetColor();
            // 订阅方法执行后
            //Logger.Info("消费完成");
            // Assistant.Logger.Info(JsonHelper.JsonSerialize(context.DeliverMessage.Headers));
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
