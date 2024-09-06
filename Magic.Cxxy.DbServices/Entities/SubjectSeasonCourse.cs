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
	public partial class SubjectSeasonCourse {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = 0;

		[JsonProperty]
		public Guid CourseID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty]
		public Guid SeasonId { get; set; }

		[JsonProperty]
		public int Status { get; set; } = 1;

		[JsonProperty]
		public Guid SubjectId { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

	}

}
