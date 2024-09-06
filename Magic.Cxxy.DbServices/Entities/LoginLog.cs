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
	public partial class LoginLog {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		[JsonProperty]
		public Guid? AccountID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Login_at { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Remark { get; set; } = "老版本(江苏在用)";

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Uid { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Version { get; set; } = "1.0";

	}

}
