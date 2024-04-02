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

        Task<UserAnswerRecord> Marking(long urid,bool submit);
    }
}
