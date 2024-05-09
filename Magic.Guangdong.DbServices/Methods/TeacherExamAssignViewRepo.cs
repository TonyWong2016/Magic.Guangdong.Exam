using FreeSql.Internal.Model;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Teacher;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using MathNet.Numerics.Distributions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
        
        /// <summary>
        /// 获取用户主观题答案详情
        /// 注意，这里入参是recordId，每个record下会有多个主观题，所以返回值是一个列表
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public async Task<TeacherSubjectiveMarkDto> GetSubjectiveQuestionAndAnswers(long recordId)
        {
            //稍后把这个转化成存储过程调用，减少和数据库的通信次数
            try
            {                
                var userAnswerRecordViewRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecordView>();
                var userAnswerRecord = await userAnswerRecordViewRepo.Where(u => u.Id == recordId).ToOneAsync();
                var examInfoDto = userAnswerRecord.Adapt<ExamInfoDto>();
                examInfoDto.IdNumber = $"{userAnswerRecord.IdNumber.Substring(0, 2)}************{userAnswerRecord.IdNumber.Substring(userAnswerRecord.IdNumber.Length - 2, 2)}";

                var userAnswerSubmitRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerSubmitRecord>();
                var userAnswerSubmitRecords = await userAnswerSubmitRecordRepo
                    .Where(u => u.RecordId == recordId && u.IsSubjective == 1)
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
                return null;
            }            
        }
    }
}
