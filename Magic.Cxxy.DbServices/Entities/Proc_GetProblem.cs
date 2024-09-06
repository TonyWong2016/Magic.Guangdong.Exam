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
	public partial class Proc_GetProblem {

		[JsonProperty, Column(Name = "@ShowOrder")]
		public int ShowOrder { get; set; }

		[JsonProperty, Column(Name = "@TeamID")]
		public int TeamID { get; set; }

	}

}
