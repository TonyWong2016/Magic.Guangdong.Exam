using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class SubjectSeason {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// 按钮的背景色
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(10)")]
		public string ButtenColor { get; set; }

		/// <summary>
		/// 封面
		/// </summary>
		[JsonProperty, Column(StringLength = 400)]
		public string Cover { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		/// <summary>
		/// 详情
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string Detail { get; set; }

		[JsonProperty]
		public int OrderNum { get; set; } = 0;

		[JsonProperty, Column(StringLength = 100)]
		public string Remark { get; set; }

		[JsonProperty]
		public int Status { get; set; } = 1;

		[JsonProperty]
		public Guid SubjectId { get; set; }

		/// <summary>
		/// 0-普通，1-特殊
		/// </summary>
		[JsonProperty]
		public int SubjectType { get; set; } = 0;

		[JsonProperty, Column(StringLength = 50)]
		public string SubTitle { get; set; }

		/// <summary>
		/// 缩略图
		/// </summary>
		[JsonProperty, Column(StringLength = 400)]
		public string ThumbCover { get; set; }

		/// <summary>
		/// 季标题
		/// </summary>
		[JsonProperty, Column(StringLength = 20)]
		public string Title { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

	}

}
