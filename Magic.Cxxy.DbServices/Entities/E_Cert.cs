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
	public partial class E_Cert {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string CertName { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string CourseID { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string CourseTag { get; set; }

		[JsonProperty]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Location { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string Remark { get; set; }

		[JsonProperty]
		public int Template_id { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Title { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Uid { get; set; }

		[JsonProperty]
		public DateTime? Updated_at { get; set; }

	}

}
