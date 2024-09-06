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
	public partial class E_CertPath {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string AwardName { get; set; }

		/// <summary>
		/// 证书内容
		/// </summary>
		[JsonProperty, Column(StringLength = 2000)]
		public string CertContent { get; set; }

		/// <summary>
		/// 证书编号
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string CertNum { get; set; }

		[JsonProperty]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string Path { get; set; }

		/// <summary>
		/// 证书内容（存储json字符串，如{"type":"创意发明","school":"霍格沃兹"}，属性自定义，按需解析使用）
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(100)")]
		public string Remark { get; set; }

		/// <summary>
		/// 1-正常显示，2-删除
		/// </summary>
		[JsonProperty]
		public int Status { get; set; } = 1;

		[JsonProperty]
		public int? Template_id { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Title { get; set; }

		/// <summary>
		/// 1-jpg,2-pdf
		/// </summary>
		[JsonProperty]
		public int Type { get; set; } = 1;

		[JsonProperty]
		public int? Uid { get; set; }

		[JsonProperty]
		public DateTime? Updated_at { get; set; }

	}

}
