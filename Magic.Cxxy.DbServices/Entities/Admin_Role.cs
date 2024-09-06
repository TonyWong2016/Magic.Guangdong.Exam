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
	public partial class Admin_Role {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int ID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Create_at { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string Name { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Remark { get; set; }

		[JsonProperty]
		public int RoleType { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

	}

}
