using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class UserAnswerRecordRepo : ExaminationRepository<UserAnswerRecord>, IUserAnswerRecordRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public UserAnswerRecordRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }


        #region For ServerSide
        /// <summary>
        /// 获取当前有多少场考试
        /// </summary>
        /// <param name="associationId"></param>
        /// <returns></returns>
        public async Task<dynamic> GetExaminations(string associationId)
        {
            var examRepo = fsql.Get(conn_str).GetRepository<Examination>();
            if (!await examRepo.Where(u => u.AssociationId == associationId && u.IsDeleted == 0).AnyAsync())
            {
                return new { code = -1, msg = "当前活动尚未开启答题考试" };
            }
            return await examRepo.Where(u => u.AssociationId == associationId && u.IsDeleted == 0)
                .ToListAsync(u => new
                {
                    value = u.Id,
                    text = u.Title,
                    u.StartTime,
                    u.EndTime,
                    enabled = (u.StartTime < DateTime.Now && u.EndTime > DateTime.Now) ? 1 : 0 //enabled==1时选项才是可选的
                });
        }

        /// <summary>
        /// 抽考卷
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<dynamic> ConfirmMyPaper(UserPaperRecordDto dto)
        {
            var paperRepo = fsql.Get(conn_str).GetRepository<Paper>();
            if (!await paperRepo.Where(u => u.ExamId == dto.examId && u.IsDeleted == 0).AnyAsync())
            {
                return new { code = -1, msg = "当前活动尚未创建任何试卷，暂时无法答题" };
            }

            var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
            if (await userAnswerRecordRepo.Where(u => u.IdNumber == dto.idNumber && u.ReportId == dto.reportId && u.IsDeleted == 0).AnyAsync())
            {
                var record = await userAnswerRecordRepo.Where(u => u.IdNumber == dto.idNumber && u.ReportId == dto.reportId && u.IsDeleted == 0).ToOneAsync();
                return new { code = 0, msg = "您已经抽过题，请直接进入答题或者查看成绩", data = record };
            }
            //如果当前赛队有人答题了，且答题人不是提交的身份证号所属人，也给他返回。
            if (await userAnswerRecordRepo.Where(u => u.ReportId == dto.reportId && u.IsDeleted == 0).AnyAsync())
            {
                var record = await userAnswerRecordRepo.Where(u => u.ReportId == dto.reportId && u.IsDeleted == 0).ToOneAsync();
                return new { code = -1, msg = "当前赛队已经有其他人进行了答题，请和赛队中其他成员确认答题情况", data = record };
            }

            var papers = await paperRepo.Where(u => u.ExamId == dto.examId && u.Status == 0 && u.IsDeleted == 0)
                .ToListAsync(u => new
                {
                    u.Id,
                    u.Title,
                    u.Score,
                    u.Duration
                });

            int rd = new Random().Next(papers.Count);
            var myPaper = papers[rd];

            var finalRecord = await userAnswerRecordRepo.InsertAsync(new UserAnswerRecord()
            {
                IdNumber = dto.idNumber,
                ReportId = dto.reportId,
                //UserId = dto.userId,
                AccountId = dto.accountId,
                PaperId = myPaper.Id,
                ExamId = dto.examId,
                UserName = dto.userName,
                Remark = $"初始化答题，答题人识别码[{dto.idNumber}]；",
                CreatedBy = dto.accountId,
                CreatedAt = DateTime.Now
            });
            //初始化后，只要没有交卷，这里就会被锁定，防止被其他赛队成员抢答
            await RedisHelper.HSetAsync("UserExamLog", dto.reportId.ToString(), dto.idNumber);

            return new { code = 1, msg = "success", data = finalRecord };
        }

        /// <summary>
        /// 预览试卷
        /// </summary>
        /// <param name="paperId"></param>
        /// <returns></returns>
        public async Task<FinalPaperDto> GetMyPaper(Guid paperId)
        {
            //试卷基本信息
            var paper = await fsql.Get(conn_str).Select<Paper, Examination>()
                .LeftJoin((a, b) => a.ExamId == b.Id)
                .Where((a, b) => a.Id == paperId && a.IsDeleted == 0)
                .ToOneAsync((a, b) => new FinalPaperDto()
                {
                    PaperId = a.Id,
                    ExamId = b.Id,
                    AssociationId = b.AssociationId,
                    AssociationTitle = b.AssociationTitle,
                    ExamTitle = b.Title,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    PaperTitle = a.Title,
                    PaperScore = a.Score,
                    Duration = (double)a.Duration,
                    PaperType = (int)a.PaperType,

                });

            //试卷题目
            var paperQuestions = await fsql.Get(conn_str).Select<QuestionView, Relation>()
                .LeftJoin((a, b) => a.Id == b.QuestionId)
                .Where((a, b) => b.PaperId == paperId && a.IsDeleted == 0)
                .ToListAsync((a, b) => new PaperQuestionDto()
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
                    .Select(u => new PaperQuestionItemDto()
                    {
                        Id = u.Id,
                        Code = u.Code,
                        Description = u.Description,
                        DescriptionTxt = u.DescriptionText,
                        //IsAnswer = u.IsAnswer,
                        OrderIndex = u.OrderIndex == null ? 0 : 1
                    });
                question.Items = items.ToList();
            }

            paper.Questions = paperQuestions;

            return paper;
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
                    return new { code = -3, msg = "考试已经完成，不能再提交", data = record };//考试已经完成，不能再提交
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
                    string remark = $"交卷,答题人识别码[{dto.idNumber}]";
                    if (!record.Remark.Contains(remark))
                        record.Remark += remark;
                }

                record.ComplatedMode = (ExamComplatedMode)dto.complatedMode;
                record.Complated =(ExamComplated)(dto.complatedMode == 0 ? 0 : 1);
                //record.UsedTime = dto.usedTime;
                record.UsedTime = Math.Floor((DateTime.Now - record.CreatedAt).TotalSeconds);
                record.SubmitAnswer = string.IsNullOrWhiteSpace(dto.submitAnswerStr) ? "" : dto.submitAnswerStr;
                await userAnswerRecordRepo.InsertOrUpdateAsync(record);
                if (dto.complatedMode != (int)ExamComplatedMode.Auto)
                {
                    var reportProcessRepo = fsql.Get(conn_str).GetRepository<ReportProcess>();
                    long _reportId = Convert.ToInt64(dto.reportId);
                    var process = await reportProcessRepo.Where(u => u.ReportId == _reportId && u.ExamId==record.ExamId).ToOneAsync();
                    process.TestedTime = record.Stage;
                    process.UpdatedAt = DateTime.Now;
                    await reportProcessRepo.UpdateAsync(process);
                }
                await RedisHelper.HDelAsync("UserExamLog", dto.reportId.ToString());
                return new { code = 1, msg = "success", data = record };
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
        public async Task<UserAnswerRecord> Marking(long urid, bool submit = false)
        {
            using(var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    //第一步：把提交的答案取出来
                    var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
                    var questionRecordRepo = fsql.Get(conn_str).GetRepository<QuestionRecord>();
                    userAnswerRecordRepo.UnitOfWork = uow;
                    questionRecordRepo.UnitOfWork= uow;
                    
                    var record = await userAnswerRecordRepo.Where(u => u.Id == urid).ToOneAsync();

                    var questionRecord = new QuestionRecord()
                    {
                        RecordId = urid,
                        AccountId = record.AccountId,
                        
                    };
                    if (record.UpdatedBy == "systemmarked")
                    {
                        return record;//已经给过分了
                    }
                    if (record.Complated == 0 && record.LimitedTime > DateTime.Now && !submit)
                    {
                        return record;//尚未
                    }
                    List<SubmitAnswerDto> Answers = JsonHelper.JsonDeserialize<List<SubmitAnswerDto>>(record.SubmitAnswer);

                    //第二步，把试卷的题目和分数取出来
                    var relationRepo = fsql.Get(conn_str).GetRepository<Relation>();
                    var relations = await fsql.Get(conn_str).Select<Relation, QuestionView>()
                        .LeftJoin((a, b) => a.QuestionId == b.Id)
                        .Where((a, b) => a.PaperId == record.PaperId && a.IsDeleted == 0)
                        .ToListAsync((a, b) => new
                        {
                            a.QuestionId,
                            a.ItemScore,
                            b.Objective,
                            b.SingleAnswer
                        });
                    //double paperScore = relations.Sum(u => u.ItemScore);


                    //第三步，把用户提交的题目的正确答案都取出来
                    var questionItemRepo = fsql.Get(conn_str).GetRepository<QuestionItem>();
                    var userQuestionIds = Answers.Select(u => u.questionId).ToList();
                    var questionItems = await questionItemRepo
                        //.Where(u => u.IsDeleted == 0 && u.IsAnswer == 1 && userQuestionIds.Contains(u.QuestionId))
                        .Where(u => u.IsDeleted == 0 && userQuestionIds.Contains(u.QuestionId))
                        .ToListAsync(u => new
                        {
                            u.IsAnswer,
                            u.Id,
                            u.Description,
                            u.Code,
                            u.IsOption,
                            u.QuestionId
                        });
                    var correctItems = questionItems.Where(u => u.IsAnswer == 1);
                    //string lastQuestionId = "";

                    //第四步，开始判分，客观题直接给，主观题撂着...
                    double userObjectiveScore = 0;
                    foreach (var answer in Answers)
                    {
                        var relation = relations.Where(u => u.QuestionId == answer.questionId).First();
                        questionRecord.QuestionId = answer.questionId;
                        
                        //if (answer.userAnswer.Length==1 && answer.userAnswer[0]==)
                        if (relation.Objective != 1)
                        {
                            questionRecord.UserAnswerContent = string.Join("_EOF_", answer.userAnswer);
                            continue;//主观题，跳过
                        }
                        //如果是单选或者判断题
                        if (relation.SingleAnswer == 1)
                        {
                            questionRecord.IsCorrect = 2;
                            questionRecord.UserAnswerId = answer.userAnswer[0];
                            questionRecord.UserAnswerContent = questionItems.Where(u => u.QuestionId == Convert.ToInt64(questionRecord.UserAnswerId)).First().Code;
                            var currItem = correctItems.Where(u => u.Id == answer.questionId).First();
                            //且答案正确
                            if (answer.userAnswer.Length == 1 && (answer.userAnswer[0] == currItem.Id.ToString() || answer.userAnswer[0] == currItem.Code))
                            {
                                userObjectiveScore += relation.ItemScore;//得分
                                questionRecord.IsCorrect = 1;
                            }
                        }
                        //如果是多选
                        if (relation.SingleAnswer == 0)
                        {
                            var currItems = correctItems.Where(u => u.QuestionId == answer.questionId).ToList();
                            questionRecord.IsCorrect = 2;
                            questionRecord.UserAnswerId = string.Join(",", answer.userAnswer) ;
                            if (answer.userAnswer.Length != currItems.Count)
                            {
                                continue;//首先答案数量得一致，不一致就直接跳过
                            }
                            int correctCnt = 0;
                            foreach (var currItem in currItems)
                            {
                                foreach (var userAnswer in answer.userAnswer)
                                {
                                    questionRecord.UserAnswerContent += questionItems.Where(u => u.Id == currItem.Id).First().Code;
                                    if (userAnswer == currItem.Id.ToString() || userAnswer == currItem.Code)
                                    {
                                        correctCnt++;//这里判分的逻辑也可以改成判定数组是否包含正确答案，但实际上，字符串的包含判定还是比较耗资源的，两个循环看起来麻烦，实际相对contains的方式还是省了。
                                    }
                                }
                            }
                            if (correctCnt == currItems.Count)
                            {
                                userObjectiveScore += relation.ItemScore;
                                questionRecord.IsCorrect = 1;
                            }
                        }
                    }
                    if (submit)//强制交卷
                    {
                        record.Complated = ExamComplated.Yes;
                        record.ComplatedMode = ExamComplatedMode.Force;
                    }
                    record.Remark += $"客观题成绩为{userObjectiveScore}分";
                    record.UpdatedAt = DateTime.Now;
                    record.UpdatedBy = "systemmarked";
                    record.Marked = ExamMarked.Part;
                    record.ObjectiveScore = userObjectiveScore;
                    await userAnswerRecordRepo.UpdateAsync(record);
                    await questionRecordRepo.InsertAsync(questionRecord);
                    uow.Commit();
                    return record;
                }
                catch(Exception ex)
                {
                    Assistant.Logger.Error(ex);
                    uow.Rollback();
                    throw;
                }
            }
            

        }
        
        #endregion

        #region For ClientSide

        #endregion
    }
}
