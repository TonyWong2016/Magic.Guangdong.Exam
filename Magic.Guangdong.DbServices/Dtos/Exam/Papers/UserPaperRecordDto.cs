using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Magic.Guangdong.DbServices.Dtos.Exam.Papers
{
    public class UserPaperRecordDto
    {
        /// <summary>
        /// 抽题人ID
        /// </summary>
        //public long userId { get; set; }
        public string accountId { get; set; }

        /// <summary>
        /// 抽题人
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 唯一编号(身份证号/准考证号)
        /// </summary>
        public string idNumber { get; set; }

        /// <summary>
        /// 申报id
        /// </summary>
        public long reportId { get; set; }

        /// <summary>
        /// 考试id
        /// </summary>
        public Guid examId { get; set; }

        public string loginType { get; set; } = "证件号登录";

        /// <summary>
        /// 考试关键字
        /// 因为同一活动可以创建多个考试，需要用关键字确定具体是哪场考试
        /// 关键字对应考试表里的title
        /// </summary>
        //public string examKeyword { get; set; }
    }


    public class UserAnswerDto
    {
        public long recordId { get; set; }
        /// <summary>
        /// 是否完成，判定为1时，则直接交卷
        /// </summary>
        public int finished { get; set; } = 0;

        public AnswerDto answerDto { get; set; }
    }

    public class AnswerDto
    {
        public long questionId { get; set; }

        public string[] userAnswer { get; set; }
    }

    /// <summary>
    /// 提交答案模型
    /// </summary>
    public class SubmitMyAnswerDto
    {
        public long recordId { get; set; }

        public long reportId { get; set; }

        public string submitAnswerStr { get; set; } = "";

        public string idNumber { get; set; }

        public int userId { get; set; }

        public int complatedMode { get; set; } = 0;

        public int isPractice { get; set; } = 0;

        //public double usedTime { get; set; }
    }

    /// <summary>
    /// 作弊记录
    /// </summary>
    public class CheatDto
    {
        public long recordId { get; set; }
    }


    public class UserAnswerRecordDto
    {
        public long Urid { get; set; }


        public string AccountName { get; set; }

        [Description("答题账号")]
        public string AccountNameStr
        {
            get
            {
                if (!string.IsNullOrEmpty(AccountName))
                    return HttpUtility.UrlDecode(AccountName);
                return "无";
            }
        }

        [Description("证件号")]
        public string IdNumber { get; set; }

        [Description("活动标题")]
        public string AssociationTitle { get; set; }

        [Description("考试标题")]
        public string ExamTitle { get; set; }

        [Description("试卷标题")]
        public string PaperTitle { get; set; }

        [Description("总分数")]
        public string Score { get; set; }

        [Description("客观题分数")]
        public string ObjectScore { get; set; }



        [Description("完成情况")]
        public string Complated { get; set; }

        [Description("项目编号")]
        public string ProjectNo { get; set; } = "";

        //[Description("赛队名称")]
        public string TeamName { get; set; } = "";

        [Description("赛队人员")]
        public string Participants { get; set; } = "";

        [Description("学校")]
        public string Schools { get; set; } = "";

        [Description("指导教师")]
        public string Teachers { get; set; } = "";

        [Description("组别")]
        public string GroupName { get; set; } = "";
    }

    public class UserAnswerRecordMiniDto
    {
        public long recordId { get; set; }

        public string idNumber { get; set; }

        public string applyId { get; set; }

    }

    public class ExamRecordDto
    {
        public long recordId { get; set; }
        public string examTitle { get; set; }

        public string paperTitle { get; set; }

        public Guid paperId { get; set; }

        public Guid examId { get; set; }

        public string accountName { get; set; }

        public string idNumber { get; set; }

        public string applyId { get; set; }

        public string associationId { get; set; }

        public int openResult { get; set; }

        public int marked { get; set; }

        public double score { get; set; }

        public int isComplated { get; set; } = 0;

        public DateTime CreatedAt { get; set; }

        public string submitAnswer { get; set; }

        public DateTime LimitedAt { get; set; }

        public int isDeleted { get; set; } = 0;

        public int isStrict { get; set; } = 0;

        public int testedTime { get; set; } = 0;

        public int examType { get; set; }
    }
}
