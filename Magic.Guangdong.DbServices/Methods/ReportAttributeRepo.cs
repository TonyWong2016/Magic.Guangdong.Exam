using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class ReportAttributeRepo : ExaminationRepository<ReportAttribute>, IReportAttributeRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public ReportAttributeRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        /// <summary>
        /// 同步申报属性，注意上传调用前要检查reportid是否存在
        /// </summary>
        /// <param name="reportAttribute"></param>
        /// <returns></returns>
        public async Task<bool> SyncReportAttribute(ReportAttributeParam reportAttribute)
        {
            using (var attrRepo = fsql.Get(conn_str).GetRepository<ReportAttribute>())
            {
                try
                {
                    if (!await attrRepo.Where(u => u.ReportId == reportAttribute.ReportId && u.IsDeleted == 0).AnyAsync())
                    {
                        await attrRepo.InsertAsync(reportAttribute.Adapt<ReportAttribute>());
                        return true;
                    }
                    var reportAttr = await attrRepo
                        .Where(u => u.ReportId == reportAttribute.ReportId && u.IsDeleted == 0)
                        .ToOneAsync();
                    reportAttr.Participants = reportAttribute.Participants;
                    reportAttr.ProjectNo = reportAttribute.ProjectNo;
                    reportAttr.Teachers = reportAttribute.Teachers;
                    reportAttr.Schools = reportAttribute.Schools;
                    reportAttr.TeamName = reportAttribute.TeamName;
                    reportAttr.GroupName = reportAttribute.GroupName;
                    reportAttr.UpdatedAt = DateTime.Now;

                    await attrRepo.UpdateAsync(reportAttr);
                    return true;
                }
                catch (Exception ex) {
                    Assistant.Logger.Error(ex);
                    return false;
                }
            }
        }
    }
}
