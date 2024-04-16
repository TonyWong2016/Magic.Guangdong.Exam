using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Report.ReportInfo
{
    public class ReportCheckHistoryDto
    {
        public long[] reportIds { get; set; }
        public CheckStatus checkStatus { get; set; }
        public Guid adminId { get; set; }
        public string checkRemark { get; set; }

        public ReportStatus reportStatus
        {
            get
            {
                if(checkStatus == CheckStatus.Passed)
                {
                    return ReportStatus.Succeed;
                }
                if(checkStatus == CheckStatus.UnPassed)
                {
                    return ReportStatus.Failed;
                }
                return ReportStatus.UnChecked;
            }
        }
    }
}
