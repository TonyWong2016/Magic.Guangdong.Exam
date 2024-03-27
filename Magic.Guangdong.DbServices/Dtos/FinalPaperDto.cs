using FreeSql.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dto
{
    public class FinalPaperDto
    {
        public Guid PaperId { get; set; }

        public Guid ExamId { get; set; }
        public string ExamTitle { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string PaperTitle { get; set; }

        public string AssociationId { get; set; }

        public string AssociationTitle { get; set; }

        public double PaperScore { get; set; }

        public double Duration { get; set; }

        public int PaperType { get; set; }

        public string PaperTypeStr
        {
            get
            {
                if (PaperType == 0) return "机器组卷";
                if (PaperType == 1) return "人工组卷";
                if (PaperType == 2) return "随机抽题";
                return "";
            }
        }

        public int Status { get; set; }


        public int OpenResult { get; set; }


        public List<PaperQuestionDto> Questions { get; set; }

    }

    public class PaperQuestionDto
    {

        public long Id { get; set; }

        public string Title { get; set; }

        public string TitleRaw
        {
            get
            {
                //string filterTitle= Regex.Replace(Title, @"^\d+(\.|、|,|\w)", "");
                //@"^\d+(\.|、|,|\w)"加上^标识匹配开头，但富文本开头不一定是序号，可能有html字符
                return Regex.Replace(Title, @"^\d+(\.|、|,|\w)", "");
            }
            set { }
        }

        public string Degree { get; set; }

        public int IsOpen { get; set; }

        public string SubjectName { get; set; }

        public string TypeName { get; set; }

        public int? IsObjective { get; set; }

        public int? SingleAnswer { get; set; }

        public double ItemScore { get; set; }

        public string Analysis { get; set; }

        public string AnalysisTxt
        {
            get
            {
                if (!string.IsNullOrEmpty(Analysis))
                    return Magic.Guangdong.Assistant.Utils.StripHTML(Analysis);
                return "无";
            }
        }

        public List<PaperQuestionItemDto> Items { get; set; }
    }

    public class PaperQuestionItemDto
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public int IsAnswer { get; set; }

        public string DescriptionTxt { get; set; }

        public int? OrderIndex { get; set; }
    }

    /// <summary>
    /// 初始化答题记录(开始作答的时候)
    /// </summary>
    public class InitPaperDto
    {
        public string userId { get; set; }

        public string currUserName { get; set; }

        public Guid PaperId { get; set; }
    }

    public class SubmitPaperDto
    {
        public long userId { get; set; }

        public string userName { get; set; }

        public Guid paperId { get; set; }

        public string idNumber { get; set; }

        public int cheatCnt { get; set; } = 0;

        public int complatedMode { get; set; } = 0;

        public string applyId { get; set; } = Guid.NewGuid().ToString();

        // public double itemScore { get; set; }
        public List<SubmitAnswerDto> Answers { get; set; }
    }

    public class SubmitAnswerDto
    {
        public long questionId { get; set; }

        public string[] userAnswer { get; set; }

        ////客观题答案
        //public long[] objectiveItems { get; set; }

        ////主观题答案
        //public string[] subjectiveItems { get;set; }
    }

    public class SubmitPaperForCorrectionDto
    {
        public Guid paperId { get; set; }
        public string paperTitle { get; set; }

        public double paperDuration { get; set; }

        public int paperStatus { get; set; }

        public int paperOpenResult { get; set; }

        public string adminId { get; set; }

        public List<SubmitAnswerDto> answers { get; set; }
    }
}
