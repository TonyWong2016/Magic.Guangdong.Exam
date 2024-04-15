using Magic.Guangdong.DbServices.Dtos;
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
        /// <summary>
        /// 活动报名列表
        /// </summary>
        /// <param name="pageDto"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        dynamic GetReportInfos(PageDto pageDto, out long total);

        /// <summary>
        /// 导出活动报名列表
        /// </summary>
        /// <param name="whereJsonStr"></param>
        /// <returns></returns>
        Task<List<ExportReportInfo>> GetReportInfosForExcel(string whereJsonStr);

        Task<bool> ReportActivity(ReportInfoDto dto);

        /// <summary>
        /// 获取报名列表
        /// for client
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<ReportOrderList> GetReportOrderListClient(GetReportListDto dto);
        
        Task<dynamic> GetReportDetailByOutTradeNo(string outTradeNo);
    }
}
