using Essensoft.Paylink.Alipay.Parser;
using FreeSql.Internal.Model;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Dtos.Exam.UserAnswerRecord;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using Newtonsoft.Json;
using NPOI.POIFS.Dev;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class UserAnswerRecordViewRepo : ExaminationRepository<UserAnswerRecordView>, IUserAnswerRecordViewRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public UserAnswerRecordViewRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        /// <summary>
        /// 获取答题列表
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<UserAnswerSubmitRecordDto> GetUserRecord(PageDto dto, out long total)
        {
            var items = GetUserAnswerRecords(dto, out total);
            return items.Adapt<List<UserAnswerSubmitRecordDto>>();
        }

        /// <summary>
        /// 获取教师试卷列表
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<TeacherPapersDto> GetTeacherPapers(PageDto dto, out long total)
        {
            var items = GetUserAnswerRecords(dto, out total);
            var config = new TypeAdapterConfig();
            config.ForType<UserAnswerRecordView, TeacherPapersDto>()
                .Map(dest=>dest.IdNumber,src=>$"{src.IdNumber.Substring(0, 4)}**********{src.IdNumber.Substring(src.IdNumber.Length-2),2}");
           return items.Adapt<List<TeacherPapersDto>>(config);

        }

        private List<UserAnswerRecordView> GetUserAnswerRecords(PageDto dto, out long total)
        {
            if (string.IsNullOrEmpty(dto.whereJsonStr))
            {

                return fsql.Get(conn_str).Select<UserAnswerRecordView>()
                //.Where(u => u.IsDeleted == 0)
                .OrderByDescending(u => u.Id)
                .Count(out total)
                .Page(dto.pageindex, dto.pagesize)
                .ToList();
            }

            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(dto.whereJsonStr);
            return fsql.Get(conn_str).Select<UserAnswerRecordView>()
                //.Where(u => u.IsDeleted == 0)                
                .WhereDynamicFilter(dyfilter)
                .OrderByDescending(u => u.Id)
                .Count(out total)
                .Page(dto.pageindex, dto.pagesize)
                .ToList();
        }

        public async Task<List<UserAnswerRecordDto>> GetUserRecordForExport(string whereJsonStr)
        {
            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(whereJsonStr);
            return await fsql.Get(conn_str).Select<UserAnswerRecordView>()
                .Where(u => u.IsDeleted == 0)
                .WhereDynamicFilter(dyfilter)
                .OrderByDescending(u => u.Id)
                .ToListAsync(u => new UserAnswerRecordDto()
                {
                    urid = u.Id,
                    idNumber = u.IdNumber,
                    accountName = u.Name,
                    associationTitle = u.AssociationTitle,
                    examTitle = u.ExamTitle,
                    paperTitle = u.PaperTitle,
                    score = u.Score.ToString(),
                    objectScore=u.ObjectiveScore.ToString(),
                    complated = u.Complated == 0 ? "未交卷" : "已交卷"
                });
        }

        public async Task<dynamic> GetUserAnswerRecordApi(string[] reportIds,int? examType,int? isDeleted)
        {
            return await fsql.Get(conn_str).Select<UserAnswerRecordView>()
                .Where(u => reportIds.Contains(u.ReportId))
                .WhereIf(examType!=null,u=>u.ExamType==examType)
                .WhereIf(isDeleted != null, u => u.IsDeleted == isDeleted)
                .OrderByDescending(u => u.Id)
                .ToListAsync(u => new
                {
                    u.Id,
                    u.ReportNumber,
                    u.Name,
                    u.Score,
                    u.ExamId,
                    u.PaperId,
                    u.AccountId,
                    u.ObjectiveScore,
                    u.PaperScore,
                    u.SubmitAnswer,
                    u.Stage,
                    u.Complated,
                    u.CreatedAt,
                    u.UsedTime,
                    u.Marked,
                    u.IsDeleted,
                    u.ExamType,
                    u.ReportId
                });
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
                marked = record.Marked == 0 ? "待出分" : (record.Marked == 1?"客观分已出":"主客观均已出分"),
                answers = ret
            };
        }

        /// <summary>
        /// 移除答题记录
        /// </summary>
        /// <param name="urid"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveUserRecord(long urid, string adminId)
        {
            var userRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();

            var record = await userRepo
                .Where(u => u.Id == urid)
                .ToOneAsync();
            record.IsDeleted = 1;
            record.IdNumber += "(被移除)";
            //record.ReportId += "(被移除)";
            record.Remark += $"|被{adminId}移除记录";
            record.UpdatedAt = DateTime.Now;
            record.UpdatedBy = $"{adminId}移除";

            return await userRepo.UpdateAsync(record) == 1;
        }

        /// <summary>
        /// 强制交卷
        /// </summary>
        /// <param name="urid"></param>
        /// <returns></returns>
        [Obsolete("过期，客观题打分请统一使用ScoreObjectivePart方法")]
        public async Task<UserAnswerRecordView> ForceMarking(long urid)
        {
            //第一步：把提交的答案取出来
            var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecordView>();
            var record = await userAnswerRecordRepo.Where(u => u.Id == urid).ToOneAsync();
            if (record.Complated == 1)
            {
                return record;
            }
            double userObjectiveScore = 0;
            //如果是答卷空的，直接给0分,否则开始计算得分
            if (!string.IsNullOrEmpty(record.SubmitAnswer) && record.SubmitAnswer.Length > 2)
            {
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
            }
            record.Complated = (int)ExamComplated.Yes;
            record.ComplatedMode = (int)ExamComplatedMode.Force;
            record.Remark += $"客观题成绩为{userObjectiveScore}分(后台强制)";
            record.UpdatedAt = DateTime.Now;
            record.UpdatedBy = "systemmarked(manager force)";
            record.ObjectiveScore = userObjectiveScore;
            if (record.Marked == (int)ExamMarked.No)
                record.Score = userObjectiveScore;
            record.Marked = (int)ExamMarked.Part;
            await userAnswerRecordRepo.UpdateAsync(record);
            //增加一次答题记录
            var reportProcessRepo = fsql.Get(conn_str).GetRepository<ReportProcess>();
            long _reportId = Convert.ToInt64(record.ReportId);
            var process = await reportProcessRepo.Where(u => u.ReportId == _reportId).ToOneAsync();
            process.TestedTime = record.Stage;
            process.UpdatedAt = DateTime.Now;
            await reportProcessRepo.UpdateAsync(process);
            return record;

        }

        /// <summary>
        /// 添加题目记录
        /// </summary>
        /// <param name="urid"></param>
        /// <returns></returns>
        public async Task<bool> AddQuestionRecord(long urid)
        {
            try
            {
                var questionRecordRepo = fsql.Get(conn_str).GetRepository<QuestionRecord>();

                var userAnswerRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
                var record = await userAnswerRepo.Where(u => u.Id == urid).ToOneAsync();
                if (string.IsNullOrEmpty(record.SubmitAnswer) || record.SubmitAnswer.Length <= 2)
                {
                    return false;
                }
                var questionRecords = new List<QuestionRecord>();
                //取题目
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

                //取选项
                List<SubmitAnswerDto> Answers = JsonHelper.JsonDeserialize<List<SubmitAnswerDto>>(record.SubmitAnswer);
                var questionItemRepo = fsql.Get(conn_str).GetRepository<QuestionItem>();
                var userQuestionIds = Answers.Select(u => u.questionId).ToList();
                var questionItems = await questionItemRepo
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

                
                //遍历
                foreach (var answer in Answers)
                {
                    var questionRecord = new QuestionRecord()
                    {
                        RecordId = urid,
                        AccountId = record.AccountId
                    };
                    if (!relations.Where(u => u.QuestionId == answer.questionId).Any())
                    {
                        continue;
                    }
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
                        questionRecord.UserAnswerContent = questionItems.Where(u => u.Id == Convert.ToInt64(questionRecord.UserAnswerId)).First().Code;


                        var currItem = correctItems.Where(u => u.QuestionId == answer.questionId).First();
                        //且答案正确
                        if (answer.userAnswer.Length == 1 && (answer.userAnswer[0] == currItem.Id.ToString() || answer.userAnswer[0] == currItem.Code))
                        {
                            questionRecord.IsCorrect = 1;
                        }
                    }
                    //如果是多选
                    if (relation.SingleAnswer == 0)
                    {
                        questionRecord.IsCorrect = 2;
                        questionRecord.UserAnswerId = string.Join(",", answer.userAnswer);
                        bool sub_flag = true;
                        var currItems = correctItems.Where(u => u.QuestionId == answer.questionId).ToList();

                        if (answer.userAnswer.Length != currItems.Count)
                        {
                            questionRecord.IsCorrect = 2;
                            sub_flag = false;
                        }
                        int correctCnt = 0;
                        foreach (var currItem in currItems)
                        {
                            questionRecord.UserAnswerContent += questionItems.Where(u => u.Id == currItem.Id).First().Code;

                        }
                        if (correctCnt == currItems.Count && sub_flag)
                        {
                            questionRecord.IsCorrect = 1;
                        }
                    }

                    if (await questionRecordRepo.Where(u => u.RecordId == questionRecord.RecordId && u.QuestionId == questionRecord.QuestionId).AnyAsync())
                    {
                        await questionRecordRepo.Where(u => u.RecordId == questionRecord.RecordId && u.QuestionId == questionRecord.QuestionId)
                            .ToUpdate().Set(u => u.IsDeleted == 1).ExecuteAffrowsAsync();
                    }
                    questionRecords.Add(questionRecord);
                }

                
                await questionRecordRepo.InsertAsync(questionRecords);
                return true;
            }
            catch(Exception ex)
            {
                Assistant.Logger.Error(ex);
                return false;
            }
        }

        public async Task<List<long>> GetNotComplatedList(Guid examId)
        {
            //检索出宽松考试类型下的，未提交的，未删除的答题记录id
            return await fsql.Get(conn_str).Select<UserAnswerRecordView>()
                .Where(u => u.ExamId == examId && u.IsStrict == 0 && (u.Marked == 0 || u.Complated == 0 || (u.Score == 0 && u.ComplatedMode != 4)) && u.IsDeleted == 0)
                .ToListAsync(u => u.Id);
        }
    }
}
