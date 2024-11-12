using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IPaperRepo : IExaminationRepository<Paper>
    {
        /// <summary>
        /// 获取试卷列表（下拉）
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        Task<dynamic> GetPaperMini(Guid? examId);

        /// <summary>
        /// 获取试卷列表
        /// </summary>
        /// <param name="pageDto"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        dynamic GetPaperList(PageDto pageDto, out long total);

        /// <summary>
        /// 设定组卷规则
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Guid[]> SetPaperRule(GeneratePaperDto model);

        /// <summary>
        /// 组卷生成试题
        /// </summary>
        /// <param name="paperIds"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        Task<int> GeneratePaper(Guid[] paperIds, string adminId);

        Task<int> UpdatePaperExamDuration(Examination examId);
        /// <summary>
        /// 试卷预览
        /// </summary>
        /// <param name="paperId"></param>
        /// <returns></returns>
        Task<FinalPaperDto> PreviewPaper(Guid paperId);

        /// <summary>
        /// 提交试卷（后台测试用）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<dynamic> SubmitPaper(SubmitPaperDto dto);

        /// <summary>
        /// 校准试卷
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<dynamic> SubmitPaperForCorrection(SubmitPaperForCorrectionDto dto);

        /// <summary>
        /// 打分
        /// </summary>
        /// <param name="idNumber"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        [Obsolete("过期，客观题打分请统一使用ScoreObjectivePart方法")]
        Task<dynamic> Marking(string idNumber, Guid paperId, string adminId);
    }
}
