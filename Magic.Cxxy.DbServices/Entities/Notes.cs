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
	public partial class Notes {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[JsonProperty]
		public Guid? CourseID { get; set; }

		[JsonProperty]
		public Guid? LessonID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		/// <summary>
		/// 笔记详情
		/// </summary>
		[JsonProperty, Column(DbType = "text")]
		public string Detail { get; set; }

		/// <summary>
		/// 点赞数
		/// </summary>
		[JsonProperty]
		public int Praise { get; set; } = 0;

		/// <summary>
		/// 标星数
		/// </summary>
		[JsonProperty]
		public int Star { get; set; } = 0;

		/// <summary>
		/// 1-私有笔记，2-共享笔记
		/// </summary>
		[JsonProperty]
		public int? Type { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string UserID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string UserName { get; set; }

		/// <summary>
		/// 浏览数
		/// </summary>
		[JsonProperty]
		public int Watch { get; set; } = 0;

	}

}
