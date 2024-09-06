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
	public partial class JudgeQuestion {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; }

		[JsonProperty]
		public Guid? ExamId { get; set; }

		/// <summary>
		/// 0-错误，1-正确
		/// </summary>
		[JsonProperty]
		public int Answer { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string Remark { get; set; }

		[JsonProperty]
		public double Res { get; set; } = 0d;

		[JsonProperty, Column(StringLength = 200, IsNullable = false)]
		public string Subject { get; set; }

		[JsonProperty]
		public int? Type { get; set; } = 3;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

	}

}
