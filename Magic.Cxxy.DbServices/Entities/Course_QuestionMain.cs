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
	public partial class Course_QuestionMain {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int MainId { get; set; }

		[JsonProperty]
		public DateTime AddDate { get; set; }

		[JsonProperty]
		public int AddUserID { get; set; }

		[JsonProperty]
		public DateTime EndTime { get; set; }

		[JsonProperty, Column(StringLength = 4000, IsNullable = false)]
		public string Intro { get; set; }

		[JsonProperty, Column(StringLength = 200, IsNullable = false)]
		public string Name { get; set; }

		[JsonProperty, Column(StringLength = 10, IsNullable = false)]
		public string SessionYear { get; set; }

		[JsonProperty]
		public DateTime StartTime { get; set; }

		[JsonProperty]
		public int Status { get; set; }

	}

}
