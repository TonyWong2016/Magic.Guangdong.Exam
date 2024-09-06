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
	public partial class QAs {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[JsonProperty]
		public Guid? CourseID { get; set; }

		[JsonProperty]
		public Guid? LessonID { get; set; }

		[JsonProperty, Column(DbType = "text")]
		public string Answer { get; set; }

		/// <summary>
		/// 问题的金币数
		/// </summary>
		[JsonProperty]
		public int? Coin { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty]
		public Guid? LiveID { get; set; }

		/// <summary>
		/// 对应的问题，当type为2时，该字段有值
		/// </summary>
		[JsonProperty]
		public Guid? ParentID { get; set; }

		[JsonProperty]
		public int Praise { get; set; } = 0;

		[JsonProperty, Column(DbType = "text")]
		public string Question { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Title { get; set; }

		/// <summary>
		/// 1-问题，2-答案
		/// </summary>
		[JsonProperty]
		public int Type { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string UserID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string UserName { get; set; }

	}

}
