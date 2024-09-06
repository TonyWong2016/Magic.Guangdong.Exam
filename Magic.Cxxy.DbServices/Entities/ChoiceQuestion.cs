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
	public partial class ChoiceQuestion {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[JsonProperty]
		public Guid? ExamId { get; set; }

		/// <summary>
		/// 答案
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Answer { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string OptionA { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string OptionB { get; set; }

		/// <summary>
		/// 选择题至少要有两个选项，所以A，B不可为空，C，D，E可以为空，不设定C，D，E选项的话就是只有AB选项
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string OptionC { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string OptionD { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string OptionE { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string Remark { get; set; }

		/// <summary>
		/// 分数
		/// </summary>
		[JsonProperty]
		public double Res { get; set; } = 0d;

		[JsonProperty, Column(StringLength = 500, IsNullable = false)]
		public string Subject { get; set; }

		/// <summary>
		/// 1-单选，2-多选
		/// </summary>
		[JsonProperty]
		public int? Type { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

	}

}
