using Essensoft.Paylink.Alipay.Domain;
using log4net.Repository.Hierarchy;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
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
    internal class UserAnswerSubmitRecordRepo : ExaminationRepository<UserAnswerSubmitRecord>, IUserAnswerSubmitRecordRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public UserAnswerSubmitRecordRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        /// <summary>
        /// 判客观题分数，统一使用该方法，其余都作废
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="markingType">
        /// 判分类型，
        /// 0-正常交卷，自然判分
        /// 1-时间到，系统强制交卷
        /// 2-后台强制交卷
        /// </param>
        /// <returns></returns>
        public async Task<int> ScoreObjectivePart(long recordId, int markingType=0)
        {           

            var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
            if (!await userAnswerRecordRepo.Where(u => u.Id == recordId).AnyAsync())
            {
                //不存在
                return -1;
            }
            //前置处理流程，获取答题记录，并做必要的数据处理
            var preModel = await ScoreObjectivePreOption(recordId, markingType > 0);
            if (preModel.immeReturn)
            {
                //return preModel.recordView;
                return 0;//已经给分了
            }
            var recordView = preModel.recordView;
            try
            {
                var questionRecordRepo = fsql.Get(conn_str).GetRepository<QuestionRecord>();
                                
                double userObjectiveScore = 0;

                //第一步，反序列化用户的答案
                List<SubmitAnswerDto>? Answers = ScoreObjectiveStepOne(recordView.SubmitAnswer);
                if (Answers == null)
                {
                    return -2;//答案记录的有异常
                }

                //第二步,把试卷的题目和分数取出来，注意这里有一个取分表数据的策略
                var relations = await ScoreObjectiveStepTwo(recordView.CreatedAt.Year, recordView.PaperId);

                //第三步，把用户提交的题目的正确答案都取出来
                var correctItems = await ScoreObjectiveStepThree(Answers);

                //第三点五步，取出评分标准
                var scoreScheme = await GetScoreSchemeByExamId(recordView.ExamId);

                //第四步，计算分数
                List<UserAnswerSubmitRecord> userSubmitAnswerRecords = new List<UserAnswerSubmitRecord>();
                foreach (SubmitAnswerDto answer in Answers)
                {
                    if (!relations.Where(u => u.QuestionId == answer.questionId).Any())
                    {
                        continue;
                    }
                    var relation = relations.Where(u => u.QuestionId == answer.questionId).First();
                    var scoreStepFourResult = ScoreObjectiveStepFourPart(answer, ref relation, ref correctItems, ref scoreScheme, recordId);
                    userSubmitAnswerRecords.Add(scoreStepFourResult.userAnswerSubmitRecord);
                    userObjectiveScore += scoreStepFourResult.objectiveScore;
                    
                }
                var userSubmitAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerSubmitRecord>();
                await userSubmitAnswerRecordRepo.InsertAsync(userSubmitAnswerRecords);

                var record = await userAnswerRecordRepo.Where(u => u.Id == recordId).ToOneAsync();

                record.Complated = ExamComplated.Yes;
                record.ComplatedMode = (ExamComplatedMode)markingType;
                record.Remark += $"客观题成绩为{userObjectiveScore}分";
                if (record.Remark.Length > 1000)
                {
                    record.Remark = $"客观题成绩为{userObjectiveScore}分";
                }
                if (markingType ==1 || (recordView.LimitedTime < DateTime.Now && recordView.IsStrict == 0))
                {
                    record.ComplatedMode = ExamComplatedMode.Timeup;
                    record.Remark += "（到时自动交卷）";
                }
                if (markingType == 2)
                {
                    record.ComplatedMode = ExamComplatedMode.Force;
                    record.Remark += "（后台强制交卷）";
                    await ForceSubmitSuffixOption(record);
                }
                record.UpdatedAt = DateTime.Now;
                record.UpdatedBy = "systemmarked";

                record.ObjectiveScore = userObjectiveScore;
                if (record.Marked == ExamMarked.No)
                    record.Score = userObjectiveScore;
                record.Marked = ExamMarked.Part;
                
                var paper = await fsql.Get(conn_str).Select<Paper>().Where(u => u.Id == recordView.PaperId).ToOneAsync();
                if (paper.IncludeSubjective == 0)
                    record.Marked = ExamMarked.All;
                return await userAnswerRecordRepo.UpdateAsync(record);
                
            }
            catch (Exception ex)
            {
                Assistant.Logger.Error(ex);
                //uow.Rollback();
                return -3;
            }
        }


        /// <summary>
        /// 判分的前置处理
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="submit"></param>
        /// <returns></returns>
        private async Task<ScoreStepPreResult> ScoreObjectivePreOption(long recordId,bool submit=false)
        {
            var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecordView>();
            var recordView = await userAnswerRecordRepo.Where(u => u.Id == recordId).ToOneAsync();
            var returnModel = new ScoreStepPreResult()
            {
                recordView = recordView,

            };
            //已经给过分了
            if (recordView.UpdatedBy == "systemmarked" && recordView.Marked!=0)
            {
                returnModel.immeReturn = true;
                return returnModel;
            }
            //没交卷，答题也还没结束
            if (recordView.Complated == (int)ExamComplated.No && recordView.LimitedTime > DateTime.Now && !submit)
            {
                returnModel.immeReturn = true;
                return returnModel;
            }
            //严格模式下，要判定是否超时，允许2分钟以内的误差交卷时间
            if (recordView.IsStrict == 1 && recordView.LimitedTime.AddMinutes(2) < DateTime.Now)
            {
                var record = recordView.Adapt<UserAnswerRecord>();
                //没交卷，答题也结束了且超过了误差允许范围，先给个0分
                await fsql.Get(conn_str).Select<UserAnswerRecord>().Where(u => u.Id == recordId)
                    .ToUpdate()
                    .Set(u => u.Complated, ExamComplated.Yes)
                    .Set(u => u.ComplatedMode, ExamComplatedMode.Timeup)
                    .Set(u => u.Marked, record.Marked)
                    .Set(u => u.UpdatedAt, record.LimitedTime)
                    .Set(u => u.UpdatedBy, "systemmarked")
                    .Set(u => u.Score, 0)
                    .ExecuteAffrowsAsync();
                returnModel.immeReturn = true;
                return returnModel;
            }

            //如果是答卷空的，看一下提交记录里有没有答案记录，如果没有那就确定是没提交，给0分,否则开始计算得分
            if (string.IsNullOrEmpty(recordView.SubmitAnswer) || !recordView.SubmitAnswer.Contains("userAnswer"))
            {
                var submitLogRepo = fsql.Get(conn_str).GetRepository<UserAnswerSubmitLog>();
                if (await submitLogRepo.Where(u => u.Urid == recordView.Id && u.SubmitAnswer.Contains("userAnswer")).AnyAsync())
                {
                    var submitLog = await submitLogRepo
                        .Where(u => u.Urid == recordView.Id && u.SubmitAnswer.Contains("userAnswer"))
                        .OrderByDescending(u => u.Id)
                        .ToOneAsync();
                    recordView.SubmitAnswer = submitLog.SubmitAnswer;
                    recordView.Remark += "交卷异常，取草稿中最近且正常的一次记录;";
                }
                else
                    recordView.SubmitAnswer = "[]";//的确交了白卷
            }
            returnModel.recordView = recordView;
            returnModel.immeReturn = false;
            return returnModel;
        }
    
        /// <summary>
        /// 判分第一步
        /// </summary>
        /// <param name="submitAnswer"></param>
        /// <returns></returns>
        private List<SubmitAnswerDto>? ScoreObjectiveStepOne(string submitAnswer)
        {
            try
            {
                return JsonHelper.JsonDeserialize<List<SubmitAnswerDto>>(submitAnswer);
            }
            catch (Exception ex) 
            {
                Assistant.Logger.Error(ex);
                return null;
            }
        }
    
        /// <summary>
        /// 判分第二步
        /// </summary>
        /// <returns></returns>
        private async Task<List<ScoreStepTwoResult>> ScoreObjectiveStepTwo(int recordCreatedYear,Guid paperId)
        {
            //第二步，把试卷的题目和分数取出来，注意这里有一个分表的动作，即过往年份的题目分表，所以要分表
            return await fsql.Get(conn_str).Select<Relation, QuestionView>()
               .AsTable((type, oldname) =>
               {
                   if (type.Name == "Relation" && recordCreatedYear < DateTime.Now.Year)
                       return $"Relation_{recordCreatedYear}";
                   return null;
               })
                .LeftJoin((a, b) => a.QuestionId == b.Id)
                .Where((a, b) => a.PaperId == paperId && a.IsDeleted == 0)
                .ToListAsync((a, b) => new ScoreStepTwoResult()
                {
                    QuestionId= a.QuestionId,
                    ItemScore = a.ItemScore,
                    Objective = b.Objective,
                    SingleAnswer = b.SingleAnswer
                });
        }

        /// <summary>
        /// 判分第三步
        /// </summary>
        /// <param name="Answers">第一步取得的答案列表，注意这里是值传递，可能会有性能问题</param>
        /// <returns></returns>
        private async Task<List<ScoreStepThreeResult>> ScoreObjectiveStepThree(List<SubmitAnswerDto> Answers)
        {
            //第三步，把用户提交的题目的正确答案都取出来，这里因为是读操作，不用绑定事物单元
            using (var questionItemRepo = fsql.Get(conn_str).GetRepository<QuestionItem>())
            {
                var userQuestionIds = Answers.Select(u => u.questionId).ToList();
                return await questionItemRepo
                    .Where(u => u.IsDeleted == 0 && u.IsAnswer == 1 && userQuestionIds.Contains(u.QuestionId))
                    .ToListAsync(u => new ScoreStepThreeResult()
                    {
                        isAnswer = u.IsAnswer,
                        itemId = u.Id,
                        description = u.Description,
                        Code = u.Code,
                        isOption = u.IsOption,
                        questionId = u.QuestionId
                    });
            }
        }

        /// <summary>
        /// 判分第三点五步，取评分标准
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        private async Task<ScoreScheme> GetScoreSchemeByExamId(Guid examId)
        {
            return await fsql.Get(conn_str).Select<Examination, ScoreScheme>()
                .LeftJoin((a, b) => a.SchemeId == b.Id)
                .Where((a, b) => a.Id == examId)
                .ToOneAsync((a, b) => b);
        }

        /// <summary>
        /// 第四步判分
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        private ScoreStepFourResult ScoreObjectiveStepFourPart(SubmitAnswerDto answer,ref ScoreStepTwoResult relation, ref List<ScoreStepThreeResult> correctItems, ref ScoreScheme scoreScheme, long recordId)
        {
            ScoreStepFourResult scoreStepFourResult = new ScoreStepFourResult();
            //把答题记录存到单独的表里
            var submitAnswerRecord = new UserAnswerSubmitRecord();
            submitAnswerRecord.QuestionId = answer.questionId;
            submitAnswerRecord.RecordId = recordId;
            if (relation.Objective != 1)
            {
                submitAnswerRecord.IsSubjective = 1;
                submitAnswerRecord.SubjectiveAnswer = JsonHelper.JsonSerialize(answer.userAnswer);
                submitAnswerRecord.Remark = "主观题";
                scoreStepFourResult.userAnswerSubmitRecord=submitAnswerRecord;
                scoreStepFourResult.continurLoop = true;//主观题，跳过
                return scoreStepFourResult;
            }
            submitAnswerRecord.ObjectiveAnswer = JsonHelper.JsonSerialize(answer.userAnswer);
            submitAnswerRecord.Remark = "客观题";
            scoreStepFourResult.userAnswerSubmitRecord = submitAnswerRecord;
            //double userObjectiveScore = 0;
            //如果是单选或者判断题
            if (relation.SingleAnswer == 1)
            {
                scoreStepFourResult.objectiveScore += ScoreObjectStepFourPartSingleScore(ref answer, ref relation, ref correctItems, ref scoreScheme);
            }
            //如果是多选
            if (relation.SingleAnswer == 0)
            {
                scoreStepFourResult.objectiveScore += ScoreObjectStepFourPartMultScore(ref answer, ref relation, ref correctItems, ref scoreScheme);
            }
            return scoreStepFourResult;
        }
    
    
        private double ScoreObjectStepFourPartSingleScore(ref SubmitAnswerDto answer, ref ScoreStepTwoResult relation, ref List<ScoreStepThreeResult> correctItems, ref ScoreScheme scoreScheme)
        {
            double SingleScore = 0;
            long questionId = answer.questionId;
            var currItem = correctItems.Where(u => u.questionId == questionId).First();
            //且答案正确
            if (answer.userAnswer.Length == 1 && (answer.userAnswer[0] == currItem.itemId.ToString() || answer.userAnswer[0] == currItem.Code))
            {
                SingleScore += relation.ItemScore * Math.Abs(scoreScheme.CorrectAction) ;//得分
            }
            //题目没作答
            else if (answer.userAnswer.Length == 0)
            {
                SingleScore += relation.ItemScore * Math.Abs(scoreScheme.EmptyAction) * -1;//一般emptyaction都是0
            }
            //回答错误
            else
            {
                SingleScore += relation.ItemScore * Math.Abs(scoreScheme.WrongAction) * -1;
            }
            return SingleScore;
        }

        private double ScoreObjectStepFourPartMultScore(ref SubmitAnswerDto answer, ref ScoreStepTwoResult relation, ref List<ScoreStepThreeResult> correctItems, ref ScoreScheme scoreScheme)
        {
            double MultScore = 0;
            long answerQuestionId = answer.questionId;
            var currItems = correctItems.Where(u => u.questionId == answerQuestionId).ToList();
            //没作答
            if (answer.userAnswer.Length == 0)
            {
                MultScore += relation.ItemScore * Math.Abs(scoreScheme.EmptyAction) * -1;
            }
            //答案数量和正确答案不一致，直接错
            else if (answer.userAnswer.Length != currItems.Count)
            {
                MultScore += relation.ItemScore * Math.Abs(scoreScheme.WrongAction) * -1;
            }
            //判断答案是否正确
            else
            {
                int correctCnt = 0;
                foreach (var currItem in currItems)
                {
                    foreach (var userAnswer in answer.userAnswer)
                    {
                        if (userAnswer == currItem.itemId.ToString() || userAnswer == currItem.Code)
                        {
                            correctCnt++;//这里判分的逻辑也可以改成判定数组是否包含正确答案，但实际上，字符串的包含判定还是比较耗资源的，两个循环看起来麻烦，实际相对contains的方式还是省了。
                        }
                    }
                }
                if (correctCnt == currItems.Count)
                {
                    MultScore += relation.ItemScore * Math.Abs(scoreScheme.CorrectAction);
                }
                else
                {
                    MultScore += relation.ItemScore * Math.Abs(scoreScheme.WrongAction) * -1;
                }
            }
            return MultScore;
        }
    
        /// <summary>
        /// 强制交卷后，做一些后置处理
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private async Task ForceSubmitSuffixOption(UserAnswerRecord record)
        {            
            using (var reportProcessRepo = fsql.Get(conn_str).GetRepository<ReportProcess>())
            {
                long reportId = Convert.ToInt64(record.ReportId);
                var process = await reportProcessRepo.Where(u => u.ReportId == reportId).ToOneAsync();
                process.TestedTime = record.Stage;
                process.UpdatedAt = DateTime.Now;
                await reportProcessRepo.UpdateAsync(process);
            } 
        }
    }

    /// <summary>
    /// 前置检查模型
    /// </summary>
    public class ScoreStepPreResult
    {
        public UserAnswerRecordView recordView { get; set; }

        //是否需要立即返回，true代表前置检查存在问题。
        public bool immeReturn { get; set; } = false;
    }

    /// <summary>
    /// 第二部判分流程的结果集模型
    /// </summary>
    public class ScoreStepTwoResult
    {
        public long QuestionId { get; set; }

        public double ItemScore {  get; set; }
        public int? Objective { get; set; }

        public int? SingleAnswer { get; set; }
    }

    /// <summary>
    /// 第三步判分流程的结果集模型
    /// </summary>
    public class ScoreStepThreeResult
    {
        public int isAnswer {  get; set; }
        public long itemId {  get; set; }
        public string description { get; set; }
        public string Code {  get; set; }
        public int isOption {  get; set; }
        public long questionId {  get; set; }
    }      
    
    public class ScoreStepFourResult
    {
       public UserAnswerSubmitRecord userAnswerSubmitRecord { get; set; }

        public double objectiveScore { get; set; }
        public bool continurLoop { get; set; }
    }
}              
               
               
               