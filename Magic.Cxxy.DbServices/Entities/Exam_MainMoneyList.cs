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
	public partial class Exam_MainMoneyList {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int MoneySetID { get; set; }

		[JsonProperty]
		public int? ExamID { get; set; }

		[JsonProperty]
		public int? IsDone { get; set; } = 0;

		[JsonProperty]
		public int? Money0Count { get; set; }

		[JsonProperty]
		public int? Money1Count { get; set; }

		[JsonProperty]
		public int? Money2Count { get; set; }

		[JsonProperty]
		public int? Money3Count { get; set; }

		[JsonProperty]
		public int? Money4Count { get; set; }

		[JsonProperty]
		public int? Money5Count { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string SetDay { get; set; }

	}

}
