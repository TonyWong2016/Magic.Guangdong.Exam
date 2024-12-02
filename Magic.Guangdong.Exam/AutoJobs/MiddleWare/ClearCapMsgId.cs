using Coravel.Invocable;
using EasyCaching.Core;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.DbServices.Interfaces;

namespace Magic.Guangdong.Exam.AutoJobs.MiddleWare
{
    public class ClearCapMsgId : IInvocable
    {
        private readonly IRedisCachingProvider _redisCachingProvider;

        public ClearCapMsgId(IRedisCachingProvider redisCachingProvider)
        {
            _redisCachingProvider = redisCachingProvider;
        }

        public async Task Invoke()
        {
            Assistant.Logger.Warning("检查cap缓存");
            await _redisCachingProvider.KeyDelAsync(CapConsts.MsgIdCacheOaName);
            await _redisCachingProvider.KeyDelAsync(CapConsts.MsgIdCacheClientName);
            await _redisCachingProvider.KeyDelAsync(CapConsts.MsgIdCacheTeacherName);
            Assistant.Logger.Warning("cap缓存检查完成");

            Assistant.Logger.Warning("清除判分队列");
            await _redisCachingProvider.KeyDelAsync("markingProcess");

           
        }
    }
}
