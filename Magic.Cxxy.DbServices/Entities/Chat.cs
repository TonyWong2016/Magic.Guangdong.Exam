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
	public partial class Chat {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[JsonProperty, Column(StringLength = 100)]
		public string Address { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Grade { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Mobile { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Name { get; set; }

		/// <summary>
		/// 进房口令，默认为123456（sha-256加密）
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(100)", IsNullable = false)]
		public string Password { get; set; } = "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=";

		/// <summary>
		/// 备注
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string Remark { get; set; }

		/// <summary>
		/// 角色名称，学生，专家，主持
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string Role { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string School { get; set; }

		[JsonProperty]
		public int Status { get; set; } = 0;

		/// <summary>
		/// 项目名称
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string Title { get; set; }

		/// <summary>
		/// 1-学生，2-专家，3-主持
		/// </summary>
		[JsonProperty]
		public int? Type { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Updated_at { get; set; }

	}

}
