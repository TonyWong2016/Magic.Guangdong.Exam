using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class UserAnswerRecordClientRepo : ExaminationRepository<UserAnswerRecord>, IUserAnswerRecordClientRepo
    {
        private IdleBus<IFreeSql> fsql;
        
        const string conn_str = "db_exam";
        public UserAnswerRecordClientRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<UserAnswerRecordView> GetUserRecordDetail(string idNumber, string reportId, Guid examId)
        {
            return await fsql.Get(conn_str).Select<UserAnswerRecordView>()
                .Where(u => u.IsDeleted == 0 && u.ExamId == examId)
                .WhereIf(!string.IsNullOrEmpty(idNumber), u => u.IdNumber == idNumber)
                .WhereIf(!string.IsNullOrEmpty(reportId), u => u.ReportId == reportId)
                .ToOneAsync();
        }

        public async Task<UserAnswerRecordView> GetUserRecordDetailById(long id)
        {
            return await fsql.Get(conn_str).Select<UserAnswerRecordView>()
                .Where(u => u.IsDeleted == 0 && u.Id == id)
                .ToOneAsync();
        }

        /// <summary>
        /// 获取当前有多少场考试
        /// </summary>
        /// <param name="associationId"></param>
        /// <returns></returns>
        [Obsolete("请使用GetReportExamsForClient")]
        public async Task<List<SelectExaminationsDto>> GetExaminations(string associationId, string examId = "", string groupCode = "")
        {
            var examRepo = fsql.Get(conn_str).GetRepository<Examination>();
            if (!await examRepo.Where(u => u.AssociationId == associationId && u.Status == ExamStatus.Enabled && u.IsDeleted == 0).AnyAsync())
            {
                //return new { code = -1, msg = "当前活动尚未开启任何有效答题考试" };
                return null;
            }
            return await examRepo.Where(u => u.AssociationId == associationId && u.IsDeleted == 0)
                .WhereIf(!string.IsNullOrEmpty(examId), u => u.Id == Guid.Parse(examId))
                .WhereIf(!string.IsNullOrEmpty(groupCode), u => u.GroupCode == groupCode)
                .OrderBy(u => u.GroupCode)
                .OrderBy(u => u.OrderIndex)
                //.ToListAsync(u => new
                //{
                //    value = u.Id,
                //    text = u.Title,
                //    startTime = u.StartTime.ToString("yyyy/MM/dd HH:mm"),
                //    endTime = u.EndTime.ToString("yyyy/MM/dd HH:mm"),
                //    enabled = (u.StartTime < DateTime.Now && u.EndTime > DateTime.Now) ? 1 : 0, //enabled==1时选项才是可选的

                //});
                .ToListAsync(u => new SelectExaminationsDto
                {
                    value = u.Id,
                    text = u.Title,
                    startTime = u.StartTime.ToString("yyyy/MM/dd HH:mm"),
                    endTime = u.EndTime.ToString("yyyy/MM/dd HH:mm"),
                    enabled = (u.StartTime < DateTime.Now && u.EndTime > DateTime.Now) ? 1 : 0, //enabled==1时选项才是可选的

                });
        }



        /// <summary>
        /// 抽考卷
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<dynamic> ConfirmMyPaper(ConfirmPaperDto dto)
        {
            using(var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var examRepo = fsql.Get(conn_str).GetRepository<Examination>();
                    var exam = await examRepo.Where(u => u.Id == dto.examId).FirstAsync();

                    if (exam.ExamType == ExamType.Practice)
                    {
                        return ConfirmMyPracticePaper(dto);
                    }

                    var paperRepo = fsql.Get(conn_str).GetRepository<Paper>();
                    if (!await paperRepo.Where(u => u.ExamId == dto.examId && u.Status == ExamStatus.Enabled && u.IsDeleted == 0).AnyAsync())
                    {
                        return new { code = -1, msg = "当前活动尚未创建任何有效试卷，暂时无法答题" };
                    }
                    var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
                    var userAnswerRecordQuery = userAnswerRecordRepo
                        .Where(u => u.ReportId == dto.reportId &&
                        u.ExamId == dto.examId &&
                        u.Complated != ExamComplated.Cancle &&
                        u.IsDeleted == 0);

                    if (await userAnswerRecordQuery.AnyAsync())
                    {
                        var record = await userAnswerRecordQuery.ToOneAsync();
                        return new { code = 1, msg = "您已经抽过题，请直接进入答题或者查看成绩", data = record };
                    }

                    var papers = await paperRepo
                        .Where(u => u.ExamId == dto.examId)
                        .Where(u => u.Status == ExamStatus.Enabled)
                        .Where(u => u.IsDeleted == 0)
                        .ToListAsync(u => new
                        {
                            u.Id,
                            u.Title,
                            u.Score,
                            u.Duration
                        });
                    int rd = new Random().Next(papers.Count);
                    var myPaper = papers[rd];


                    var reportInfoRepo = fsql.Get(conn_str).GetRepository<ReportInfo>();
                    var reportInfo = await reportInfoRepo.Where(u => u.Id == dto.reportId).ToOneAsync();
                    DateTime limitedTime = DateTimeOffset.UtcNow.AddMinutes(myPaper.Duration).LocalDateTime;
                    //Logger.Debug("试卷时间：" + myPaper.Duration);
                    //Logger.Debug("限定时间：" + limitedTime);
                    var finalRecord = await userAnswerRecordRepo.InsertAsync(new UserAnswerRecord()
                    {
                        Id = YitIdHelper.NextId(),
                        IdNumber = reportInfo.ReportNumber,
                        ReportId = dto.reportId,
                        AccountId = reportInfo.AccountId,
                        UserName = reportInfo.Name,
                        ExamId = dto.examId,
                        Remark = $"初始化答题，答题人证件号[{reportInfo.IdCard}]；",
                        CreatedBy = reportInfo.AccountId,
                        CreatedAt = DateTimeOffset.UtcNow.LocalDateTime,
                        LimitedTime = DateTimeOffset.UtcNow.AddMinutes(myPaper.Duration).LocalDateTime,
                        PaperId = myPaper.Id
                    });
                    uow.Commit();
                    await RedisHelper.HSetAsync("GDExamLog", dto.reportId.ToString(), reportInfo.ReportNumber);
                    await RedisHelper.ExpireAsync("GDExamLog", Convert.ToInt32(myPaper.Duration) * 60);
                    return new { code = 0, msg = "success", data = finalRecord };
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    Logger.Error(ex.Message);
                    return new { code = -1, msg = "抽卷失败" };

                }
            }
            
        }

        /// <summary>
        /// 抽练习题
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<dynamic> ConfirmMyPracticePaper(ConfirmPaperDto dto)
        {

            var examRepo = fsql.Get(conn_str).GetRepository<Examination>();
            var exam = await examRepo.Where(u => u.Id == dto.examId).FirstAsync();
            if (exam.ExamType == ExamType.Examination)
            {
                //这个就不跳了，肯定是业务出问题了
                return new { code = -1, msg = "非练习模式，不可以抽题" };
            }

            var paperRepo = fsql.Get(conn_str).GetRepository<Paper>();
            if (!await paperRepo.Where(u => u.ExamId == dto.examId && u.Status == ExamStatus.Enabled && u.IsDeleted == 0).AnyAsync())
            {
                return new { code = -1, msg = "当前活动尚未创建任何有效试卷，暂时无法答题" };
            }
            var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
            var userAnswerRecordQuery = userAnswerRecordRepo
                .Where(u => u.ReportId == dto.reportId &&
                u.ExamId == dto.examId &&
                u.IsDeleted == 0);

            if (await userAnswerRecordQuery.Where(u=>u.Complated == ExamComplated.No).AnyAsync())
            {
                var record = await userAnswerRecordQuery.Where(u => u.Complated == ExamComplated.No).ToOneAsync();
                return new { code = 1, msg = "存在尚未提交的练习，请先提交后在重新刷题", data = record };
            }

           
            var papers = await paperRepo
                .Where(u => u.ExamId == dto.examId)
                .Where(u => u.Status == ExamStatus.Enabled)
                .Where(u => u.IsDeleted == 0)
                .ToListAsync(u => new
                {
                    u.Id,
                    u.Title,
                    u.Score,
                    u.Duration
                });
            int rd = new Random().Next(papers.Count);
            var myPaper = papers[rd];


            var reportInfoRepo = fsql.Get(conn_str).GetRepository<ReportInfo>();
            var reportInfo = await reportInfoRepo.Where(u => u.Id == dto.reportId).ToOneAsync();
            DateTime limitedTime = DateTimeOffset.UtcNow.AddMinutes(myPaper.Duration).LocalDateTime;
            int stage = Convert.ToInt32(await userAnswerRecordRepo.Where(u => u.ExamId == dto.examId && u.ReportId == dto.reportId).CountAsync());
            var finalRecord = await userAnswerRecordRepo.InsertAsync(new UserAnswerRecord()
            {
                Id = YitIdHelper.NextId(),
                IdNumber = reportInfo.ReportNumber,
                ReportId = dto.reportId,
                AccountId = reportInfo.AccountId,
                UserName = reportInfo.Name,
                ExamId = dto.examId,
                Remark = $"初始化刷题，刷题人证件号[{reportInfo.IdCard}]；",
                CreatedBy = reportInfo.AccountId,
                CreatedAt = DateTimeOffset.UtcNow.LocalDateTime,
                LimitedTime = DateTimeOffset.UtcNow.AddMinutes(myPaper.Duration).LocalDateTime,
                PaperId = myPaper.Id,
                Stage = stage//刷题模式，这个要给
            });

            //await RedisHelper.HSetAsync("GDPracticeLog", dto.reportId.ToString(), reportInfo.ReportNumber);
            return new { code = 0, msg = "success", data = finalRecord };
        }

        /// <summary>
        /// 预览试卷
        /// </summary>
        /// <param name="paperId"></param>
        /// <returns></returns>
        public async Task<FinalPaperClientDto> GetMyPaper(Guid paperId)
        {
            //试卷基本信息
            var paper = await fsql.Get(conn_str).Select<Paper, Examination>()
                .LeftJoin((a, b) => a.ExamId == b.Id)
                .Where((a, b) => a.Id == paperId && a.IsDeleted == 0)
                .ToOneAsync((a, b) => new FinalPaperClientDto()
                {
                    PaperId = a.Id,
                    AssociationTitle = b.AssociationTitle,
                    ExamTitle = b.Title,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    PaperTitle = a.Title,
                    PaperScore = a.Score,
                    Duration = a.Duration,
                    PaperType = (int)a.PaperType,
                    CreatedAt = a.CreatedAt
                });

            //试卷题目
            var paperQuestions = await fsql.Get(conn_str).Select<QuestionView, Relation>()
                .AsTable((type, oldname) =>
                {
                    if (type.Name == "Relation" && paper.CreatedAt.Year < DateTime.Now.Year)
                        return $"Relation_{paper.CreatedAt.Year}";
                    return null;
                })
                .LeftJoin((a, b) => a.Id == b.QuestionId)
                .Where((a, b) => b.PaperId == paperId && a.IsDeleted == 0)
                .ToListAsync((a, b) => new PaperQuestionClientDto()
                {
                    Id = a.Id,
                    IsObjective = a.Objective,
                    SubjectName = a.SubjectName,
                    TypeName = a.TypeName,
                    Degree = a.Degree,
                    ItemScore = b.ItemScore,
                    // IsOpen = a.IsOpen,
                    Title = a.Title,
                    SingleAnswer = a.SingleAnswer,
                    //Analysis = a.Analysis,
                });

            var questionIds = paperQuestions.Select(u => u.Id).ToList();

            //题目选项
            var questionItems = new List<QuestionItem>();
            fsql.Get(conn_str).Select<QuestionItem>()
                .Where(u => questionIds.Contains(u.QuestionId) && u.IsDeleted == 0)
                .ToChunk(100, done =>
                {
                    questionItems.AddRange(done.Object);
                });

            //组合题目和选项
            foreach (var question in paperQuestions)
            {
                var items = questionItems
                    .Where(u => u.QuestionId == question.Id)
                    .OrderBy(u => u.OrderIndex)
                    .Select(u => new PaperQuestionItemClientDto()
                    {
                        Id = u.Id,
                        Code = u.Code,
                        Description = u.Description,
                        DescriptionTxt = u.DescriptionText,
                        //IsAnswer = u.IsAnswer,
                        OrderIndex = Convert.ToInt32(u.OrderIndex)
                    });
                question.Items = items.ToList();
            }

            paper.Questions = paperQuestions;
            return paper;
        }

        public async Task<UserAnswerRecordView> GetMyRecord(long urid)
        {
            return await fsql.Get(conn_str).Select<UserAnswerRecordView>()
                .Where(u => u.Id == urid)
                .ToOneAsync();
        }

        /// <summary>
        /// 获取用户答案
        /// </summary>
        /// <param name="urid"></param>
        /// <returns></returns>
        public async Task<dynamic> GetUserAnswer(long urid)
        {
            var recordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecordView>();
            var record = await recordRepo.Where(u => u.Id == urid).ToOneAsync();
            List<AnswerDto> ret = new List<AnswerDto>();
            if (!string.IsNullOrEmpty(record.SubmitAnswer))
            {
                List<AnswerDto> answerList = JsonHelper.JsonDeserialize<List<AnswerDto>>(record.SubmitAnswer);
                var questionRepo = fsql.Get(conn_str).GetRepository<QuestionView>();
                var itemRepo = fsql.Get(conn_str).GetRepository<QuestionItem>();
                var questionIds = answerList.Select(u => u.questionId);
                var userSubmitQuestions = await questionRepo.Where(u => questionIds.Contains(u.Id)).ToListAsync();
                List<string> listAnswer = new List<string>();
                foreach (var userQuestion in userSubmitQuestions)
                {
                    var userAnswer = answerList.Where(u => u.questionId == userQuestion.Id).First();
                    //如果是客观题，直接返回选项代码
                    if (userQuestion.Objective == 1)
                    {
                        var userItems = await itemRepo
                            .Where(u => userAnswer.userAnswer.Contains(u.Id.ToString()))
                            .ToListAsync(u => u.Code);
                        ret.Add(new AnswerDto()
                        {
                            questionId = userQuestion.Id,
                            userAnswer = userItems.ToArray()
                        });
                    }
                    //如果是主观题，直接返回用户填写的答案
                    else
                    {
                        ret.Add(userAnswer);
                    }
                }
            }


            return new
            {
                userName = record.Name,
                record.IdNumber,
                record.Score,
                record.ObjectiveScore,
                record.ReportId,
                record.UpdatedAt,
                record.LimitedTime,
                record.UpdatedBy,
                record.ComplatedMode,
                record.CreatedAt,
                complated = record.Complated == 1 ? "已交卷" : "未交卷",
                marked = record.Marked == 1 ? "已出成绩" : "未出成绩",
                answers = ret
            };
        }

        /// <summary>
        /// 获取我的答题记录
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<ExamRecordDto>> GetMyReportExamRecords(string reportId)
        {
            return await fsql.Get(conn_str).Select<UserAnswerRecordView>()
                .Where(u => u.ReportId == reportId)
                .OrderBy(u => u.IsDeleted)
                .OrderBy(u => u.Complated)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(u => new ExamRecordDto()
                {
                    recordId = u.Id,
                    score = u.Score,
                    
                    isComplated = u.Complated,
                    accountName = u.Name,
                    examId = u.ExamId,
                    paperId = u.PaperId,
                    examTitle = u.ExamTitle,
                    paperTitle = u.PaperTitle,
                    idNumber = u.IdNumber,
                    CreatedAt = u.CreatedAt,
                    LimitedAt = u.LimitedTime,
                    submitAnswer = u.SubmitAnswer,
                    associationId = u.AssociationId,
                    openResult = Convert.ToInt32( u.OpenResult),
                    isDeleted = u.IsDeleted,
                    isStrict = Convert.ToInt32(u.IsStrict),
                    testedTime = u.Stage,
                    examType = u.ExamType
                });
        }

        /// <summary>
        /// 获取我的账号答题记录
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<ExamRecordDto>> GetMyAccountExamRecords(string accountId)
        {
            return await fsql.Get(conn_str).Select<UserAnswerRecordView>()
                .Where(u => u.AccountId == accountId)
                .OrderBy(u => u.IsDeleted)
                .OrderBy(u => u.Complated)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(u => new ExamRecordDto()
                {
                    recordId = u.Id,
                    score = u.Score,
                    isComplated = u.Complated,
                    accountName = u.Name,
                    examId = u.ExamId,
                    paperId = u.PaperId,
                    examTitle = u.ExamTitle,
                    paperTitle = u.PaperTitle,
                    idNumber = u.IdNumber,
                    CreatedAt = u.CreatedAt,
                    LimitedAt = u.LimitedTime,
                    submitAnswer = u.SubmitAnswer,
                    associationId = u.AssociationId,
                    openResult = Convert.ToInt32(u.OpenResult),
                    isDeleted = u.IsDeleted,
                    isStrict = Convert.ToInt32(u.IsStrict),
                    testedTime = u.Stage,
                    examType = u.ExamType
                });
        }
        /// <summary>
        /// 交卷（这是提交一整张试卷）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<dynamic> SubmitMyPaper(SubmitMyAnswerDto dto)
        {
            try
            {
                var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
                if (!await userAnswerRecordRepo.Where(u => u.Id == dto.recordId).AnyAsync())
                {
                    return new { code = -1, msg = "答题记录不存在，请联系管理人员" };//记录不存在
                }
                var record = await userAnswerRecordRepo.Where(u => u.Id == dto.recordId).ToOneAsync();

                if (record.IdNumber != dto.idNumber || record.ReportId != dto.reportId)
                {
                    return new { code = -2, msg = "提交记录和之前初始化时的信息不一致，请联系管理人员" };//提交记录和之前初始化时的信息不一致，这种情况常规也不会出现，就是避免一些特殊情况，比如后台偷偷给人家该信息，暗箱操作，但如果做的太天衣无缝也没招。。
                }

                if (record.Complated == ExamComplated.Yes)
                {
                    return new { code = -3, msg = "测评已正常提交并完成，不用再次提交", data = record };//考试已经完成，不能再提交
                }
                //不管是不是严格模式，超时就不能提交答案了
                if (record.LimitedTime.AddMinutes(2) < DateTime.Now)
                {
                    return new { code = -3, msg = "考试已结束，您的答题时间超时，无法提交，请确认答题过程中是否有较长时间的息屏。", data = record };
                }
                //record.UpdatedBy = dto.userId.ToString();
                //record.CheatCnt = dto.cheatCnt;//作弊的判定单独有接口做更新
                record.UpdatedAt = DateTime.Now;

                //string remark = $"#保存草稿,答题人识别码[{dto.idNumber}]";
                if (dto.complatedMode == 0)
                {
                    record.UpdatedBy = $"保存草稿,[{dto.idNumber}]";
                }
                else
                {
                    record.UpdatedBy = dto.userId.ToString();
                    record.Remark += $"交卷,答题人识别码[{dto.idNumber}]";
                    await RedisHelper.HDelAsync("GDExamLog", dto.reportId.ToString());

                }

                record.ComplatedMode = (ExamComplatedMode)dto.complatedMode;
                record.Complated = (ExamComplated)(dto.complatedMode == 4 ? 0 : 1);
                //record.UsedTime = dto.usedTime;
                record.UsedTime = Math.Floor((DateTime.Now - record.CreatedAt).TotalSeconds);

                //record.SubmitAnswer = string.IsNullOrWhiteSpace(dto.submitAnswerStr) ? "" : dto.submitAnswerStr;
                if (!string.IsNullOrEmpty(dto.submitAnswerStr) && dto.submitAnswerStr.Contains("userAnswer"))
                {
                    record.SubmitAnswer = dto.submitAnswerStr;
                }
                //插入答题提交记录
                var submitLogRepo = fsql.Get(conn_str).GetRepository<UserAnswerSubmitLog>();
                await submitLogRepo.InsertAsync(new UserAnswerSubmitLog()
                {
                    Urid = record.Id,
                    ComplatedMode = dto.complatedMode,
                    SubmitAnswer = dto.submitAnswerStr,//记录表里就如实记录提交的内容
                });

                
                if (dto.complatedMode != (int)ExamComplatedMode.Auto)
                {
                    //int stage = Convert.ToInt32(await userAnswerRecordRepo.Where(u => u.ReportId == record.ReportId && u.IsDeleted == 0).CountAsync());
                    //record.Stage = stage + 1;

                    var reportProcessRepo = fsql.Get(conn_str).GetRepository<ReportProcess>();
                    long _reportId = Convert.ToInt64(dto.reportId);
                    var process = await reportProcessRepo.Where(u => u.ReportId == _reportId && u.ExamId==record.ExamId).ToOneAsync();
                    process.TestedTime = record.Stage;
                    process.UpdatedAt = DateTime.Now;
                    await reportProcessRepo.UpdateAsync(process);
                }

                await userAnswerRecordRepo.InsertOrUpdateAsync(record);
                //await RedisHelper.HDelAsync("UserExamLog", dto.applyId);
                return new { code = 0, msg = "success", data = record };
            }
            catch
            {
                throw;
            }
        }



        /// <summary>
        /// 打分
        /// </summary>
        /// <param name="idNumber"></param>
        /// <returns></returns>
        public async Task<UserAnswerRecordView> Marking(long urid, bool submit = false)
        {
            //第一步：把提交的答案取出来
            var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecordView>();
            var record = await userAnswerRecordRepo.Where(u => u.Id == urid).ToOneAsync();
            if (record.Marked != (int)ExamMarked.No && record.Complated == (int)ExamComplated.Yes && !string.IsNullOrEmpty(record.SubmitAnswer))
            {
                return record;//已经给过分了
            }

            if (record.Complated == (int)ExamComplated.No && record.LimitedTime > DateTime.Now && !submit)
            {
                return record;//没交卷，答题也还没结束
            }
            //严格模式下，要判定是否超时，允许2分钟以内的误差交卷时间
            if (record.IsStrict == 1 && record.LimitedTime.AddMinutes(2) < DateTime.Now)
            {
                //没交卷，答题也结束了且超过了误差允许范围，先给个0分
                record.Complated = (int)ExamComplated.Yes;
                record.ComplatedMode = (int)ExamComplatedMode.Timeup;
                record.Remark = "未在规定时间内交卷，给0分";
                record.Marked = (int)ExamMarked.All;
                record.UpdatedAt = record.LimitedTime;
                record.UpdatedBy = "systemmarked";
                record.Score = 0;
                await userAnswerRecordRepo.UpdateAsync(record);
                return record;
            }
            double userObjectiveScore = 0;

            //如果是答卷空的，看一下提交记录里有没有答案记录，如果没有那就确定是没提交，给0分,否则开始计算得分
            if (string.IsNullOrEmpty(record.SubmitAnswer))
            {
                var submitLogRepo = fsql.Get(conn_str).GetRepository<UserAnswerSubmitLog>();
                if (await submitLogRepo.Where(u => u.Urid == record.Id && u.SubmitAnswer.Contains("userAnswer")).AnyAsync())
                {
                    var submitLog = await submitLogRepo
                        .Where(u => u.Urid == record.Id && u.SubmitAnswer.Contains("userAnswer"))
                        .OrderByDescending(u => u.Id)
                        .ToOneAsync();
                    record.SubmitAnswer = submitLog.SubmitAnswer;
                    record.Remark += "交卷异常，取草稿中最近且正常的一次记录;";
                }
                else
                    record.SubmitAnswer = "[]";//的确交了白卷
            }
            
            //这里的判定条件也要留着，因为经过上面的处理，并不一定能保证一定有答题记录，有可能就是没有提交任何记录。
            if (record.SubmitAnswer.Contains("userAnswer"))
            {
                List<SubmitAnswerDto> Answers = JsonHelper.JsonDeserialize<List<SubmitAnswerDto>>(record.SubmitAnswer);
                //第二步，把试卷的题目和分数取出来，注意这里有一个取分表数据的策略
                var relations = await fsql.Get(conn_str).Select<Relation, QuestionView>()
                   .AsTable((type, oldname) =>
                   {
                       if (type.Name == "Relation" && record.CreatedAt.Year < DateTime.Now.Year)
                           return $"Relation_{record.CreatedAt.Year}";
                       return null;
                   })
                    .LeftJoin((a, b) => a.QuestionId == b.Id)
                    .Where((a, b) => a.PaperId == record.PaperId && a.IsDeleted == 0)
                    .ToListAsync((a, b) => new
                    {
                        a.QuestionId,
                        a.ItemScore,
                        b.Objective,
                        b.SingleAnswer
                    });

                //第三步，把用户提交的题目的正确答案都取出来
                var questionItemRepo = fsql.Get(conn_str).GetRepository<QuestionItem>();
                var userQuestionIds = Answers.Select(u => u.questionId).ToList();
                var correctItems = await questionItemRepo
                    .Where(u => u.IsDeleted == 0 && u.IsAnswer == 1 && userQuestionIds.Contains(u.QuestionId))
                    .ToListAsync(u => new
                    {
                        u.IsAnswer,
                        u.Id,
                        u.Description,
                        u.Code,
                        u.IsOption,
                        u.QuestionId
                    });
                //string lastQuestionId = "";
                var userSubmitAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerSubmitRecord>();
                List<UserAnswerSubmitRecord> userSubmitAnswerRecords = new List<UserAnswerSubmitRecord>();
                //第四步，开始判分，客观题直接给，主观题撂着...
                foreach (var answer in Answers)
                {                   
                    if (!relations.Where(u => u.QuestionId == answer.questionId).Any())
                    {
                        continue;
                    }
                    var relation = relations.Where(u => u.QuestionId == answer.questionId).First();
                    
                    //把答题记录存到单独的表里
                    var submitAnswerRecord = new UserAnswerSubmitRecord();
                    submitAnswerRecord.QuestionId = answer.questionId;
                    submitAnswerRecord.RecordId = record.Id;
                    if (relation.Objective != 1)
                    {
                        submitAnswerRecord.IsObjective = 1;
                        submitAnswerRecord.ObjectiveAnswer = JsonHelper.JsonSerialize(answer.userAnswer);
                        submitAnswerRecord.Remark = "主观题";
                        userSubmitAnswerRecords.Add(submitAnswerRecord);
                        continue;//主观题，跳过
                    }
                    submitAnswerRecord.SubjectiveAnswer = JsonHelper.JsonSerialize(answer.userAnswer);
                    submitAnswerRecord.Remark = "客观题";
                    userSubmitAnswerRecords.Add(submitAnswerRecord);
                    //如果是单选或者判断题
                    if (relation.SingleAnswer == 1)
                    {
                        var currItem = correctItems.Where(u => u.QuestionId == answer.questionId).First();
                        //且答案正确
                        if (answer.userAnswer.Length == 1 && (answer.userAnswer[0] == currItem.Id.ToString() || answer.userAnswer[0] == currItem.Code))
                        {
                            userObjectiveScore += relation.ItemScore;//得分
                        }
                    }
                    //如果是多选
                    if (relation.SingleAnswer == 0)
                    {
                        var currItems = correctItems.Where(u => u.QuestionId == answer.questionId).ToList();
                        if (answer.userAnswer.Length != currItems.Count)
                        {
                            continue;//首先答案数量得一致，不一致就直接跳过
                        }
                        int correctCnt = 0;
                        foreach (var currItem in currItems)
                        {
                            foreach (var userAnswer in answer.userAnswer)
                            {
                                if (userAnswer == currItem.Id.ToString() || userAnswer == currItem.Code)
                                {
                                    correctCnt++;//这里判分的逻辑也可以改成判定数组是否包含正确答案，但实际上，字符串的包含判定还是比较耗资源的，两个循环看起来麻烦，实际相对contains的方式还是省了。
                                }
                            }
                        }
                        if (correctCnt == currItems.Count)
                        {
                            userObjectiveScore += relation.ItemScore;
                        }
                    }
                }
                await userSubmitAnswerRecordRepo.InsertAsync(userSubmitAnswerRecords);
            }
           

            if (submit || (record.LimitedTime < DateTime.Now && record.IsStrict == 0))//强制交卷
            {
                record.Complated = (int)ExamComplated.Yes;
                record.ComplatedMode = submit ? (int)ExamComplatedMode.Force : (int)ExamComplatedMode.Timeup;
                record.Remark += "强制交卷;";
            }
            record.Remark += $"客观题成绩为{userObjectiveScore}分";
            record.UpdatedAt = DateTime.Now;
            record.UpdatedBy = "systemmarked";
            
            record.ObjectiveScore = userObjectiveScore;
            if (record.Marked == (int)ExamMarked.No)
                record.Score = userObjectiveScore;
            record.Marked = (int)ExamMarked.Part;
            
            await userAnswerRecordRepo.UpdateAsync(record);
            return record;
        }
    }

    public class SelectExaminationsDto
    {
        public Guid value { get; set; }

        public string text { get; set; }

        public string startTime { get; set; }

        public string endTime { get; set; }

        public int enabled { get; set; }
    }
}
