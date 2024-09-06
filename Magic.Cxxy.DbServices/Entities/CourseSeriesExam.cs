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
	public partial class CourseSeriesExam {

		[JsonProperty]
		public DateTime? Created_at { get; set; }

		[JsonProperty]
		public int ExamID { get; set; }

		[JsonProperty]
		public long Id { get; set; }

		[JsonProperty]
		public long SeriesID { get; set; }

	}

}
