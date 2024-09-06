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
	public partial class CourseSeriesDetail {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty]
		public Guid? CourseID { get; set; }

		[JsonProperty]
		public DateTime? Created_at { get; set; }

		[JsonProperty]
		public Guid? LessonID { get; set; }

		[JsonProperty]
		public Guid? LiveID { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string LocationID { get; set; }

		[JsonProperty]
		public long? SeriesID { get; set; }

		[JsonProperty]
		public int? Type { get; set; }

		[JsonProperty]
		public DateTime? Updated_at { get; set; }

	}

}
