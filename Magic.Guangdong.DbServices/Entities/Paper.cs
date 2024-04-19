using FreeSql.DataAnnotations;
using MassTransit;
using Newtonsoft.Json;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Paper {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = NewId.NextGuid();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; }

		/// <summary>
		/// 试卷的考试时间，默认继承examination的时间
		/// </summary>
		[JsonProperty]
		public double Duration { get; set; }

		[JsonProperty]
		public Guid ExamId { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 是否公开成绩查询，0-不公开，1-公开
		/// </summary>
		[JsonProperty]
		public int OpenResult { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(100)", IsNullable = false)]
		public string PaperDegree { get; set; } = "all";

		/// <summary>
		/// 0-机器组卷（考试之前先随机生成多套试卷，考试时随机分发，推荐采用此方式，考试前生成多套试卷既满足随机性，也可以对不同的试卷提前校验，避免疏漏，默认），1-人工组卷（严谨的场合下适用，比如统一考试等），2-即时组卷（每个学生考试时随机生成，不推荐正式考试用这种方式，无法对试卷进行校验）
		/// </summary>
		[JsonProperty]
		public PaperType PaperType { get; set; } = PaperType.Rule;

		/// <summary>
		/// 抽题情况
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(MAX)")]
		public string QuestionDetailJson { get; set; }

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string Remark { get; set; }

		/// <summary>
		/// 试卷的卷面总分，默认继承examination的基础分数
		/// </summary>
		[JsonProperty]
		public double Score { get; set; }

		[JsonProperty]
		public ExamStatus Status { get; set; } = ExamStatus.Enabled;

		[JsonProperty, Column(DbType = "varchar(150)")]
		public string Title { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

	}

	

	public enum PaperType {
		Rule,
		Custom,
		Practice,
		Other
	}

}
