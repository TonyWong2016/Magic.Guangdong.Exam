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
	public partial class Course_ResourceLink {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int RCLinkID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? AddDate { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string CourseID { get; set; }

		[JsonProperty]
		public int ResourceID { get; set; }

	}

}
