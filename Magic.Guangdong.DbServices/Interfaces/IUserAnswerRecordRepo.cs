using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
