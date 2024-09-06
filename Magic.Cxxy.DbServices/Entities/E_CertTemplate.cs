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
	public partial class E_CertTemplate {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 生成图片格式的属性规格json字符串
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(5000)")]
		public string Attributes { get; set; }

		/// <summary>
		/// 生成pdf格式的属性规格json字符串
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(2000)")]
		public string AttributesPdf { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		/// <summary>
		/// 模板路径
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string Path { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Remark { get; set; }

		/// <summary>
		/// 模板名称
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string Title { get; set; }

	}

}
