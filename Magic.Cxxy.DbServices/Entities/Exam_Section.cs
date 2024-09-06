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
	/// 答题活动章节表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Exam_Section {

		/// <summary>
		/// 章节ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int SectionID { get; set; }

		/// <summary>
		/// 添加时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 添加人
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string AddUserID { get; set; }

		/// <summary>
		/// 答题活动ID
		/// </summary>
		[JsonProperty]
		public int? ExamID { get; set; }

		/// <summary>
		/// 章节排序
		/// </summary>
		[JsonProperty]
		public int? OrderIndex { get; set; }

		/// <summary>
		/// 分数 单个题目
		/// </summary>
		[JsonProperty]
		public double? Score { get; set; }

		/// <summary>
		/// 总分表 备用字段暂时不用
		/// </summary>
		[JsonProperty]
		public double? ScoreSum { get; set; }

		/// <summary>
		/// 章节名称
		/// </summary>
		[JsonProperty, Column(StringLength = 1000, IsNullable = false)]
		public string SectionTitle { get; set; }

	}

}
