using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IUserAnswerRecordClientRepo:IExaminationRepository<UserAnswerRecord>
    {
        Task<UserAnswerRecordView> GetUserRecordDetail(string idNumber, string reportId, Guid examId);
        Task<UserAnswerRecordView> GetUserRecordDetailById(long id);

        [Obsolete("请使用GetReportExamsForClient")]
        Task<List<SelectExaminationsDto>> GetExaminations(string associationId, string examId = "", string groupCode = "");

        Task<dynamic> ConfirmMyPaper(ConfirmPaperDto dto);

        Task<FinalPaperClientDto> GetMyPaper(Guid paperId);

        Task<UserAnswerRecordView> GetMyRecord(long urid);

        /// <summary>
        /// 获取账号下的答题记录
        /// 一个账号可以使用多个身份证号答题
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<List<ExamRecordDto>> GetMyAccountExamRecords(string accountId);

        Task<dynamic> SubmitMyPaper(SubmitMyAnswerDto dto);

        Task<UserAnswerRecordView> Marking(long urid, bool submit = false);

        Task<int> ClearTestData();
    }
}
