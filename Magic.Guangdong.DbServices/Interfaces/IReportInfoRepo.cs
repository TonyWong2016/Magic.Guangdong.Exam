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

        /// <summary>
        /// 审查报名
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> CheckReportInfo(ReportCheckHistoryDto dto);
        Task<bool> ReportActivity(ReportInfoDto dto);

        /// <summary>
        /// 获取报名列表
        /// for client
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>        
        Task<ReportOrderList> GetReportOrderListClient(GetReportListDto dto);
            
        Task<dynamic> GetReportDetailByOutTradeNoForClient(string outTradeNo);
        
        /// <summary>
        /// 获取申报详情
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        Task<dynamic> GetReportDetailForClient(long reportId);

        /// <summary>
        /// 脱敏报名数据
        /// </summary>
        /// <returns></returns>
        Task<int> MaskReportInfoData();

        /// <summary>
        /// 安全插入报名数据
        /// </summary>
        /// <param name="reportInfo"></param>
        /// <returns></returns>
        Task<bool> InsertReportSecurity(ReportInfo reportInfo);
    }
}
