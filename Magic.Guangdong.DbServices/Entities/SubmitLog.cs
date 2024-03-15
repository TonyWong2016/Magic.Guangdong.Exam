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
	public partial class SubmitLog {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty]
		public int ComplatedMode { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(MAX)")]
		public string SubmitAnswer { get; set; } = "";

		[JsonProperty]
		public long Urid { get; set; }

	}

}
