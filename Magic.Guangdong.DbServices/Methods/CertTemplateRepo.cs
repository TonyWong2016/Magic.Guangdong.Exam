using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos.Cert;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class CertTemplateRepo: ExaminationRepository<CertTemplate>, ICertTemplateRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public CertTemplateRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<bool> CloneTemplate(long templateId, string adminId)
        {
            var templateRepo = fsql.Get(conn_str).GetRepository<CertTemplate>();

            if (!await templateRepo.Where(x => x.Id == templateId).AnyAsync())
            {
                return false;
            }

            var template = await templateRepo.Where(u => u.Id == templateId)
                .ToOneAsync();
            template.Id = YitIdHelper.NextId();
            template.Remark = "快速克隆";
            template.Title = $"克隆_{template.Title}_{Utils.GenerateRandomCodePro(4)}";
            template.UpdatedAt = DateTime.Now;
            template.CreatedAt = DateTime.Now;
            template.CreatedBy = adminId;
            await templateRepo.InsertAsync(template);
            return true;
        }
   
        public async Task CacheActivitiesAndExams(List<ImportTemplateDto> importList)
        {
            if (importList.Where(u => u.ActivityTitle != string.Empty).Any())
            {
                var titles = importList.Select(u => u.ActivityTitle).Distinct();
                var activityRepo = fsql.Get(conn_str).GetRepository<Activity>();
                var activities = await activityRepo
                    .Where(u => titles.Contains(u.Title) && u.IsDeleted==0)
                    .ToListAsync(u => new
                {
                    u.Id,
                    u.Title
                });
                if (activities.Any()) {
                    foreach (var activity in activities) 
                    { 
                        await RedisHelper.HSetAsync("ImportActivities", activity.Title, activity.Id.ToString());
                    }
                    await RedisHelper.ExpireAsync("ImportActivities", TimeSpan.FromHours(1));
                }
            }

            if (importList.Where(u => u.ExamTitle != string.Empty).Any()) { 
                var titles = importList.Select(u => u.ExamTitle).Distinct();
                var examRepo = fsql.Get(conn_str).GetRepository<Examination>();
                var exams = await examRepo
                    .Where(u => titles.Contains(u.Title) && u.IsDeleted==0)
                    .ToListAsync(u => new
                {
                    u.Id,
                    u.Title
                });
                if (exams.Any()) {
                    foreach (var exam in exams) {
                        await RedisHelper.HSetAsync("ImportExams", exam.Title, exam.Id.ToString());
                    }
                    await RedisHelper.ExpireAsync("ImportExams", TimeSpan.FromHours(1));
                }
            }
        }
    }
}
