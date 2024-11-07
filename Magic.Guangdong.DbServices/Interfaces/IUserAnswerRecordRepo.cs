using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Entities;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IUserAnswerRecordRepo : IExaminationRepository<UserAnswerRecord>
    {
        Task<dynamic> GetExaminations(string associationId);

        Task<dynamic> ConfirmMyPaper(UserPaperRecordDto dto);

        Task<FinalPaperDto> GetMyPaper(Guid paperId);

        Task<dynamic> SubmitMyPaper(SubmitMyAnswerDto dto);

        [Obsolete("过期，客观题打分请统一使用ScoreObjectivePart方法")]
        Task<UserAnswerRecord> Marking(long urid,bool submit);
    }
}
