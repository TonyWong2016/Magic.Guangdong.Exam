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
	public partial class Proc_PageCourseInfoNew {

		[JsonProperty, Column(Name = "@from", IsNullable = false)]
		public string from { get; set; }

		[JsonProperty, Column(Name = "@order", IsNullable = false)]
		public string order { get; set; }

		[JsonProperty, Column(Name = "@pageindex")]
		public int pageindex { get; set; }

		[JsonProperty, Column(Name = "@pagesize")]
		public int pagesize { get; set; }

		[JsonProperty, Column(Name = "@where", StringLength = 300, IsNullable = false)]
		public string where { get; set; }

	}

}
