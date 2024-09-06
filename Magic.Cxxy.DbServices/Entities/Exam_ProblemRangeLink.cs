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
	/// 答题试题范围表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Exam_ProblemRangeLink {

		/// <summary>
		/// 答题问题链接ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int ERLinkID { get; set; }

		/// <summary>
		/// 添加时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 答题活动ID
		/// </summary>
		[JsonProperty]
		public int? ExamID { get; set; }

		/// <summary>
		/// 是否是公开题目 默认为0不公开 1为公开
		/// </summary>
		[JsonProperty]
		public int? IsPublic { get; set; }

		/// <summary>
		/// 问题ID
		/// </summary>
		[JsonProperty]
		public long? ProblemID { get; set; }

		[JsonProperty]
		public double? Score { get; set; } = 0d;

	}

}
