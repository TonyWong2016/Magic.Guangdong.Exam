using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Report.ReportInfo
{
    public class ExportReportInfo
    {
        public long Id { get; set; }

        public int orderStatus { get; set; }

        public int ReportStatus { get; set; }
        public string AccountId { get; set; }
        [Description("姓名")]
        public string Name { get; set; }


        public string IdCard { get; set; }
        [Description("证件号")]
        public string SecurityIdCard { get; set; }


        [Description("准考证号")]
        public string ReportNumber { get; set; }

        [Description("省份")]
        public string ProvinceName { get; set; }

        [Description("城市")]
        public string CityName { get; set; }

        [Description("区县")]
        public string DistrictName { get; set; }

        [Description("电子邮箱")]
        public string Email { get; set; }

        public string Mobile { get; set; }

        [Description("电话")]
        public string SecurityMobile { get; set; }

        [Description("单位")]
        public string Job { get; set; }

        [Description("考试科目")]
        public string Subject { get; set; }


        [Description("报名状态")]
        public string ReportStatusStr
        {
            get
            {
                if (ReportStatus == 0)
                {
                    return "报名成功";
                }
                else if (ReportStatus == 3)
                {
                    return "已退款";
                }
                else if (ReportStatus == 2)
                {
                    return "未审核";
                }
                else
                {
                    return "报名失败";
                }
            }
        }

        [Description("订单编号")]
        public string TradeNo { get; set; }

        [Description("退款编号")]
        public string RefundNo { get; set; }

        [Description("订单状态")]
        public string OrderStatusStr
        {
            get
            {
                if (orderStatus == 0)
                {
                    return "支付成功";
                }
                else if (orderStatus == 2)
                {
                    return "订单过期";
                }
                else if (orderStatus == 3)
                {
                    return "支付失败";
                }
                else if (orderStatus == 4)
                {
                    return "已退款";
                }
                return "待支付";
            }
        }
    }
}
