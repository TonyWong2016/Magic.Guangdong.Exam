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
	public partial class E_CertAttributes {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		[JsonProperty]
		public int? Color_b { get; set; } = 0;

		[JsonProperty]
		public int? Color_g { get; set; } = 0;

		[JsonProperty]
		public int? Color_r { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Font_family { get; set; }

		[JsonProperty]
		public int? Font_size { get; set; }

		[JsonProperty]
		public int? Location_x { get; set; }

		[JsonProperty]
		public int? Location_y { get; set; }

		/// <summary>
		/// 1-姓名，2-编号，3-日期，4-名称，5-其他
		/// </summary>
		[JsonProperty]
		public int? OrderIndex { get; set; } = 1;

		[JsonProperty]
		public int Template_id { get; set; }

		/// <summary>
		/// 模板名称
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string Template_name { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Updated_at { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Words { get; set; }

	}

}
