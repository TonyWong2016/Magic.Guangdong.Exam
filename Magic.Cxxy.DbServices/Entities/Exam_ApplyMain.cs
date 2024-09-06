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
	public partial class Exam_ApplyMain {

		[JsonProperty]
		public int? EAID { get; set; }

		[JsonProperty]
		public int? ExamID { get; set; }

		[JsonProperty]
		public int? StepStatus1 { get; set; }

		[JsonProperty]
		public int? StepStatus2 { get; set; }

		[JsonProperty]
		public int? StepStatus3 { get; set; }

		[JsonProperty]
		public int? StepStatus4 { get; set; }

		[JsonProperty]
		public int? UID { get; set; }

	}

}
