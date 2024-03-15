using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{ 
    public interface IValidateExpressionRepo : IExaminationRepository<ValidateExpression>
    {
        Task<bool> InsertExamValidateExpressions(ExamValidateExpressionDto dto);
    }
}
