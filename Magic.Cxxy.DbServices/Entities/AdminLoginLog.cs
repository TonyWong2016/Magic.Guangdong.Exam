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
	public partial class AdminLoginLog {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty]
		public Guid? AdminID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string AdminName { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string IP { get; set; }

	}

}
