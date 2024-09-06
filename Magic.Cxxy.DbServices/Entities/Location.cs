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
	public partial class Location {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid LocationId { get; set; } = Guid.NewGuid();

		/// <summary>
		/// 简称，如江苏是js或者jiangsu，视情况而定
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(200)")]
		public string Abbreviation { get; set; }

		/// <summary>
		/// 背景图地址（如果有的话）
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string Background { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		/// <summary>
		/// 不同地区对应的css地址（半自动配置）
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string CssPath { get; set; }

		[JsonProperty]
		public int? DisplayRule { get; set; } = 0;

		[JsonProperty]
		public int IsRemove { get; set; } = 0;

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string Name { get; set; }

		/// <summary>
		/// 根域名地址，如江苏stem中心的根域名是：www.jsstem.org
		/// </summary>
		[JsonProperty, Column(StringLength = 200)]
		public string ParentHost { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string Remark { get; set; }

		[JsonProperty]
		public int? Status { get; set; } = 0;

		/// <summary>
		/// 0-省份，1-市，2-其他
		/// </summary>
		[JsonProperty]
		public int Type { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Updated_at { get; set; }

	}

}
