using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Report.ReportInfo
{
    public class ReportOrderList()
    {
        public List<ReportOrderView> items { get; set; }

        public long total { get; set; }
    }
}
