using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IReportInfoRepo : IExaminationRepository<ReportInfo>
    {
        Task<bool> ReportActivity(ReportInfoDto dto);

        Task<ReportOrderList> GetReportOrderList(GetReportListDto dto);
        
        Task<dynamic> GetReportDetailByOutTradeNo(string outTradeNo);
    }
}
