using Coravel.Invocable;
using Magic.Guangdong.DbServices.Interfaces;

namespace Magic.Guangdong.Exam.AutoJobs.CheckMarkingProgress
{
    public class AutoChecking : IInvocable
    {
        private readonly IUserAnswerRecordRepo _userAnswerRecordRepo;
        private readonly IUserAnswerSubmitRecordRepo _userAnswerSubmitRecordRepo;
        public AutoChecking(IUserAnswerRecordRepo userAnswerRecordRepo,IUserAnswerSubmitRecordRepo userAnswerSubmitRecordRepo)
        {
           _userAnswerRecordRepo = userAnswerRecordRepo;
            _userAnswerSubmitRecordRepo = userAnswerSubmitRecordRepo;
        }

        public async Task Invoke()
        {
            var noMarkingRecords = (await _userAnswerRecordRepo.getListAsync(
                u => u.IsDeleted == 0
                && u.CreatedAt.Date == DateTime.Today
               && u.Complated == DbServices.Entities.ExamComplated.Yes
               && u.Marked == 0))
               .Select(u => new {u.Id,u.Complated,u.ComplatedMode})
               .ToList();

            if (noMarkingRecords.Count > 0)
            {
                foreach (var record in noMarkingRecords)
                {
                    await _userAnswerSubmitRecordRepo.ScoreObjectivePart(record.Id, (int)record.ComplatedMode);
                }
            }
        }
    }
}
