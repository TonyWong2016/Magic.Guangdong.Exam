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
	public partial class UserViewRecord {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty]
		public int? Count { get; set; } = 1;

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string CourseID { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseName { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string LessonID { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string LiveID { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string LocationID { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string LocationName { get; set; }

		/// <summary>
		/// 0-普通课程，1-直播课程
		/// </summary>
		[JsonProperty]
		public int? Type { get; set; } = 0;

		[JsonProperty, Column(StringLength = 20)]
		public string UID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Updated_at { get; set; }

		/// <summary>
		/// 浏览时间（时间戳）
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string View_at { get; set; }

	}

}
