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
	public partial class AppKey {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int id { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string appId { get; set; } = Guid.NewGuid().ToString().ToUpper();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime created_at { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string name { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string remark { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string secret { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime updated_at { get; set; }

	}

}
