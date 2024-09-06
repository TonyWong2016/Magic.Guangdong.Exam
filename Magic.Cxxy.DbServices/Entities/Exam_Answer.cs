using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	/// <summary>
	/// 用户答题记录表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Exam_Answer {

		/// <summary>
		/// 主键ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int AnswerID { get; set; }

		/// <summary>
		/// 添加时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 总分
		/// </summary>
		[JsonProperty]
		public double? AllScore { get; set; }

		/// <summary>
		/// 用户答题类型 1为PC官网 2为手机端 如果从官网开始答题 那么手机端不能同时开始答题
		/// </summary>
		[JsonProperty]
		public int AnswerType { get; set; }

		/// <summary>
		/// 徽章
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string Badges { get; set; }

		/// <summary>
		/// 证书
		/// </summary>
		[JsonProperty, Column(StringLength = 150)]
		public string Certificate { get; set; }

		/// <summary>
		/// 答题完成时间
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string ExamEndDate { get; set; }

		/// <summary>
		/// 答题活动ID Exam_Main
		/// </summary>
		[JsonProperty]
		public int ExamID { get; set; }

		/// <summary>
		/// 答题开始时间 默认为空 未开始答题  用于记录用户开始答题时间
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string ExamStartDate { get; set; } = "";

		/// <summary>
		/// 答题用时 精确到毫秒
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string ExamUseTime { get; set; }

		/// <summary>
		/// 是否提交答题 0:未提交 1：手动提交 2 自动提交
		/// </summary>
		[JsonProperty]
		public int IsSubmit { get; set; }

		/// <summary>
		/// 最大提交提交，=开始时间+最大答题时间
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string MaxExamEndDate { get; set; }

		[JsonProperty]
		public int? Ori_UserID { get; set; }

		/// <summary>
		/// 届次ID
		/// </summary>
		[JsonProperty]
		public int? SessionID { get; set; }

		/// <summary>
		/// 是否阅读规则 1为是 默认为0 未阅读
		/// </summary>
		[JsonProperty]
		public int? StepStatus1 { get; set; } = 0;

		/// <summary>
		/// 是否完成学习 1为是
		/// </summary>
		[JsonProperty]
		public int? StepStatus2 { get; set; } = 0;

		/// <summary>
		/// 是否答题 1为是
		/// </summary>
		[JsonProperty]
		public int? StepStatus3 { get; set; } = 0;

		/// <summary>
		/// 是否获得证书
		/// </summary>
		[JsonProperty]
		public int? StepStatus4 { get; set; } = 0;

		/// <summary>
		/// 用户ID
		/// </summary>
		[JsonProperty]
		public int UserID { get; set; }

	}

}
