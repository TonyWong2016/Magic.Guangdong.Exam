using FreeSql.Internal.Model;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Newtonsoft.Json;
using SixLabors.ImageSharp.Metadata.Profiles.Iptc;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class PaperRepo : ExaminationRepository<Paper>, IPaperRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public PaperRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<dynamic> GetPaperMini(Guid? examId)
        {

            return await fsql.Get(conn_str).Select<Paper>()
                .Where(u => u.IsDeleted == 0)
                .WhereIf(examId != null, u => u.ExamId == examId)
                .ToListAsync(u => new
                {
                    value = u.Id,
                    text = u.Title,
                    //u.OpenResult,
                    //u.Duration
                });
        }

        public dynamic GetPaperList(PageDto pageDto, out long total)
        {
            List<Guid> paperIds = new List<Guid>();
            bool tagAdd = false;
            if (pageDto.tagId > 0)
            {
                tagAdd = true;
                var tagRelations = fsql.Get(conn_str).Select<TagRelations>()
                   .Where(u => u.TagId == pageDto.tagId && u.TableName == "Paper")
                   .ToList(u => new
                   {
                       paperId = Guid.Parse(u.AssociationId)
                   });

                if (tagRelations.Count > 0)
                {
                    paperIds = tagRelations.Select(u => u.paperId).ToList();
                    
                }
            }
            
            if (string.IsNullOrWhiteSpace(pageDto.whereJsonStr))
                return fsql.Get(conn_str)
                    .Select<Paper>()
                    .WhereIf(tagAdd, u => paperIds.Contains(u.Id))
                    .OrderByPropertyName(pageDto.orderby, pageDto.isAsc)
                    .Count(out total)
                    .Page(pageDto.pageindex, pageDto.pagesize)
                    .ToList();

            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(pageDto.whereJsonStr);

           
            return fsql.Get(conn_str)
                .Select<Paper>()
                .WhereDynamicFilter(dyfilter)
                .WhereIf(tagAdd, u => paperIds.Contains(u.Id))

                .OrderByPropertyName(pageDto.orderby, pageDto.isAsc)
                .Count(out total)
                .Page(pageDto.pageindex, pageDto.pagesize).ToList();
        }

        /// <summary>
        /// 设定组卷规则 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Guid[]> SetPaperRule(GeneratePaperDto model)
        {
            try
            {
                List<Paper> targetPapers = new List<Paper>(model.paperNumber);
               
                var questionRepo = fsql.Get(conn_str).GetRepository<QuestionView>();

                if (model.generateQuestionTypeModels.Sum(u => u.number) > await questionRepo.Where(u => u.IsDeleted == 0).CountAsync())
                {
                    return null;
                }

                var paperRepo = fsql.Get(conn_str).GetRepository<Paper>();
                var examRepo = fsql.Get(conn_str).GetRepository<Examination>();

                long existPaperCnt = await paperRepo.Where(u => u.ExamId == model.examId).CountAsync();
                var exam = await examRepo.Where(u => u.Id == model.examId).FirstAsync();
                int paperNum = 0;//试卷套数
                int IncludeSubjective = 0;
                var typeIds = model.generateQuestionTypeModels.Select(u => u.typeId);
                if(await questionRepo.Where(u=> typeIds.Contains(u.TypeId) && u.Objective == 0).AnyAsync())
                {
                    IncludeSubjective = 1;
                }

                while (paperNum < model.paperNumber)
                {
                    foreach (var sub in model.generateQuestionTypeModels)
                    {
                        long totalQuestion = await questionRepo
                            .Where(u => u.SubjectId == sub.subjectId && u.TypeId == sub.typeId && u.IsDeleted == 0)
                            .WhereIf(!string.IsNullOrEmpty(model.degrees) && model.degrees.ToLower() != "all", u => model.degrees.Contains(u.Degree))
                            .CountAsync();
                        if (totalQuestion < sub.number)
                        {
                            return null;//有异常
                        }

                    }

                    var paper = new Paper()
                    {
                        Title = $"{exam.Title}卷{paperNum + existPaperCnt + 1}",
                        Remark = $"卷{paperNum + existPaperCnt + 1}抽题情况",
                        Status = 0,
                        ExamId = model.examId,
                        Score = model.paperScore,
                        Duration = exam.BaseDuration,
                        OpenResult = (PaperOpenResult)model.openResult,
                        PaperDegree = model.degrees,
                        IncludeSubjective = IncludeSubjective
                    };

                    paper.QuestionDetailJson = JsonHelper.JsonSerialize(model.generateQuestionTypeModels);

                    targetPapers.Add(paper);
                    paperNum++;
                }
                var list = await paperRepo.InsertAsync(targetPapers);

                return list.Select(u => u.Id).ToArray();
            }
            catch
            {
                throw;
            }

        }

        /// <summary>
        /// 针对规则，生成试题
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> GeneratePaper(Guid[] paperIds, string adminId)
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    if(paperIds==null || paperIds.Length==0)
                    {
                        return 0;
                    }
                    //var examRepo = fsql.Get(conn_str).GetRepository<Examination>();
                    //var examItem = await examRepo.Where(u => u.Id == paperIds[0]).FirstAsync();
                    var examItem = await fsql.Get(conn_str).Select<Examination, Paper>()
                        .LeftJoin((a, b) => a.Id == b.ExamId)
                        .Where((a, b) => b.Id == paperIds[0])
                        .ToOneAsync((a, b) => a);
                    if (examItem == null)
                    {
                        return -1;
                    }
                    long activityId = Convert.ToInt64(examItem.AssociationId);
                    var paperRepo = fsql.Get(conn_str).GetRepository<Paper>();
                    var questionRepo = fsql.Get(conn_str).GetRepository<Question>();
                    var relationRepo = fsql.Get(conn_str).GetRepository<Relation>();
                    paperRepo.UnitOfWork = uow;
                    questionRepo.UnitOfWork = uow;
                    relationRepo.UnitOfWork = uow;

                    int totalRelation = 0;
                    List<Relation> targetRelations = new List<Relation>();

                    foreach (Guid paperId in paperIds)
                    {
                        var paper = await paperRepo.Where(u => u.Id == paperId).FirstAsync();
                        var rules = JsonHelper.JsonDeserialize<List<GenerateQuestionTypeDto>>(paper.QuestionDetailJson);

                        foreach (var rule in rules)
                        {
                            var selectedQuestions = await questionRepo
                                .Where(u => u.SubjectId == rule.subjectId && u.TypeId == rule.typeId && u.IsDeleted == 0)
                                //.WhereIf(paper.PaperType == PaperType.Practice, u => u.IsOpen == IsOpen.Yes)//如果是生成练习题，那就只抽取开放的题
                                .Where(u => u.IsOpen == IsOpen.Yes)
                                .WhereIf(!string.IsNullOrEmpty(paper.PaperDegree) && paper.PaperDegree != "all", u => paper.PaperDegree.Contains(u.Degree))//如果没有设定试卷难度，那就抽取对应难度的题
                                .Where(u => u.ActivityId == 0 || u.ActivityId == activityId)
                                .ToListAsync();
                            int selectedCnt = selectedQuestions.Count();

                            int questionNumber = 0;
                            while (questionNumber < rule.number)
                            {
                                var selectedQuestionIndex = new Random().Next(0,selectedCnt-1);
                                var randomQuestion = selectedQuestions[selectedQuestionIndex];
                                targetRelations.Add(new Relation()
                                {
                                    ExamId = paper.ExamId,
                                    PaperId = paperId,
                                    QuestionId = randomQuestion.Id,
                                    ItemScore = rule.itemScore == 0 ? randomQuestion.Score : rule.itemScore,
                                    CreatedBy = adminId,
                                    Remark = "抽题组卷"
                                });
                                questionNumber++;
                                selectedQuestions.Remove(randomQuestion);
                                selectedCnt--;
                            }
                        }
                        if (targetRelations.Count > 1000)
                        {
                            totalRelation += 1000;
                            await relationRepo.InsertAsync(targetRelations);
                            targetRelations.Clear();
                        }
                    }
                    totalRelation += targetRelations.Count;
                    await relationRepo.InsertAsync(targetRelations);
                    uow.Commit();
                    return totalRelation;
                }
                catch
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        public async Task<int> UpdatePaperExamDuration(Examination exam)
        {
            
            return await fsql.Get(conn_str).Select<Paper>()
                .Where(u => u.ExamId == exam.Id)
                .Where(u => u.IsDeleted == 0)
                .ToUpdate()
                .Set(u => u.Duration == exam.BaseDuration)
                .ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 预览试卷
        /// </summary>
        /// <param name="paperId"></param>
        /// <returns></returns>
        public async Task<FinalPaperDto> PreviewPaper(Guid paperId)
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
                    Status = (int)a.Status,
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
                    IsOpen = a.IsOpen,
                    Title = a.Title,
                    SingleAnswer = a.SingleAnswer,
                    Analysis = a.Analysis,
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
                        IsAnswer = u.IsAnswer,
                        OrderIndex = u.OrderIndex==null?0:1
                    });
                question.Items = items.ToList();
            }

            paper.Questions = paperQuestions;

            return paper;
        }


        /// <summary>
        /// 提交试卷
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<dynamic> SubmitPaper(SubmitPaperDto dto)
        {
            if (await RedisHelper.HGetAsync("UserExamLog", dto.reportId.ToString()) != dto.idNumber)
            {
                return -1;//当前赛队提交答卷的人和初始化答题的人不是一个，正常情况不会出现，这里就时以防万一
            }
            //交卷要开事务，避免并发问题
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    //取出试卷详情
                    var paperRepo = fsql.Get(conn_str).GetRepository<Paper>();
                    paperRepo.UnitOfWork = uow;
                    var paper = await paperRepo.Where(u => u.Id == dto.paperId).ToOneAsync();

                    var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
                    userAnswerRecordRepo.UnitOfWork = uow;
                    //当前试卷下所有的答题记录
                    var currPaperRecords = await userAnswerRecordRepo.Where(u => u.PaperId == dto.paperId).ToListAsync(u => new
                    {
                        u.IdNumber,
                        u.ReportId,
                        u.Score,
                        u.ObjectiveScore
                    });

                    var record = new UserAnswerRecord();
                    //如果已经创建答题记录了，一般情况下都会进入这段，因为确定答题时会有一步初始化的操作，相关的验证都会在那边集成好
                    //而下面那段else判定是预留的
                    if (currPaperRecords.Where(u => u.IdNumber == dto.idNumber).Any())
                    {
                        record = await userAnswerRecordRepo.Where(u => u.IdNumber == dto.idNumber).ToOneAsync();
                        if (record.ReportId != dto.reportId)
                        {
                            uow.Dispose();//释放，正常情况不会走到这里，交卷前还会有验证
                            return new { code = -1, msg = "当前身份证号已经提交了其他活动的试卷，请检查答题人身份证号是否要参加当前考试" };
                        }
                        record.UpdatedBy = dto.accountId.ToString();
                        record.CheatCnt = dto.cheatCnt;
                        record.UpdatedAt = DateTime.Now;
                        //record.Remark += $"交卷，答题人识别码[{dto.idNumber}]；";
                        string remark = $"交卷,答题人识别码[{dto.idNumber}]";
                        if (!record.Remark.Contains(remark))
                            record.Remark += remark;
                    }
                    else
                    {
                        record.AccountId = dto.accountId;
                        record.UserName = dto.userName;
                        record.CreatedBy = dto.accountId.ToString();
                        record.CreatedAt = DateTime.Now;
                        record.PaperId = dto.paperId;
                        record.ExamId = paper.ExamId;
                        record.IdNumber = dto.idNumber;
                        record.ReportId = dto.reportId;
                        //record.Complated = 0;//创建考试
                        record.Remark = $"初始化答题并交卷，答题人识别码[{dto.idNumber}];";
                    }
                    
                    record.ComplatedMode = (ExamComplatedMode)dto.complatedMode;
                    record.Complated = (ExamComplated)(dto.complatedMode == 0 ? 0 : 1);
                    record.SubmitAnswer = JsonHelper.JsonSerialize(dto.Answers);
                    await userAnswerRecordRepo.InsertOrUpdateAsync(record);
                    await RedisHelper.HDelAsync("UserExamLog", dto.reportId.ToString());
                    uow.Commit();
                    return 1;
                }
                catch
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 校准试卷
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<dynamic> SubmitPaperForCorrection(SubmitPaperForCorrectionDto dto)
        {
            //交卷要开事务，避免并发问题
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    //取出试卷详情
                    var paperRepo = fsql.Get(conn_str).GetRepository<Paper>();
                    paperRepo.UnitOfWork= uow;
                    var paper = await paperRepo.Where(u => u.Id == dto.paperId).ToOneAsync();
                    paper.Title = dto.paperTitle;
                    paper.Status =(ExamStatus)dto.paperStatus;
                    paper.Duration = dto.paperDuration;
                    paper.OpenResult = (PaperOpenResult)dto.paperOpenResult;
                    paper.UpdatedBy = dto.adminId + "修改试卷信息";
                    paper.UpdatedAt = DateTime.Now;
                    await paperRepo.UpdateAsync(paper);
                    if (dto.answers != null && dto.answers.Any())
                    {
                        var questionRepo = fsql.Get(conn_str).GetRepository<QuestionView>();
                        var itemRepo = fsql.Get(conn_str).GetRepository<QuestionItem>();
                        questionRepo.UnitOfWork = uow;
                        itemRepo.UnitOfWork = uow;

                        foreach (var answer in dto.answers)
                        {
                            if (answer.userAnswer.Length == 0 || answer.userAnswer.Any(value => string.IsNullOrEmpty(value)))
                            {
                                continue;//啥也没改
                            }
                            var question = await questionRepo.Where(u => u.Id == answer.questionId).ToOneAsync();
                            var items = await itemRepo.Where(u => u.QuestionId == answer.questionId).ToListAsync();

                            foreach (var item in items)
                            {
                                if (question.Objective == 1)
                                {
                                    item.IsAnswer = 0;
                                    if (answer.userAnswer.Any(vaule => vaule == item.Id.ToString()))
                                        item.IsAnswer = 1;
                                }
                                else
                                {
                                    item.Description = string.Join("|", answer.userAnswer);
                                }
                                item.UpdatedAt = DateTime.Now;
                                item.UpdatedBy = dto.adminId + "校准答案";
                                await itemRepo.UpdateAsync(item);
                            }

                        }
                    }
                    uow.Commit();
                    return 1;
                }
                catch
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 打分
        /// </summary>
        /// <param name="idNumber"></param>
        /// <returns></returns>
        [Obsolete("过期，客观题打分请统一使用ScoreObjectivePart方法")]
        public async Task<dynamic> Marking(string idNumber, Guid paperId, string adminId)
        {
            //第一步：把提交的答案取出来
            var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
            var record = await userAnswerRecordRepo.Where(u => u.IdNumber == idNumber && u.PaperId == paperId && u.IsDeleted == 0).ToOneAsync();
            if(record.Marked!= ExamMarked.No)
                return 1;
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
            double userObjectScore = 0;
            foreach (var answer in Answers)
            {
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
                        userObjectScore += relation.ItemScore;//得分
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
                        userObjectScore += relation.ItemScore;
                    }
                }
            }

            record.Remark += $"客观题成绩为{userObjectScore}分,打分人{adminId}";
            record.UpdatedAt = DateTime.Now;
            record.UpdatedBy = adminId;
            record.ObjectiveScore = userObjectScore;
            if (record.Marked == ExamMarked.No)
                record.Score = userObjectScore;//此时总分数就是客观题分数
            record.Marked = ExamMarked.Part;
            return await userAnswerRecordRepo.UpdateAsync(record); ;

        }
        //一道题一道题的提交，就在控制器里把提交记录写到缓存里就行，不用写数据层的方法了
        // public async Task<dynamic> SubmitPaperOnebyOne()
    }
}
