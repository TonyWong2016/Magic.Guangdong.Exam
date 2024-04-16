using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IReportCheckHistoryRepo : IExaminationRepository<ReportCheckHistory>
    {
        Task NoticeReportResult(long[] reportIds);

        Task<List<ReportHistoryDto>> GetReportCheckHistory(long reportId);
    }
}
