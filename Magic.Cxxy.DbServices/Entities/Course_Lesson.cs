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
	/// 课时表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Course_Lesson {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid LessonID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string AddUserID { get; set; }

		/// <summary>
		/// 课时ID 如果为空则为章节 如果不为空则为课时
		/// </summary>
		[JsonProperty]
		public Guid? AttachID { get; set; }

		[JsonProperty]
		public Guid? CourseID { get; set; }

		/// <summary>
		/// 标记该记录是否删除，0-显示，1-隐藏（标记删除）。暂时不建议直接删除记录，因为课时下关联的数据包含学生提交的笔记和问答，如果删掉课时则这些信息也需要删掉，对用户来说这是不符合规范的，因为用户的笔记内容需要有用户来管理，所以如果一定要删除的话，就打上一个标记吧
		/// </summary>
		[JsonProperty]
		public int IsRemove { get; set; } = 0;

		[JsonProperty]
		public int OrderNum { get; set; } = 0;

		[JsonProperty]
		public int ProcessID { get; set; } = 0;

		[JsonProperty]
		public int QaCnt { get; set; } = 0;

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string SectionCaption { get; set; }

		[JsonProperty]
		public int? SectionID { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string SectionIntro { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string SectionSubCaption { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string TimeSpan { get; set; }

	}

}
