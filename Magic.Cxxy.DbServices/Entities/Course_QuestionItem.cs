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
	public partial class Course_QuestionItem {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid ItemID { get; set; }

		[JsonProperty]
		public DateTime AddDate { get; set; }

		[JsonProperty, Column(StringLength = 10, IsNullable = false)]
		public string ItemCode { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string ItemContent { get; set; }

		[JsonProperty]
		public Guid QuestionID { get; set; }

		[JsonProperty]
		public int SortID { get; set; }

	}

}
