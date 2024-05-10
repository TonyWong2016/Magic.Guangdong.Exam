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
        /// <summary>
        /// 获取用户提交的主观题
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        Task<TeacherSubjectiveMarkDto> GetSubjectiveQuestionAndAnswers(long recordId);

        /// <summary>
        /// 提交判分
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> SaveSubjectiveScore(SaveSubjectiveScoreDto dto);
    }
}
