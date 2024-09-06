using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	/// <summary>
	/// 课时记录表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Course_LessonRecord {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int ULRecordID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		[JsonProperty]
		public Guid CourseID { get; set; }

		/// <summary>
		/// 已学习的时间（秒）
		/// </summary>
		[JsonProperty]
		public double LearnTime { get; set; } = 0d;

		[JsonProperty]
		public Guid LessonID { get; set; }

		/// <summary>
		/// 课程期别ID
		/// </summary>
		[JsonProperty, Column(StringLength = 20)]
		public string ProcessID { get; set; }

		/// <summary>
		/// 0-未学完，1-已学完
		/// </summary>
		[JsonProperty]
		public int Status { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string UserID { get; set; }

	}

}
