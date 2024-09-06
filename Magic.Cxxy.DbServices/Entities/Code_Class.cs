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
	public partial class Code_Class {

		/// <summary>
		/// 班级ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public int Code { get; set; }

		/// <summary>
		/// 班级名称
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string ClassName { get; set; } = "";

	}

}
