using Coravel.Invocable;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.DbServices.Interfaces;
using MessagePack;

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
            try
            {

                Assistant.Logger.Warning("检查并移除临时生成的文件，每个终端都要检查");
                if (await _redisCachingProvider.KeyExistsAsync("tempfiles"))
                {
                    var files = await _redisCachingProvider.HValsAsync("tempfiles");
                    foreach (var item in files)
                    {
                        if (File.Exists(item))
                            File.Delete(item);
                    }
                    //别直接删，避免其他终端没检查完造成错误，延时10分钟
                    await _redisCachingProvider.KeyExpireAsync("tempfiles", 600);
                }

                await Task.Delay(new Random().Next(100, 5000));
                if (await _redisCachingProvider.KeyExistsAsync("autoclearcache"))
                {
                    Assistant.Logger.Info("自动清理缓存的服务已分配到其他终端");
                    return;
                }
                await _redisCachingProvider.StringSetAsync("autoclearcache", DateTime.Now.ToString(), TimeSpan.FromMinutes(10));
                Assistant.Logger.Warning("检查过期缓存");
                //作废clientsigns
                await _redisCachingProvider.KeyDelAsync("clientsigns");
                await _redisCachingProvider.KeyDelAsync("markingProcess");
                await _redisCachingProvider.KeyDelAsync("generationCertMark");
                Assistant.Logger.Warning("过期缓存检查完成");

                Assistant.Logger.Warning("检查并移除过期的keyaction");
                await _keyActionRepo.delItemAsync(u => u.ExpiredAt <= DateTime.Now);

                Assistant.Logger.Warning("检查并移除过期的草稿记录");
                await _userAnswerSubmitRecordRepo.delItemAsync(u => u.ExpiredAt <= DateTime.Now);
                await _userAnswerSubmitLogRepo.delItemAsync(u => u.ExpiredAt <= DateTime.Now);
                

            }
            catch (Exception ex) {
                Assistant.Logger.Error(ex);
            }
        }
    }
}
