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
	public partial class Course_Question {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid QuestionID { get; set; }

		[JsonProperty]
		public DateTime AddDate { get; set; }

		[JsonProperty]
		public int AddUserID { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string PhotoUrl { get; set; }

		[JsonProperty, Column(StringLength = 200, IsNullable = false)]
		public string QuestionName { get; set; }

		[JsonProperty]
		public int Required { get; set; } = 1;

		[JsonProperty]
		public int SortID { get; set; }

		[JsonProperty]
		public int Type { get; set; }

	}

}
