using Coravel.Invocable;
using EasyCaching.Core;
using Magic.Guangdong.DbServices.Interfaces;

namespace Magic.Guangdong.Exam.AutoJobs.MiddleWare
{
    public class CacheHandle : IInvocable
    {
        public readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IUserAnswerSubmitLogRepo _userAnswerSubmitLogRepo;
        private readonly IUserAnswerSubmitRecordRepo _userAnswerSubmitRecordRepo;
        private readonly IKeyActionRepo _keyActionRepo;
        public CacheHandle(IRedisCachingProvider redisCachingProvider, IKeyActionRepo keyActionRepo, IUserAnswerSubmitLogRepo userAnswerSubmitLogRepo, IUserAnswerSubmitRecordRepo userAnswerSubmitRecordRepo)
        {
            _redisCachingProvider = redisCachingProvider;
            _keyActionRepo = keyActionRepo;
            _userAnswerSubmitLogRepo = userAnswerSubmitLogRepo;
            _userAnswerSubmitRecordRepo = userAnswerSubmitRecordRepo;
        }

        public async Task Invoke()
        {
            Assistant.Logger.Warning("检查过期缓存");
            //作废clientsigns
            await _redisCachingProvider.KeyDelAsync("clientsigns");
            await _redisCachingProvider.KeyDelAsync("markingProcess");
            Assistant.Logger.Warning("过期缓存检查完成");

            Assistant.Logger.Warning("检查并移除过期的keyaction");
            await _keyActionRepo.delItemAsync(u => u.ExpiredAt <= DateTime.Now);

            Assistant.Logger.Warning("检查并移除过期的草稿记录");
            await _userAnswerSubmitRecordRepo.delItemAsync(u => u.ExpiredAt <= DateTime.Now);
            await _userAnswerSubmitLogRepo.delItemAsync(u=>u.ExpiredAt <= DateTime.Now);
        }
    }
}
