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
	public partial class Proc_SummaryExamScore {

		[JsonProperty, Column(Name = "@end")]
		public DateTime end { get; set; }

		[JsonProperty, Column(Name = "@exam_id")]
		public int exam_id { get; set; }

		[JsonProperty, Column(Name = "@flag")]
		public int flag { get; set; }

		[JsonProperty, Column(Name = "@start")]
		public DateTime start { get; set; }

	}

}
