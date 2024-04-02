
using Magic.Guangdong.Assistant.Dto;
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
    public interface IQuestionRepo : IExaminationRepository<Question>
    {
        dynamic GetQuestions(PageDto dto, out long total);

        /// <summary>
        /// 新增或更新单条题目
        /// </summary>
        /// <param name="question"></param>
        /// <param name="questionItems"></param>
        /// <returns></returns>
        Task<int> AddOrUpdateSingleQuestion(Question question, List<QuestionItem> questionItems);

        /// <summary>
        /// 批量导入题目
        /// </summary>
        /// <param name="Items"></param>
        /// <returns></returns>
        Task<int> ImportQuestionsFromWord(List<ImportQuestionFromWord> Items);

        /// <summary>
        /// 从excel里导入题库
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<int> ImportQuestionFromExcel(List<ImportQuestionDto> items);
    }
}
