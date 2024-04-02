using Magic.Guangdong.DbServices.Dtos.Exam.Examinations;
using Magic.Guangdong.DbServices.Entities;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IValidateExpressionRepo : IExaminationRepository<ValidateExpression>
    {
        Task<bool> InsertExamValidateExpressions(ExamValidateExpressionDto dto);
    }
}
