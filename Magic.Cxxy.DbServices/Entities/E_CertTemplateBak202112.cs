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
	public partial class E_CertTemplateBak202112 {

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string Attributes { get; set; }

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string AttributesPdf { get; set; }

		[JsonProperty]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(IsIdentity = true)]
		public int Id { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string Path { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Remark { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Title { get; set; }

	}

}
