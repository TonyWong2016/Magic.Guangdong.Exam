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
	public partial class Score {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; }

		[JsonProperty]
		public Guid? CourseId { get; set; }

		[JsonProperty]
		public Guid ExamId { get; set; }

		/// <summary>
		/// 填空题答案，用管道符'|'分隔
		/// </summary>
		[JsonProperty, Column(StringLength = -2)]
		public string AnswerBlankStr { get; set; }

		/// <summary>
		/// 选择题答案，用管道符'|'分隔
		/// </summary>
		[JsonProperty, Column(StringLength = -2)]
		public string AnswerChoiceStr { get; set; }

		/// <summary>
		/// 问答题答案，用管道符'|'分隔
		/// </summary>
		[JsonProperty, Column(StringLength = -2)]
		public string AnswerEssayStr { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty, Column(StringLength = 300)]
		public string Remark { get; set; }

		/// <summary>
		/// 填空题得分
		/// </summary>
		[JsonProperty]
		public double ResBlank { get; set; } = 0d;

		/// <summary>
		/// 选择题得分
		/// </summary>
		[JsonProperty]
		public double ResChoice { get; set; } = 0d;

		/// <summary>
		/// 问答题得分
		/// </summary>
		[JsonProperty]
		public double ResEssay { get; set; } = 0d;

		/// <summary>
		/// 判断题得分（暂时废弃，用单选题替换）
		/// </summary>
		[JsonProperty]
		public double ResJudge { get; set; } = 0d;

		[JsonProperty]
		public double ResTotal { get; set; } = 0d;

		/// <summary>
		/// 1-初审成绩，2-复审成绩
		/// </summary>
		[JsonProperty]
		public int? Status { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string UserId { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string UserName { get; set; }

	}

}
