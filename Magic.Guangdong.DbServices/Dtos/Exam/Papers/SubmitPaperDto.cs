using Magic.Guangdong.Assistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Exam.Papers
{

    public class SubmitPaperDto
    {
        //public long userId { get; set; }
        public string accountId { get; set; }

        public string userName { get; set; }

        public Guid paperId { get; set; }

        public string idNumber { get; set; }

        public int cheatCnt { get; set; } = 0;

        public int complatedMode { get; set; } = 0;

        public long reportId { get; set; }

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

        public string answersStr {  get; set; }

        public List<SubmitAnswerDto> answers
        {
            set { }
            get
            {
                if (!string.IsNullOrWhiteSpace(answersStr))
                {
                    return JsonHelper.JsonDeserialize<List<SubmitAnswerDto>>(answersStr);
                }
                return null;
            }
        }
    }
}
