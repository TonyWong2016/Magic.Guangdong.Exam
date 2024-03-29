using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Activity {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string Description { get; set; }

		[JsonProperty]
		public DateTime EndTime { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty]
		public DateTime StartTime { get; set; }

		[JsonProperty]
		public int Status { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(200)", IsNullable = false)]
		public string Title { get; set; } = "";

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

	}

}
