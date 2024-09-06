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
	/// 题目分类表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Exam_ProblemSort {

		/// <summary>
		/// 分类ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int SortID { get; set; }

		/// <summary>
		/// 父ID
		/// </summary>
		[JsonProperty]
		public int? ParentID { get; set; }

		/// <summary>
		/// 显示排序
		/// </summary>
		[JsonProperty]
		public int? ShowOrder { get; set; }

		/// <summary>
		/// 分类名称
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string SortCaption { get; set; }

		/// <summary>
		/// 答题还是问卷（1答题 2问卷）默认1
		/// </summary>
		[JsonProperty]
		public int Type { get; set; } = 1;

	}

}
