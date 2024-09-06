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
	/// 答题固定试卷
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Exam_ProblemSectionLink {

		/// <summary>
		/// 答题问题链接ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int LinkID { get; set; }

		/// <summary>
		/// 添加试卷
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 问题ID
		/// </summary>
		[JsonProperty]
		public int? ProblemID { get; set; }

		/// <summary>
		/// 问题序号
		/// </summary>
		[JsonProperty, Column(StringLength = 10)]
		public string ProblemNo { get; set; }

		/// <summary>
		/// 分数 冗余字段 章节中带有分数 适用于章节中不同分数题目
		/// </summary>
		[JsonProperty]
		public int? Score { get; set; }

		/// <summary>
		/// 章节ID 如无章节默认为0
		/// </summary>
		[JsonProperty]
		public int SectionID { get; set; }

	}

}
