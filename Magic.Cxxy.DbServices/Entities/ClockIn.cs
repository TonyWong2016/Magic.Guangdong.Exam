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
	public partial class ClockIn {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string LessonID { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string LiveID { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string LocationID { get; set; }

		[JsonProperty]
		public double? PlayTime { get; set; } = 0d;

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Remark { get; set; }

		[JsonProperty]
		public long? SeriesID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Updated_at { get; set; }

		[JsonProperty, Column(DbType = "varchar(30)")]
		public string UserID { get; set; }

	}

}
