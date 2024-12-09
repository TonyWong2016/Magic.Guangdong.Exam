using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.Examinations;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IExaminationRepo : IExaminationRepository<Examination>
    {
        Task<dynamic> GetExamMini(string id, int type, int examType = -1);

        Task<List<ExaminationDropsDto>> GetExamDrops();

        dynamic GetExamList(PageDto dto, out long total);

        Task<bool> UpdateExamInfo(Examination exam);

        Task<bool> DeleteExamInfo(Guid examId);

        /// <summary>
        /// 克隆考试
        /// </summary>
        /// <param name="examId"></param>
        /// <param name="adminId"></param>
        /// <param name="cloneExamName"></param>
        /// <param name="clonePaperTitle"></param>
        /// <returns></returns>
        Task<Guid[]> CloneExam(Guid examId, string adminId, string cloneExamName = "", string clonePaperTitle = "");

    }
}
