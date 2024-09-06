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
	public partial class Admin_Permission {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int ID { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ActionName { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ControllerName { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string Description { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Title { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

	}

}
