using Magic.Guangdong.DbServices.Dtos.Report.Activities;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IActivityRepo : IExaminationRepository<Activity>
    {
        Task<ActivityReportDto> GetActivityReportPoints(long activityId);
    }
}
