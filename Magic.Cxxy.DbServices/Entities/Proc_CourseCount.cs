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
	public partial class Proc_CourseCount {

		[JsonProperty, Column(Name = "@DisplayRule")]
		public int DisplayRule { get; set; }

		[JsonProperty, Column(Name = "@LocationID")]
		public Guid LocationID { get; set; }

	}

}
