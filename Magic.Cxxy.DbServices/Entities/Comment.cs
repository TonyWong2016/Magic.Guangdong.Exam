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
	public partial class Comment {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid CommentID { get; set; } = Guid.NewGuid();

		/// <summary>
		/// 难易程度，1-易，2-中，3-难
		/// </summary>
		[JsonProperty]
		public int ComplexityScore { get; set; } = 0;

		[JsonProperty]
		public Guid CourseID { get; set; } = Guid.NewGuid();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		/// <summary>
		/// 课程设计分数
		/// </summary>
		[JsonProperty]
		public double CurriculumDesignScore { get; set; } = 0d;

		[JsonProperty, Column(StringLength = 200)]
		public string Description { get; set; }

		/// <summary>
		/// 知识内容分数
		/// </summary>
		[JsonProperty]
		public double IntellectualScore { get; set; } = 0d;

		[JsonProperty]
		public Guid? LessonID { get; set; }

		/// <summary>
		/// 教师分数
		/// </summary>
		[JsonProperty]
		public double TeacherLevelScore { get; set; } = 0d;

		/// <summary>
		/// 教学效果得分
		/// </summary>
		[JsonProperty]
		public double TeachingEffectScore { get; set; } = 0d;

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string UID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

	}

}
