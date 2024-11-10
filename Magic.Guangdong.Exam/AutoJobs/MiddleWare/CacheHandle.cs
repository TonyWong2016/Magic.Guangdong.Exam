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
            Assistant.Logger.Debug("检查过期缓存" + DateTime.Now);
            //作废clientsigns
            await _redisCachingProvider.KeyDelAsync("clientsigns");
        }
    }
}
