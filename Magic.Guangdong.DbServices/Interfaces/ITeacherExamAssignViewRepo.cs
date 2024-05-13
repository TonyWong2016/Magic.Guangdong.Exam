using Magic.Guangdong.DbServices.Dtos.Teacher;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface ITeacherExamAssignViewRepo : IExaminationRepository<TeacherExamAssignView>
    {
        Task<dynamic> GetTeacherExams(Guid teacherId);
        /// <summary>
        /// 获取用户提交的主观题
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        Task<TeacherSubjectiveMarkDto> GetSubjectiveQuestionAndAnswers(long recordId);

        Task<List<TeacherRecordScoreLogDto>> GetSubjectiveScoreLog( long recordId);


        Task<TeacherSummaryDto> GetTeacherSummaryData(Guid teacherId);

        Task<List<TeacherPapersCntDto>> GetTeacherPapersSummaryData(Guid teacherId);

        Task<List<TeacherPapersMarkedCntLast7DaysDto>> GetTeacherMarkedCntLast7Days(Guid teacherId);
    }
}
