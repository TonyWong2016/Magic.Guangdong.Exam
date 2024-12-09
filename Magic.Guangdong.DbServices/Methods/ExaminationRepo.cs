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
using Magic.Guangdong.DbServices.Dtos;
using NPOI.POIFS.Dev;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
using Magicodes.ExporterAndImporter.Core.Extension;
using EasyCaching.CSRedis;
using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.DbServices.Dtos.Exam.Examinations;

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
        /// <summary>
        /// 获取考试下拉列表，
        /// idType=0时，id代表活动id
        /// idType=1时，id代表考试id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<dynamic> GetExamMini(string id, int idType,int examType=-1)
        {
            return await fsql.Get(conn_str).Select<Examination>()
                .Where(u => u.IsDeleted == 0 && u.Status == ExamStatus.Enabled)
                .WhereIf(idType == 0 && !string.IsNullOrEmpty(id), u => u.AssociationId == id)
                .WhereIf(idType == 1 && !string.IsNullOrEmpty(id), u => u.Id == Guid.Parse(id))
                .WhereIf(examType >= 0, u => u.ExamType == (ExamType)examType)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(u => new
                {
                    value = u.Id,
                    text = u.Title,
                    score = u.BaseScore
                    //其余属性有必要在添加
                });
        }

        public async Task<List<ExaminationDropsDto>> GetExamDrops()
        {
            return await fsql.Get(conn_str).Select<Examination>()
                .Where(u => u.IsDeleted == 0 && u.Status == ExamStatus.Enabled)
                .ToListAsync(u => new ExaminationDropsDto()
                {
                    Id = u.Id,
                    Title = u.Title
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
                    paperRepo.UnitOfWork = uow;
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
                    examRepo.UnitOfWork = uow;
                    var oldExam = await examRepo.Where(u => u.Id == exam.Id).ToOneAsync();
                    exam.Expenses=oldExam.Expenses;//费用不可以改
                    if (exam.Quota < oldExam.Quota)//名额可以变大，不能变小
                        exam.Quota = oldExam.Quota;
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
            await fsql.Get(conn_str).Select<Paper>().Where(u => u.ExamId == examId)
                .ToUpdate()
                .Set(u => u.IsDeleted == 1)
                .Set(u => u.UpdatedAt == DateTime.Now)
                .Set(u => u.Remark == "考试信息被删除")
                .ExecuteAffrowsAsync();
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
                    examRepo.UnitOfWork = uow;
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
                        Address = orginExam.Address,
                        BaseDuration = orginExam.BaseDuration,
                        StartTime = orginExam.StartTime,
                        EndTime = orginExam.EndTime,
                        Status = orginExam.Status,
                        BaseScore = orginExam.BaseScore,
                        ExamType = orginExam.ExamType,
                        ExtraInfo = orginExam.ExtraInfo,
                        IsDeleted = 0,
                        Quota = orginExam.Quota,
                        Expenses = orginExam.Expenses,
                        Remark = orginExam.Remark + $"(基本信息克隆自{orginExam.Id})",
                        SchemeId = orginExam.SchemeId,
                        PageConfig = orginExam.PageConfig,
                        IndependentAccess = orginExam.IndependentAccess,
                        LoginRequired = orginExam.LoginRequired,
                    };
                    await examRepo.InsertAsync(cloneExam);

                    var paperRepo = fsql.Get(conn_str).GetRepository<Paper>();
                    paperRepo.UnitOfWork = uow;
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
                            IncludeSubjective = org.IncludeSubjective
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
