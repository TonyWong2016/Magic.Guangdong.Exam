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
	/// 课程大纲表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Course_Sections {

		/// <summary>
		/// 章节ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int SectionID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 添加人ID
		/// </summary>
		[JsonProperty, Column(StringLength = 20)]
		public string AddUserID { get; set; } = "无";

		/// <summary>
		/// 所属课程ID
		/// </summary>
		[JsonProperty]
		public Guid? CourseID { get; set; }

		/// <summary>
		/// 排序 正序排列
		/// </summary>
		[JsonProperty]
		public int OrderNum { get; set; } = 0;

		/// <summary>
		/// 章节全称
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string SectionCaption { get; set; }

		/// <summary>
		/// 章节简介
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string SectionIntro { get; set; }

		/// <summary>
		/// 章节简称 如第几章
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string SectionSubCaption { get; set; }

	}

}
