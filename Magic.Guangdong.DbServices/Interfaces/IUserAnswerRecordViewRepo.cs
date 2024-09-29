using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Dtos.Exam.UserAnswerRecord;
using Magic.Guangdong.DbServices.Entities;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IUserAnswerRecordViewRepo : IExaminationRepository<UserAnswerRecordView>
    {
        List<UserAnswerSubmitRecordDto> GetUserRecord(PageDto dto, out long total);

        List<TeacherPapersDto> GetTeacherPapers(PageDto dto, out long total);

        Task<List<UserAnswerRecordDto>> GetUserRecordForExport(string whereJsonStr);
        Task<dynamic> GetUserAnswer(long urid);

        Task<bool> RemoveUserRecord(long urid, string adminId);

        Task<UserAnswerRecordView> ForceMarking(long urid);

        Task<bool> AddQuestionRecord(long urid);

        Task<List<long>> GetNotComplatedList(Guid examId);

        Task<dynamic> GetUserAnswerRecordApi(string[] reportIds, int? examType, int? isDeleted);
    }
}
