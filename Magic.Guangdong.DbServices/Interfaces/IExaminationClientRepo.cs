using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.Examinations;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IExaminationClientRepo : IExaminationRepository<Examination>
    {
        /// <summary>
        /// 该方法适用于非独立报名模式，既依托申报系统和用户中心
        /// 获取报名的考试列表执行之前先调用AnyReportExamsForClient进行检查
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<List<ReportExamView>?> GetReportExamsForClient(ReportExamDto dto);

        /// <summary>
        /// 该方法适用于独立报名模式，不依托申报系统和用户中心
        /// 但该方法只能放考试信息，无法连同报名信息一同返回，需要单独请求
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<List<ExaminationDto>?> GetExamsForClient(OnlyGetExamDto dto);

        Task<string> InfoVerificationAuto(ReportExamDto dto);

        Task<ReturnVerifyResult> InfoVerificationByNumber(OnlyGetExamDto dto);

        Task<ReportExamView> GetReportExamView(long reportId);
    }
}
