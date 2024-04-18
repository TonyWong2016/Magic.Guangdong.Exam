using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
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
        Task<List<ReportExamView>> GetReportExamsForClient(ReportExamDto dto);

        Task<string> InfoVerificationAuto(ReportExamDto dto);
    }
}
