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
	public partial class BlankQuestion {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// 填空题答案，管道符’|‘分开
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string Answer { get; set; }

		[JsonProperty]
		public int? AnswerCnt { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty]
		public Guid? ExamId { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string Remark { get; set; }

		[JsonProperty]
		public double Res { get; set; } = 0d;

		[JsonProperty, Column(StringLength = 500)]
		public string Subject { get; set; }

		[JsonProperty]
		public int Type { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

	}

}
