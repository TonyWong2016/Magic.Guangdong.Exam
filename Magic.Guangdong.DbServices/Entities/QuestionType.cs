using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class QuestionType {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string Caption { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; }

		[JsonProperty]
		public int? IsDeleted { get; set; } = 0;

		/// <summary>
		/// 是否是客观题，即有标准答案
		/// </summary>
		[JsonProperty]
		public int Objective { get; set; } = 1;

		[JsonProperty, Column(DbType = "varchar(200)")]
		public string Remark { get; set; }

		/// <summary>
		/// 只有1个答案，1-是，其他-不是
		/// </summary>
		[JsonProperty]
		public int? SingleAnswer { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

	}

}
