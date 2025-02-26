using FreeSql.DataAnnotations;
using MassTransit;
using Newtonsoft.Json;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Examination {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = NewId.NextGuid();

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string AssociationId { get; set; } = "0";

		[JsonProperty, Column(DbType = "varchar(100)", IsNullable = false)]
		public string AssociationTitle { get; set; } = "无";

		/// <summary>
		/// 标准考试时长，单位：分钟，若生成得试卷没有指定时长，则沿用该时长
		/// </summary>
		[JsonProperty]
		public double BaseDuration { get; set; } = 15d;

		/// <summary>
		/// 标准总分数，若试卷未指定总分，则沿用该分数
		/// </summary>
		[JsonProperty]
		public double BaseScore { get; set; } = 100d;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }=DateTime.Now;

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; }

		[JsonProperty, Column(DbType = "nvarchar(1500)")]
		public string Description { get; set; }

        [JsonProperty, Column(StringLength = 500)]
        public string Address { get; set; }

        [JsonProperty]
		public DateTime EndTime { get; set; }

		[JsonProperty]
		public ExamType ExamType { get; set; } = ExamType.Examination;

		[JsonProperty, Column(DbType = "nvarchar(2000)")]
		public string ExtraInfo { get; set; }

		/// <summary>
		/// 聚合码，设置之后，可以生成聚合多个考试的二维码
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string GroupCode { get; set; } = "";

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 是否严格交卷，是的话超时提交将不给分，否的话答考试结束后有多少分给多少分，1-是，0-否
		/// </summary>
		[JsonProperty]
		public int IsStrict { get; set; } = 0;

		[JsonProperty]
		public int OrderIndex { get; set; } = 1;

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string Remark { get; set; }

		[JsonProperty]
		public DateTime StartTime { get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		[JsonProperty]
		public ExamStatus Status { get; set; } = ExamStatus.Enabled;

		[JsonProperty, Column(DbType = "varchar(200)", IsNullable = false)]
		public string Title { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

        /// <summary>
        /// 活动报名的名额
        /// </summary>
        [JsonProperty]
        public int Quota { get; set; } = 0;

        /// <summary>
        /// 费用
		/// 注意，这个最小单位要控制到分！不能再小。
        /// </summary>
        [JsonProperty, Column(DbType = "money")]
        public decimal Expenses { get; set; } = 0;

		/// <summary>
		/// 报名考试是否需要审核资格
		/// </summary>
		[JsonProperty]
		public ExamAudit Audit { get; set; } = ExamAudit.Yes;

		/// <summary>
		/// 依附的考试id
		/// 只有当考试类型为练习时，该值可以不为空，
		/// 不为空时表示当前练习依附的正式考试id，用户报名时，报名信息可一键带入
		/// </summary>
		[JsonProperty]
		public Guid AttachmentId { get; set; } = Guid.Empty;

		/// <summary>
		/// 是否允许判分
		/// 关闭后，教师的判分通道也会随之关闭
		/// </summary>
        [JsonProperty]
        public ExamMarkStatus MarkStatus { get; set; } = ExamMarkStatus.Open;

		/// <summary>
		/// 考试页面配置
		/// </summary>
		[JsonProperty]
		public string PageConfig { get; set; }

		/// <summary>
		/// 是否允许独立访问
		/// </summary>
		[JsonProperty]
		public int IndependentAccess { get; set; } = 0;

		/// <summary>
		/// 绑定的评分标准
		/// </summary>
		[JsonProperty]
		public long SchemeId { get; set; } = 0;

		/// <summary>
		/// 是否需要登录
		/// </summary>
		[JsonProperty]
		public int LoginRequired { get; set; } = 1;


        /// <summary>
        /// 是否绑定了监控规则
        /// </summary>
        [JsonProperty]
        public long MonitorRuleId { get; set; } = 0;

    }

	public enum ExamStatus
	{
		Enabled,
		Disabled
	}
	public enum ExamAudit
	{
        Yes = 1,
        No = 2		
	}

	public enum ExamType
	{
		Examination,
		Practice
	}

    public enum ExamMarkStatus
    {
        Open,
        Closed
    }

}
