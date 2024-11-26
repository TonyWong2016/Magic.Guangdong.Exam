using Coravel.Invocable;
using EasyCaching.Core;
using Magic.Guangdong.Assistant.Contracts;

namespace Magic.Guangdong.Exam.AutoJobs.MiddleWare
{
    public class ClearCapMsgId : IInvocable
    {
        public readonly IRedisCachingProvider _redisCachingProvider;

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
        }
    }
}
