using Magic.Guangdong.DbServices.Entities;
using MassTransit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Exam.Examinations
{
    public class ExaminationDto
    {
        public Guid Id { get; set; } = NewId.NextGuid();

        public string AssociationId { get; set; } = "0";

        public string AssociationTitle { get; set; } = "无";

        /// <summary>
        /// 标准考试时长，单位：分钟，若生成得试卷没有指定时长，则沿用该时长
        /// </summary>
        public double BaseDuration { get; set; } = 15d;

        /// <summary>
        /// 标准总分数，若试卷未指定总分，则沿用该分数
        /// </summary>
        public double BaseScore { get; set; } = 100d;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public DateTime EndTime { get; set; }

        public ExamType ExamType { get; set; } = ExamType.Examination;

        public string ExtraInfo { get; set; }

        /// <summary>
        /// 聚合码，设置之后，可以生成聚合多个考试的二维码
        /// </summary>
        public string GroupCode { get; set; } = "";

        public int IsDeleted { get; set; } = 0;

        /// <summary>
        /// 是否严格交卷，是的话超时提交将不给分，否的话答考试结束后有多少分给多少分，1-是，0-否
        /// </summary>
        public int IsStrict { get; set; } = 0;

        public int OrderIndex { get; set; } = 1;

        public string Remark { get; set; }

        public DateTime StartTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ExamStatus Status { get; set; } = ExamStatus.Enabled;

        public string Title { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string UpdatedBy { get; set; }

        /// <summary>
        /// 活动报名的名额
        /// </summary>
        public int Quota { get; set; } = 0;

        /// <summary>
        /// 费用
		/// 注意，这个最小单位要控制到分！不能再小。
        /// </summary>
        public decimal Expenses { get; set; } = 0;

        /// <summary>
        /// 报名考试是否需要审核资格
        /// </summary>
        public ExamAudit Audit { get; set; } = ExamAudit.Yes;

        /// <summary>
        /// 依附的考试id
        /// 只有当考试类型为练习时，该值可以不为空，
        /// 不为空时表示当前练习依附的正式考试id，用户报名时，报名信息可一键带入
        /// </summary>
        public Guid AttachmentId { get; set; } = Guid.Empty;

        /// <summary>
        /// 是否允许判分
        /// 关闭后，教师的判分通道也会随之关闭
        /// </summary>、
        public ExamMarkStatus MarkStatus { get; set; } = ExamMarkStatus.Open;

        public string PageConfig { get; set; }

        /// <summary>
        /// 是否强制修改
        /// </summary>
        public int IsForce { get; set; } = 0;

        public long SchemeId { get; set; }

        public int IsDependentAccess { get; set; } = 0;
    }
}
