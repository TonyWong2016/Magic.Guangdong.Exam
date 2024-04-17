using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class ReportCheckHistoryRepo : ExaminationRepository<ReportCheckHistory>, IReportCheckHistoryRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public ReportCheckHistoryRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task NoticeReportResult(long[] reportIds)
        {
            var reportInfoRepo = fsql.Get(conn_str).GetRepository<ReportInfo>();
            var reportInfos = await reportInfoRepo.Where(r => reportIds.Contains(r.Id)).ToListAsync();
            var historyRepo = fsql.Get(conn_str).GetRepository<ReportCheckHistory>();
            List<MailboxAddress> toPassed = new List<MailboxAddress>();
            List<MailboxAddress> toUnPassed = new List<MailboxAddress>();
            foreach (var report in reportInfos)
            {
                var lastHistory = await historyRepo
                    .Where(u => u.ReportId == report.Id)
                    .OrderByDescending(u => u.CreatedAt)
                    .ToOneAsync();
                if (lastHistory.CheckStatus == CheckStatus.Passed)
                {
                    toPassed.Add(new MailboxAddress(report.Name, report.Email));
                }
                else if (lastHistory.CheckStatus == CheckStatus.UnPassed)
                {
                    toUnPassed.Add(new MailboxAddress(report.Name, report.Email));
                }
            }
            string templateFilePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "web", "emailnotice.html");
            string content = "";
            if (toPassed.Count > 0)
            {                
                using (StreamReader reader = new StreamReader(templateFilePath))
                {
                    content = reader.ReadToEnd()
                        .Replace("**content**", "恭喜您，您在广东教育协会的提交的报名信息已<b style='color:#16b777'>通过审核</b>，请尽快登录网站完成后续步骤")
                        .Replace("**username**","用户");

                }
                await EmailKitHelper.SendEMailAsync("报名审核通过",content, toPassed);
            }
            if (toUnPassed.Count > 0)
            {
                using (StreamReader reader = new StreamReader(templateFilePath))
                {
                    content = reader.ReadToEnd()
                        .Replace("**content**", "很遗憾，您在广东教育协会的提交的报名信息<b style='color:#ff5722'>没有通过审核</b>，如有疑问请联系协会相关人员")
                        .Replace("**username**", "用户");

                }
                await EmailKitHelper.SendEMailAsync("报名审核未通过", content, toUnPassed);
            }
        }

        public async Task<List<ReportHistoryDto>> GetReportCheckHistory(long reportId)
        {
            return await fsql.Get(conn_str).Select<ReportCheckHistory, Admin>()
                .LeftJoin((a, b) => a.AdminId == b.Id.ToString())
                .Where((a, b) => a.ReportId == reportId)
                .ToListAsync((a, b) => new ReportHistoryDto()
                {
                    reportId = reportId,
                    checkStatus = a.CheckStatus,
                    CheckRemark = a.CheckRemark,
                    adminId = a.AdminId,
                    adminName = b.Name,
                    checkTime = a.CreatedAt
                });
        }
    }

    public class ReportHistoryDto
    {
        public long reportId { get; set; }

        public CheckStatus checkStatus { get; set; }

        public string CheckRemark { get; set; }

        public string checkStatusStr
        {
            get
            {
                if (checkStatus == CheckStatus.Passed)
                {
                    return "通过";
                }
                if(checkStatus == CheckStatus.UnPassed)
                {
                    return "未通过";
                }
                return "";
            }
        }

        public string adminId { get; set; }

        public string adminName { get; set; }

        public DateTime checkTime { get; set; }
    }
}
