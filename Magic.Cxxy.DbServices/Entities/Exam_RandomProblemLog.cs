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
	/// 用户答题随机题目表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Exam_RandomProblemLog {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int ID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		[JsonProperty]
		public int? AnswerID { get; set; }

		/// <summary>
		/// 答题次数（1，2，3，4，5，6....）
		/// </summary>
		[JsonProperty]
		public int AnswerNum { get; set; } = 1;

		/// <summary>
		/// 答题活动主表ID（默认是2，湖北答题活动ID，湖北答题活动没有这个字段）
		/// </summary>
		[JsonProperty]
		public int ExamID { get; set; } = 2;

		[JsonProperty]
		public int? Ori_StudentApplyID { get; set; }

		[JsonProperty, Column(StringLength = 1000, IsNullable = false)]
		public string problemID { get; set; }

		[JsonProperty]
		public int? SectionID { get; set; }

		[JsonProperty]
		public int? SessionID { get; set; }

		[JsonProperty]
		public int? StudentApplyID { get; set; }

	}

}
