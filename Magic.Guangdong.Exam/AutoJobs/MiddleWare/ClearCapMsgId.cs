using Coravel.Invocable;
using EasyCaching.Core;

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
            Assistant.Logger.Debug("检查cap缓存" + DateTime.Now);
            //作废clientsigns
            await _redisCachingProvider.KeyDelAsync("capExamOaMsgs");
            await _redisCachingProvider.KeyDelAsync("capExamClientMsgs");
            await _redisCachingProvider.KeyDelAsync("capExamTeacherMsgs");
        }
    }
}
