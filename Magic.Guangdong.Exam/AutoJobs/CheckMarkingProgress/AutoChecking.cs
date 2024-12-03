using Coravel.Invocable;
using EasyCaching.Core;
using Magic.Guangdong.DbServices.Interfaces;

namespace Magic.Guangdong.Exam.AutoJobs.CheckMarkingProgress
{
    public class AutoChecking : IInvocable
    {
        private readonly IUserAnswerRecordRepo _userAnswerRecordRepo;
        private readonly IUserAnswerSubmitRecordRepo _userAnswerSubmitRecordRepo;
        private readonly IRedisCachingProvider _rediscachingProvider;
        public AutoChecking(IUserAnswerRecordRepo userAnswerRecordRepo,IUserAnswerSubmitRecordRepo userAnswerSubmitRecordRepo,IRedisCachingProvider redisCachingProvider)
        {
            _userAnswerRecordRepo = userAnswerRecordRepo;
            _userAnswerSubmitRecordRepo = userAnswerSubmitRecordRepo;
            _rediscachingProvider = redisCachingProvider;
        }

        public async Task Invoke()
        {
            await Task.Delay(new Random().Next(100, 5000));
            if (await _rediscachingProvider.KeyExistsAsync("autocheckscore"))
            {
                Assistant.Logger.Info("自动检查成绩的服务已分配到其他终端");
                return;
            }
            await _rediscachingProvider.StringSetAsync("autocheckscore", DateTime.Now.ToString(), TimeSpan.FromSeconds(300));
            Assistant.Logger.Warning("开始检查粗心大意交了卷没等成绩出来就走了的卷子");
            var noMarkingRecords = (await _userAnswerRecordRepo.getListAsync(
                u => u.IsDeleted == 0
                && (u.CreatedAt.Date == DateTime.Today || u.CreatedAt.Date == DateTime.Today.AddDays(-1))
                && (u.Complated == DbServices.Entities.ExamComplated.Yes || u.LimitedTime<DateTime.Now.AddMinutes(2))
                && u.ObjectiveScore == 0
                && u.Marked == DbServices.Entities.ExamMarked.No))
                .Select(u => new { u.Id, u.Complated, u.ComplatedMode,u.UserName,u.IdNumber })
                .ToList();

            if (noMarkingRecords.Count > 0)
            {
                Assistant.Logger.Warning($"还真有{noMarkingRecords.Count}张卷子");
                foreach (var record in noMarkingRecords)
                {
                    Assistant.Logger.Warning($"--urid:{record.Id},答题人:{record.UserName},证件号:{record.IdNumber}--");
                    await _userAnswerSubmitRecordRepo.ScoreObjectivePart(record.Id, (int)record.ComplatedMode);
                }
            }

        }
    }
}
