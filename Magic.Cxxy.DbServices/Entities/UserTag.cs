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
	public partial class UserTag {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		/// <summary>
		/// 大标签（课程领域STEAM）
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(100)")]
		public string Tag { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Uid { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Updated_at { get; set; }

		[JsonProperty]
		public int ViewCnt { get; set; } = 0;

		[JsonProperty]
		public double Weights { get; set; } = 0d;

	}

}
