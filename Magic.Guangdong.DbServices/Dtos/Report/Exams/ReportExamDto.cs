using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Report.Exams
{
    public class ReportExamDto
    {
        public Guid? examId { get; set; }

        public long? reportId { get; set; }

        public string groupCode { get; set; }

        public string accountId { get; set; }

        public string IdCard { get; set; }

        public string ReportNumber { get; set; }

        public bool isVaild
        {
            get
            {
                //这4个参数必须得有一个不为空
                if(examId == null && reportId == null && string.IsNullOrWhiteSpace(groupCode) && string.IsNullOrWhiteSpace(accountId))
                    return false;
                //身份证号和准考证号必须有一个不为空
                //if(string.IsNullOrWhiteSpace(IdCard) && string.IsNullOrWhiteSpace(ReportNumber))
                //    return false;
                return true;
            }
        }
    }
}
