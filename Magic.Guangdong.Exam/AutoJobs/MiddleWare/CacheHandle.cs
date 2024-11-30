using Coravel.Invocable;
using EasyCaching.Core;

namespace Magic.Guangdong.Exam.AutoJobs.MiddleWare
{
    public class CacheHandle : IInvocable
    {
        public readonly IRedisCachingProvider _redisCachingProvider;

        public CacheHandle(IRedisCachingProvider redisCachingProvider)
        {
            _redisCachingProvider = redisCachingProvider;
        }

        public async Task Invoke()
        {
            Assistant.Logger.Warning("检查过期缓存");
            //作废clientsigns
            await _redisCachingProvider.KeyDelAsync("clientsigns");
            await _redisCachingProvider.KeyDelAsync("markingProcess");
            Assistant.Logger.Warning("过期缓存检查完成");
        }
    }
}
