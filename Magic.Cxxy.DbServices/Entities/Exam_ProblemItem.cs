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
	/// 问题选项表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Exam_ProblemItem {

		/// <summary>
		/// 问题选项ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int ProblemItemID { get; set; }

		/// <summary>
		/// 添加时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 显示形式 1图片 2文字
		/// </summary>
		[JsonProperty]
		public int? DisplayTyp { get; set; }

		/// <summary>
		/// 是否为正确答案
		/// </summary>
		[JsonProperty]
		public int? IsCorrectAnswer { get; set; }

		/// <summary>
		/// 选项内容
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string ItemContent { get; set; }

		/// <summary>
		/// 选项排序
		/// </summary>
		[JsonProperty]
		public int? ItemOrdexIndex { get; set; }

		/// <summary>
		/// 选项代码 A B C D
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string ItemText { get; set; }

		/// <summary>
		/// 图片地址
		/// </summary>
		[JsonProperty, Column(StringLength = 150)]
		public string PhotoUrl { get; set; }

		/// <summary>
		/// 问题ID
		/// </summary>
		[JsonProperty]
		public long ProblemID { get; set; }

	}

}
