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
	public partial class VIW_CourseResourceLink {

		[JsonProperty]
		public DateTime AddDate { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string AddUserID { get; set; }

		[JsonProperty]
		public int CheckID { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string CourseID { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string PublicDate { get; set; }

		[JsonProperty]
		public int RCLinkID { get; set; }

		[JsonProperty]
		public int ResourceID { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string ResourceImgPath { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string ResourceSUbTitle { get; set; }

		[JsonProperty, Column(StringLength = 4000, IsNullable = false)]
		public string ResourceSummary { get; set; }

		[JsonProperty, Column(StringLength = 200, IsNullable = false)]
		public string ResourceTitle { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string ResourceType { get; set; }

		[JsonProperty]
		public int StatusID { get; set; }

	}

}
