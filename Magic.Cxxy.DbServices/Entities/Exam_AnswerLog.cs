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
	/// 用户答题记录详细表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Exam_AnswerLog {

		/// <summary>
		/// 主键id
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int LogID { get; set; }

		/// <summary>
		/// 添加时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 答题记录主表ID Exam_Answer
		/// </summary>
		[JsonProperty]
		public int AnswerID { get; set; }

		/// <summary>
		/// 第几次答题（1，2，3，4，5，6....）
		/// </summary>
		[JsonProperty]
		public int AnswerNum { get; set; } = 1;

		/// <summary>
		/// 答题考试ID（默认是2，湖北答题活动的ID，湖北答题活动没有这个字段）
		/// </summary>
		[JsonProperty]
		public int ExamID { get; set; } = 2;

		/// <summary>
		/// 是否正确
		/// </summary>
		[JsonProperty]
		public bool IsCorrect { get; set; }

		/// <summary>
		/// 选项ID
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string ItemIDs { get; set; }

		/// <summary>
		/// 选项代码记录 对应 Exam_ProblemItem 的 ItemText
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string ItemTexts { get; set; }

		[JsonProperty]
		public int? Ori_UserID { get; set; }

		/// <summary>
		/// 题目ID
		/// </summary>
		[JsonProperty]
		public long ProblemID { get; set; }

		[JsonProperty]
		public int Score { get; set; }

		/// <summary>
		/// 活动届次ID
		/// </summary>
		[JsonProperty]
		public int SessionID { get; set; }

		/// <summary>
		/// 用户ID
		/// </summary>
		[JsonProperty]
		public int UserID { get; set; }

	}

}
