using FreeSql.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class AdminLoginLog {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty]
		public Guid AdminId { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonProperty]
		public DateTime LoginTime { get; set; } = DateTime.Now;

        [JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string TokenVersion { get; set; } = Assistant.Utils.GenerateRandomCodePro(8);

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }=DateTime.Now;

		[JsonProperty]
		public string TokenHash { get; set; }

	}

}
