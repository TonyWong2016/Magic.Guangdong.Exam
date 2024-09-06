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
	public partial class Course_QuestionLink {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int CQLinkID { get; set; }

		[JsonProperty]
		public DateTime AddDate { get; set; }

		[JsonProperty]
		public Guid CourseID { get; set; }

		[JsonProperty]
		public Guid LiveID { get; set; }

		[JsonProperty]
		public Guid QuestionID { get; set; }

		[JsonProperty]
		public int Status { get; set; }

	}

}
