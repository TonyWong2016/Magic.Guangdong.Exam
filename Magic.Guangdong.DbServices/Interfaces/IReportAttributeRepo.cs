using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IReportAttributeRepo : IExaminationRepository<ReportAttribute>
    {
        Task<bool> SyncReportAttribute(ReportAttributeParam reportAttribute);
    }
}
