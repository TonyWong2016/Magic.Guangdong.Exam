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
	public partial class Subject {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// 专题封面
		/// </summary>
		[JsonProperty, Column(StringLength = 300)]
		public string Cover { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		/// <summary>
		/// 专题简介
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string Detail { get; set; }

		[JsonProperty]
		public Guid LocationID { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string Remark { get; set; }

		[JsonProperty]
		public int Status { get; set; } = 1;

		/// <summary>
		/// 0-普通专题，1-特殊专题（如人工智能，创新大赛等专题）
		/// </summary>
		[JsonProperty]
		public int SubjectType { get; set; } = 0;

		[JsonProperty, Column(StringLength = 100)]
		public string Title { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

	}

}
