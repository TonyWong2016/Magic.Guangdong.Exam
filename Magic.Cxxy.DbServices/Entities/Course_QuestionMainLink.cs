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
	public partial class Course_QuestionMainLink {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int QMLinkId { get; set; }

		[JsonProperty]
		public DateTime AddDate { get; set; }

		[JsonProperty]
		public int MainId { get; set; }

		[JsonProperty]
		public Guid QuestionId { get; set; }

	}

}
