using Magic.Guangdong.Assistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Exam.Papers
{
    public class GeneratePaperDto
    {
        public Guid examId { get; set; }


        /// <summary>
        /// 是否开放查询成绩，即判卷完成后立刻可以查到成绩
        /// </summary>
        public int openResult { get; set; } = 0;

        /// <summary>
        /// 组卷方式
        /// 0-机器组卷（默认,考试之前先随机生成多套试卷，考试时随机分发，推荐采用此方式，考试前生成多套试卷既可以满足随机性，也可以对不同的试卷提前校验，避免题目疏漏等问题），
        /// 1-人工组卷（人工组卷的出卷量低，每场考试可能只可以组好少数几套试卷。但人为参与可以最大限度保证试卷结构的合理性，适合严谨的考试场合，比如规定具体考试时间的统一考试，不适合单独测试），
        /// 2-即时组卷（每个学生考试时随机生成，不推荐正式考试用这种方式，无法对试卷进行校验，更适合一些自主练习的场景）
        /// </summary>
        public int paperType { get; set; } = 0;

        public int paperNumber { get; set; }

        public string adminId { get; set; }

        public double paperScore { get; set; } = 0;

        public string degrees { get; set; }

        public List<GenerateQuestionTypeDto> generateQuestionTypeModels{
            get {
                if (!string.IsNullOrWhiteSpace(generateQuestionTypeModelsStr))
                {
                    return JsonHelper.JsonDeserialize<List<GenerateQuestionTypeDto>>(generateQuestionTypeModelsStr);
                }
                return null;
            }
        }

        public string generateQuestionTypeModelsStr {  get; set; }
    }

    public class GenerateQuestionTypeDto
    {
        /// <summary>
        /// 题型
        /// </summary>
        public Guid typeId { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public Guid subjectId { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public int orderIndex { get; set; }
        /// <summary>
        /// 题型对应的题目数量
        /// </summary>
        public int number { get; set; }
        /// <summary>
        /// 题型对应的总分数
        /// </summary>
        //public double scoreTotal { get; set; }

        /// <summary>
        /// 题型对应的实际分数，默认继承题目分数
        /// </summary>
        public double itemScore { get; set; }

    }
}
