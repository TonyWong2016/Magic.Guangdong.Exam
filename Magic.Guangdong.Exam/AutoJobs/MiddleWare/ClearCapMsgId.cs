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
            await Task.Delay(new Random().Next(100, 5000));
            if (await _redisCachingProvider.KeyExistsAsync("autoclearcapcache"))
            {
                Assistant.Logger.Info("自动清理缓存的服务已分配到其他终端");
                return;
            }
            await _redisCachingProvider.StringSetAsync("autoclearcapcache", DateTime.Now.ToString(), TimeSpan.FromMinutes(10));

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
