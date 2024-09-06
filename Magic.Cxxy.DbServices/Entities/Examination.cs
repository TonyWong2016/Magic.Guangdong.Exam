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
	public partial class Examination {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid ExamId { get; set; }

		[JsonProperty]
		public Guid? CourseId { get; set; }

		[JsonProperty]
		public Guid? LessonID { get; set; }

		[JsonProperty]
		public int Cnt { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string Remark { get; set; }

		/// <summary>
		/// 0-下线，1-正常
		/// </summary>
		[JsonProperty]
		public int Status { get; set; } = 0;

		/// <summary>
		/// 考试时长，单位：分钟
		/// </summary>
		[JsonProperty]
		public int? TimeLimit { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Title { get; set; }

		/// <summary>
		/// 1-课程考试，2-课时测验
		/// </summary>
		[JsonProperty]
		public int Type { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

	}

}
