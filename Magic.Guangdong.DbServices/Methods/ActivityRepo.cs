using FreeSql.Internal.Model;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Report.Activities;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class ActivityRepo : ExaminationRepository<Activity>, IActivityRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public ActivityRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        /// <summary>
        /// 获取活动报名时的关键信息
        /// </summary>
        /// <returns></returns>
        public async Task<ActivityReportDto> GetActivityReportPoints(long activityId)
        {
            
            var activity = await fsql.Get(conn_str).Select<Activity>()
                .Where(u => u.Id == activityId)
                .ToOneAsync();
            
            var exams = await fsql.Get(conn_str).Select<Examination>()
                .Where(u => u.AssociationId == activityId.ToString() && u.Status == 0 && u.IsDeleted == 0)
                .ToListAsync(u => new ActivityExamDto()
                {
                    ExamId = u.Id,
                    ExamTitle = u.Title,
                    StartTime = u.StartTime,
                    EndTime = u.EndTime,
                    Expenses = u.Expenses,
                    Quota = u.Quota,
                    AttachmentId = u.AttachmentId,
                    examType = u.ExamType,
                });
            return new ActivityReportDto()
            {
                ActivityId = activity.Id,
                status = activity.Status,
                Title = activity.Title,
                Exams = exams,
            };
        }
    
        public List<ActivityItemsDto> GetActivityItemsClient(PageDto pageDto,out long total)
        {
            if(string.IsNullOrEmpty(pageDto.whereJsonStr))
            {
                var noWhereItems = fsql.Get(conn_str).Select<Activity>()
                    .Where(u=>u.IsDeleted==0)
                    .OrderByPropertyName(pageDto.orderby, pageDto.isAsc)
                    .Count(out total)
                    .Page(pageDto.pageindex, pageDto.pagesize)
                    .ToList();
                return noWhereItems.Adapt<List<ActivityItemsDto>>();
            }
            DynamicFilterInfo filter = JsonConvert.DeserializeObject<DynamicFilterInfo>(pageDto.whereJsonStr);
            var items = fsql.Get(conn_str).Select<Activity>()
                    .Where(u => u.IsDeleted == 0)
                    .WhereDynamicFilter(filter)
                    .OrderByPropertyName(pageDto.orderby, pageDto.isAsc)
                    .Count(out total)
                    .Page(pageDto.pageindex, pageDto.pagesize)
                    .ToList();
            return items.Adapt<List<ActivityItemsDto>>();
        }
    
        public async Task<List<ActivityDrops>> GetActivityDrops()
        {
            return await fsql.Get(conn_str).Select<Activity>()
                .Where(u => u.IsDeleted == 0)
                .ToListAsync(u => new ActivityDrops
                {
                    Id = u.Id,
                    Title = u.Title,
                });
                
        }
    }
}
