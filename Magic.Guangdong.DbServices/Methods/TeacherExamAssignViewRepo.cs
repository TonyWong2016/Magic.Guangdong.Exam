using Magic.Guangdong.DbServices.Dtos.Teacher;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class TeacherExamAssignViewRepo : ExaminationRepository<TeacherExamAssignView>, ITeacherExamAssignViewRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public TeacherExamAssignViewRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<dynamic> GetTeacherExams(Guid teacherId)
        {
            var teacherExamAssignViewRepo = fsql.Get(conn_str).GetRepository<TeacherExamAssignView>();
            return await teacherExamAssignViewRepo.Where(u => u.TeacherId == teacherId)
                .ToListAsync(u => new
                {
                    value = u.ExamId,
                    text = u.ExamTitle
                });
        }
        
        /// <summary>
        /// 获取用户主观题答案详情
        /// 注意，这里入参是recordId，每个record下会有多个主观题，所以返回值是一个列表
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public async Task<TeacherSubjectiveMarkDto> GetSubjectiveQuestionAndAnswers(long recordId)
        {
            try
            {
                var userAnswerRecordViewRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecordView>();
                var userAnswerRecord = await userAnswerRecordViewRepo.Where(u => u.Id == recordId).ToOneAsync();
                
                var examInfoDto = userAnswerRecord.Adapt<ExamInfoDto>();
                examInfoDto.IdNumber = $"{userAnswerRecord.IdNumber.Substring(0, 2)}************{userAnswerRecord.IdNumber.Substring(userAnswerRecord.IdNumber.Length - 2, 2)}";
                examInfoDto.RecordId = userAnswerRecord.Id;
                //examInfoDto.AssignId = assignId;

                var userAnswerSubmitRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerSubmitRecord>();
                var userAnswerSubmitRecords = await userAnswerSubmitRecordRepo
                    .Where(u => u.RecordId == recordId && u.IsSubjective == 1)
                    //.OrderBy(u=>u.Id)
                    .ToListAsync();

                var questionIds = userAnswerSubmitRecords.Select(u => u.QuestionId);
                var questionViewRepo = fsql.Get(conn_str).GetRepository<QuestionView>();
                var questions = await questionViewRepo
                    .Where(u => questionIds.Contains(u.Id))
                    .ToListAsync();


                List<SubjectiveQuestionAndAnswersDto> items = new List<SubjectiveQuestionAndAnswersDto>();
                foreach (var question in questions)
                {
                    items.Add(new SubjectiveQuestionAndAnswersDto()
                    {
                        question = question,
                        userAnswer = userAnswerSubmitRecords
                        .Where(u => u.QuestionId == question.Id).FirstOrDefault()
                    });
                }
                TeacherSubjectiveMarkDto result = new TeacherSubjectiveMarkDto()
                {
                    examInfoDto = examInfoDto,
                    subjectiveQuestionAndAnswers = items
                };
                return result;

            }
            catch (Exception ex)
            {
                Assistant.Logger.Error(ex.Message);
                return new TeacherSubjectiveMarkDto();
            }            
        }


        public async Task<List<TeacherRecordScoreLogDto>> GetSubjectiveScoreLog(Guid teacherId, long recordId)
        {
            return await fsql.Get(conn_str).Select<TeacherRecordScoreLog, Teacher>()
                .LeftJoin((a, b) => a.TeacherId == b.Id)
                .Where((a, b) => a.TeacherId == teacherId && a.RecordId == recordId)
                .OrderByDescending((a,b)=>a.Id)
                .ToListAsync((a, b) => new TeacherRecordScoreLogDto()
                {
                    ScoreTime = a.CreatedAt,
                    SubjectiveScore = a.SubjectiveScore,
                    TeacherId = a.TeacherId,
                    TeacherName = b.Name,
                    Id = a.Id
                });
        }
    
    
        public async Task<TeacherSummaryDto> GetTeacherSummaryData(Guid teacherId)
        {
            TeacherSummaryDto dto = new TeacherSummaryDto();
            var examIds = await fsql.Get(conn_str).Select<TeacherExamAssign>()
                .Where(u => u.TeacherId == teacherId && u.IsDeleted == 0)
                .ToListAsync(u => u.ExamId);

            dto.ExamCnt = examIds.Count;

            dto.MarkedCnt = await fsql.Get(conn_str).Select<UserAnswerRecord>()
                .Where(u => examIds.Contains(u.ExamId) && u.IsDeleted == 0)
                .CountAsync();
            var sql = $"select count(1) as PapersCnt,ExamId,ExamTitle from [UserAnswerRecordView] where ExamId in (select ExamId from [TeacherExamAssign] where teacherId='{teacherId}' and IsDeleted=0) and IsDeleted=0 group by ExamId,ExamTitle";
            dto.PapersCntList = await fsql.Get(conn_str).Ado.QueryAsync<TeacherPapersCntDto>(sql);

            return dto;
        }

        public async Task<TeacherPapersMarkedCntDto> GetTeacherExamSummaryData(Guid teacherId, Guid examId)
        {
            //string markedCntSql = $"select recordId from from [TeacherRecordScoreLog] where teacherId='{teacherId}' and examId='{examId}' group by recordId";
            //string totalCntSql = $"select recordId from from [UserAnswerRecord] where examId='{examId}'";

            long markedCnt = await fsql.Get(conn_str).Select<TeacherRecordScoreLog>()
                .Where(u => u.TeacherId == teacherId && u.ExamId == examId)
                .GroupBy(u => new { u.TeacherId, u.ExamId })                
                .CountAsync();
            long totalCnt = await fsql.Get(conn_str).Select<UserAnswerRecord>()
                .Where(u => u.ExamId == examId && u.IsDeleted == 0)
                .CountAsync();

            return new TeacherPapersMarkedCntDto { MarkedCnt = markedCnt, UnMarkedCnt = totalCnt - markedCnt };
        }
    }
}
