using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using FreeSql.Internal.Model;
using MassTransit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class ExaminationRepo : ExaminationRepository<Examination>, IExaminationRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public ExaminationRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<dynamic> GetExamMini(string id, int type)
        {
            return await fsql.Get(conn_str).Select<Examination>()
                .Where(u => u.IsDeleted == 0)
                .WhereIf(type == 0 && !string.IsNullOrEmpty(id), u => u.AssociationId == id)
                .WhereIf(type == 1 && !string.IsNullOrEmpty(id), u => u.Id == Guid.Parse(id))
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(u => new
                {
                    value = u.Id,
                    text = u.Title,
                    score = u.BaseScore
                    //其余属性有必要在添加
                });
        }

        public dynamic GetExamList(PageDto dto, out long total)
        {
            if (string.IsNullOrWhiteSpace(dto.whereJsonStr))
                return fsql.Get(conn_str)
                    .Select<Examination>()
                    .Count(out total)
                    .OrderByDescending(u => u.StartTime)
                    .OrderBy(u => u.GroupCode)
                    .OrderBy(u => u.OrderIndex)
                    .Page(dto.pageindex, dto.pagesize)
                    .ToList();

            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(dto.whereJsonStr);
            //string sql = fsql.Get(conn_str).Select<T>().WhereDynamicFilter(dyfilter).Count(out total).Page(pageDto.pageindex, pageDto.pagesize).ToSql();
            //Console.Write(sql);
            return fsql.Get(conn_str)
                .Select<Examination>()
                .WhereDynamicFilter(dyfilter)
                .OrderByDescending(u => u.StartTime)
                .OrderBy(u => u.GroupCode)
                .OrderBy(u => u.OrderIndex)
                .Count(out total)
                .Page(dto.pageindex, dto.pagesize)
                .ToList();
        }

        public async Task<bool> UpdateExamInfo(Examination exam)
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var paperRepo = fsql.Get(conn_str).GetRepository<Paper>();
                    if (await paperRepo.Where(u => u.ExamId == exam.Id).AnyAsync())
                    {
                        var papers = await paperRepo.Where(u => u.ExamId == exam.Id).ToListAsync();
                        papers.ForEach(u =>
                        {
                            u.Status = exam.Status;
                            u.IsDeleted = exam.IsDeleted;
                            u.UpdatedAt = DateTime.Now;
                            u.UpdatedBy = "修改/删除考试信息";
                        });
                        await paperRepo.UpdateAsync(papers);
                    }
                    var examRepo = fsql.Get(conn_str).GetRepository<Examination>();
                    
                    await examRepo.UpdateAsync(exam);
                    uow.Commit();
                    return true;
                }
                catch
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        public async Task<bool> DeleteExamInfo(Guid examId)
        {
            var exam = await fsql.Get(conn_str).Select<Examination>().Where(u => u.Id == examId).ToOneAsync();
            exam.IsDeleted = 1;
            exam.UpdatedBy = "删除考试信息";
            exam.UpdatedAt = DateTime.Now;
            return await UpdateExamInfo(exam);
        }

        public async Task<Guid[]> CloneExam(Guid examId, string adminId, string cloneExamName = "克隆", string clonePaperTitle = "")
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var examRepo = fsql.Get(conn_str).GetRepository<Examination>();
                    var orginExam = await examRepo.Where(u => u.Id == examId).ToOneAsync();
                    var cloneExam = new Examination()
                    {
                        Id = NewId.NextGuid(),
                        Title = cloneExamName,
                        AssociationId = orginExam.AssociationId,
                        AssociationTitle = orginExam.AssociationTitle,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedBy = adminId + "执行克隆操作",
                        UpdatedBy = adminId,
                        Description = cloneExamName,
                        BaseDuration = orginExam.BaseDuration,
                        StartTime = orginExam.StartTime,
                        EndTime = orginExam.EndTime,
                        Status = orginExam.Status,
                        BaseScore = orginExam.BaseScore,
                        ExamType = orginExam.ExamType,
                        ExtraInfo = orginExam.ExtraInfo,
                        IsDeleted = 0,
                        Remark = orginExam.Remark + $"(基本信息克隆自{orginExam.Id})",
                    };
                    await examRepo.InsertAsync(cloneExam);

                    var paperRepo = fsql.Get(conn_str).GetRepository<Paper>();
                    var orginPapers = await paperRepo.Where(u => u.ExamId == examId).ToListAsync();
                    List<Paper> papers = new List<Paper>();

                    int paperIndex = 1;
                    foreach (var org in orginPapers)
                    {
                        papers.Add(new Paper
                        {
                            Id = NewId.NextGuid(),
                            Title = $"{clonePaperTitle}卷{paperIndex}",
                            PaperType = org.PaperType,
                            PaperDegree = org.PaperDegree,
                            Duration = org.Duration,
                            QuestionDetailJson = org.QuestionDetailJson,
                            OpenResult = org.OpenResult,
                            ExamId = cloneExam.Id,
                            Score = org.Score,
                            Status = org.Status,
                            Remark = org.Remark + $"(克隆自试卷{org.Id})",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            CreatedBy = adminId + "执行克隆操作",
                            UpdatedBy = adminId,
                        });
                        paperIndex++;
                    }
                    var paperRet = await paperRepo.InsertAsync(papers);

                    uow.Commit();
                    return paperRet.Select(u => u.Id).ToArray();
                }
                catch
                {
                    uow.Rollback();
                    throw;
                }
            }
        }
    }
}
