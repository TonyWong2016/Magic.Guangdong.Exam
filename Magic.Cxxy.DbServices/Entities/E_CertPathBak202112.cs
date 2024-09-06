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
	public partial class E_CertPathBak202112 {

		[JsonProperty, Column(StringLength = 50)]
		public string AwardName { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string CertContent { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string CertNum { get; set; }

		[JsonProperty]
		public DateTime? Created_at { get; set; }

		[JsonProperty]
		public long Id { get; set; }

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string Path { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string Remark { get; set; }

		[JsonProperty]
		public int? Template_id { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Title { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Uid { get; set; }

		[JsonProperty]
		public DateTime? Updated_at { get; set; }

	}

}
