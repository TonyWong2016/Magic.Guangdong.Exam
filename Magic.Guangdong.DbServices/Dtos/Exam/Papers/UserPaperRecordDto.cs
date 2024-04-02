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
        public long userId { get; set; }

        /// <summary>
        /// 抽题人
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 唯一编号(身份证号)
        /// </summary>
        public string idNumber { get; set; }

        /// <summary>
        /// 申报id
        /// 如果需要关联赛队，这里要填写申报人员的申报id
        /// 不需要的话这里直接给个唯一值就可以，比如guid,这样就跳过同赛队参赛的验证了。
        /// </summary>
        public string reportId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 考试id
        /// </summary>
        public Guid examId { get; set; }

        /// <summary>
        /// 考试关键字
        /// 因为同一活动可以创建多个考试，需要用关键字确定具体是哪场考试
        /// 关键字对应考试表里的title
        /// </summary>
        //public string examKeyword { get; set; }
    }

    public class MemberInfoDto
    {
        public string applyId { get; set; }

        public string email { get; set; }

        public string memberId { get; set; }

        public string memberName { get; set; }

        public string mobile { get; set; }

        public string photo { get; set; }

        public string school { get; set; }

        public string sex { get; set; }

        public string projectNo { get; set; }

        public string programType { get; set; }

        public string areaName { get; set; }

        public int? provinceCheck { get; set; } = 0;
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
    /// 暂时用不到
    /// </summary>
    public class SubmitMyAnswerDto
    {
        public long recordId { get; set; }

        public string reportId { get; set; }

        public string submitAnswerStr { get; set; } = "";

        public string idNumber { get; set; }

        public int userId { get; set; }

        public int complatedMode { get; set; } = 0;

        public double usedTime { get; set; }
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
        public long urid { get; set; }


        public string accountName { get; set; }

        [Description("答题账号")]
        public string accountNameStr
        {
            get
            {
                if (!string.IsNullOrEmpty(accountName))
                    return HttpUtility.UrlDecode(accountName);
                return "无";
            }
        }

        [Description("证件号")]
        public string idNumber { get; set; }

        [Description("活动标题")]
        public string associationTitle { get; set; }

        [Description("考试标题")]
        public string examTitle { get; set; }

        [Description("试卷标题")]
        public string paperTitle { get; set; }
        [Description("分数")]
        public string score { get; set; }

        [Description("完成情况")]
        public string complated { get; set; }
    }

    public class UserAnswerRecordMiniDto
    {
        public long recordId { get; set; }

        public string idNumber { get; set; }

        public string applyId { get; set; }

    }
}
