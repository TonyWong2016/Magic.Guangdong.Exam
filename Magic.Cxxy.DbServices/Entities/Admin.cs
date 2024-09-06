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
	public partial class Admin {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid ID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Email { get; set; }

		[JsonProperty, Column(DbType = "varchar(20)")]
		public string Mobile { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Password { get; set; }

		[JsonProperty]
		public int? Role_Id { get; set; } = 1;

		[JsonProperty]
		public int Status { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string UserName { get; set; }

	}

}
