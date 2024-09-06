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
	public partial class Course_QuestionUserAnswer {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid AnswerID { get; set; }

		[JsonProperty]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 提交来源（1PC，2手机）默认1
		/// </summary>
		[JsonProperty]
		public int AnswerType { get; set; } = 1;

		[JsonProperty]
		public Guid ItemID { get; set; }

		[JsonProperty, Column(StringLength = 200, IsNullable = false)]
		public string ItemRemark { get; set; }

		[JsonProperty, Column(StringLength = 10, IsNullable = false)]
		public string ItemText { get; set; }

		[JsonProperty]
		public Guid QuestionID { get; set; }

		[JsonProperty]
		public int QuestionMainID { get; set; }

		[JsonProperty]
		public long SeriesID { get; set; }

		[JsonProperty, Column(DbType = "varchar(20)", IsNullable = false)]
		public string UserID { get; set; }

	}

}
