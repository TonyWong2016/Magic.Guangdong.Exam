﻿using DotNetCore.CAP.Filter;
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
                Assistant.Logger.Debug(JsonHelper.JsonSerialize(context.MediumMessage));

                throw new Exception("消息重复");
                // return;
            }
            await _redis.HSetAsync("capExamOaMsgs", msgKey, "processed");
            // 订阅方法执行前
            //Assistant.Logger.Debug(JsonHelper.JsonSerialize(context.DeliverMessage));
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