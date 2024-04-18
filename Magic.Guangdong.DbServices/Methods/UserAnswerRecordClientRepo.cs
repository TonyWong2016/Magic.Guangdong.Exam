using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var paperRepo = fsql.Get(conn_str).GetRepository<Paper>();
            if (!await paperRepo.Where(u => u.ExamId == dto.examId && u.Status == ExamStatus.Enabled && u.IsDeleted == 0).AnyAsync())
            {
                return new { code = -1, msg = "当前活动尚未创建任何有效试卷，暂时无法答题" };
            }
            var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
            var userAnswerRecordQuery = userAnswerRecordRepo
                .Where(u => u.ReportId == dto.reportId.ToString() &&
                u.ExamId == dto.examId &&
                u.Complated !=ExamComplated.Cancle &&
                u.IsDeleted == 0);

            if (await userAnswerRecordQuery.AnyAsync())
            {
                var record = await userAnswerRecordQuery.ToOneAsync();
                return new { code = 0, msg = "您已经抽过题，请直接进入答题或者查看成绩", data = record };
            }


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
                    AssociationTitle = b.AssociationTitle,
                    ExamTitle = b.Title,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    PaperTitle = a.Title,
                    PaperScore = a.Score,
                    Duration = (double)a.Duration,
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
                        OrderIndex = Convert.ToInt32(u.OrderIndex)
                    });
                question.Items = items.ToList();
            }

            paper.Questions = paperQuestions;

            //paper.ConfirmTime = DateTime.Now; 

            return paper;
        }
        // public async Task<>

        public async Task<UserAnswerRecordView> GetMyRecord(long urid)
        {
            return await fsql.Get(conn_str).Select<UserAnswerRecordView>()
                .Where(u => u.Id == urid)
                .ToOneAsync();
        }

        /// <summary>
        /// 获取我的答题记录
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
                    accountName = u.UserName,
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
                    isStrict = Convert.ToInt32(u.IsStrict)
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
                    await RedisHelper.HDelAsync("UserExamLog", dto.reportId);
                }

                record.ComplatedMode = (ExamComplatedMode)dto.complatedMode;
                record.Complated = (ExamComplated)(dto.complatedMode == 0 ? 0 : 1);
                record.UsedTime = dto.usedTime;
                //record.SubmitAnswer = string.IsNullOrWhiteSpace(dto.submitAnswerStr) ? "" : dto.submitAnswerStr;
                if (!string.IsNullOrEmpty(dto.submitAnswerStr) && dto.submitAnswerStr.Contains("userAnswer"))
                {
                    record.SubmitAnswer = dto.submitAnswerStr;
                }
                //插入答题提交记录
                var submitLogRepo = fsql.Get(conn_str).GetRepository<SubmitLog>();
                await submitLogRepo.InsertAsync(new SubmitLog()
                {
                    Urid = record.Id,
                    ComplatedMode = dto.complatedMode,
                    SubmitAnswer = dto.submitAnswerStr,//记录表里就如实记录提交的内容
                });

                await userAnswerRecordRepo.InsertOrUpdateAsync(record);

                //await RedisHelper.HDelAsync("UserExamLog", dto.applyId);
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
        public async Task<UserAnswerRecordView> Marking(long urid, bool submit = false)
        {
            //第一步：把提交的答案取出来
            var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecordView>();
            var record = await userAnswerRecordRepo.Where(u => u.Id == urid).ToOneAsync();
            if (record.Marked == 1 && record.Complated == 1 && !string.IsNullOrEmpty(record.SubmitAnswer))
            {
                return record;//已经给过分了
            }

            if (record.Complated == 0 && record.LimitedTime > DateTime.Now && !submit)
            {
                return record;//没交卷，答题也还没结束
            }
            //record.Stage = Convert.ToInt32(await userAnswerRecordRepo.Where(u => u.ApplyId == record.ApplyId && u.IsDeleted == 0).CountAsync());
            //严格模式下，要判定是否超时，允许2分钟以内的误差交卷时间
            //if(record.Complated==0 && record.LimitedTime < DateTime.Now)
            if (record.IsStrict == 1 && record.LimitedTime.AddMinutes(2) < DateTime.Now)
            {
                //没交卷，答题也结束了且超过了误差允许范围，先给个0分
                //record.Complated = 1;                
                record.Remark = "未在规定时间内交卷，给0分";
                record.Marked = 1;
                record.UpdatedAt = record.LimitedTime;
                record.UpdatedBy = "systemmarked";
                record.Score = 0;
                await userAnswerRecordRepo.UpdateAsync(record);
                return record;
            }
            double userScore = 0;

            //如果是答卷空的，看一下提交记录里有没有答案记录，如果没有那就确定是没提交，给0分,否则开始计算得分
            if (string.IsNullOrEmpty(record.SubmitAnswer))
            {
                var submitLogRepo = fsql.Get(conn_str).GetRepository<SubmitLog>();
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
                //第二步，把试卷的题目和分数取出来
                //var relationRepo = fsql.Get(conn_str).GetRepository<Relation>();
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
                //double paperScore = relations.Sum(u => u.ItemScore);

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

                //第四步，开始判分，客观题直接给，主观题撂着...
                foreach (var answer in Answers)
                {
                    if (!relations.Where(u => u.QuestionId == answer.questionId).Any())
                    {
                        continue;
                    }
                    var relation = relations.Where(u => u.QuestionId == answer.questionId).First();

                    //if (answer.userAnswer.Length==1 && answer.userAnswer[0]==)
                    if (relation.Objective != 1)
                    {
                        continue;//主观题，跳过
                    }
                    //如果是单选或者判断题
                    if (relation.SingleAnswer == 1)
                    {
                        var currItem = correctItems.Where(u => u.QuestionId == answer.questionId).First();
                        //且答案正确
                        if (answer.userAnswer.Length == 1 && (answer.userAnswer[0] == currItem.Id.ToString() || answer.userAnswer[0] == currItem.Code))
                        {
                            userScore += relation.ItemScore;//得分
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
                            userScore += relation.ItemScore;
                        }
                    }
                }
            }


            if (submit || (record.LimitedTime < DateTime.Now && record.IsStrict == 0))//强制交卷
            {
                record.Complated = 1;
                record.ComplatedMode = 2;
                record.Remark += "强制交卷;";
            }
            record.Remark += $"客观题成绩为{userScore}分";
            record.UpdatedAt = DateTime.Now;
            record.UpdatedBy = "systemmarked";
            record.Score = userScore;
            record.Marked = 1;

            await userAnswerRecordRepo.UpdateAsync(record);
            return record;

        }

        public async Task<int> ClearTestData()
        {
            return await fsql.Get(conn_str).Select<UserAnswerRecord>()
                .Where(u => u.UserName == "压测数据")
                .ToDelete()
                .ExecuteAffrowsAsync();
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
