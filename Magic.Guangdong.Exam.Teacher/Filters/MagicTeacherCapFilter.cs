using DotNetCore.CAP.Filter;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;

namespace Magic.Guangdong.Exam.Teacher.Filters
{
    public class MagicTeacherCapFilter : SubscribeFilter
    {
        private readonly IRedisCachingProvider _redis;

        public MagicTeacherCapFilter(IRedisCachingProvider redis)
        {
            _redis = redis;
        }
        public override async Task OnSubscribeExecutingAsync(ExecutingContext context)
        {
            // 订阅方法执行前
            string msgKey = context.DeliverMessage.Headers["cap-msg-id"];
            if (await _redis.HExistsAsync("capExamTeacherMsgs", msgKey))
            {
                Assistant.Logger.Error("消息重复:" + JsonHelper.JsonSerialize(context.DeliverMessage));
                return;
            }
            await _redis.HSetAsync("capExamTeacherMsgs", msgKey, "processed");
            Assistant.Logger.Debug(JsonHelper.JsonSerialize(context.MediumMessage));
            Assistant.Logger.Debug(JsonHelper.JsonSerialize(context.DeliverMessage));
            await base.OnSubscribeExecutingAsync(context);
        }

        public override async Task OnSubscribeExecutedAsync(ExecutedContext context)
        {
            // 订阅方法执行后
            Logger.Debug("消费完成");
            Assistant.Logger.Debug(JsonHelper.JsonSerialize(context.DeliverMessage));
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
