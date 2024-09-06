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
	public partial class Accounts {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[JsonProperty, Column(StringLength = 100)]
		public string Address { get; set; }

		[JsonProperty, Column(DbType = "varchar(400)")]
		public string Avatar { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string City { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string District { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Email { get; set; }

		[JsonProperty, Column(StringLength = 32)]
		public string IdentityNo { get; set; }

		[JsonProperty]
		public int Is_bind { get; set; } = 0;

		/// <summary>
		/// 从原用户表里倒过来的数据，设定超级脑残
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string LoginAccounts { get; set; }

		[JsonProperty]
		public int LoginCount { get; set; } = 0;

		[JsonProperty, Column(StringLength = 20)]
		public string Mobile { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Name { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string Password { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string PrivateKey { get; set; }

		[JsonProperty, Column(StringLength = 20)]
		public string Province { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Remark { get; set; }

		/// <summary>
		/// 1-普通用户（学生）2-嘉宾，3-主持人
		/// </summary>
		[JsonProperty]
		public int RoleType { get; set; } = 1;

		[JsonProperty, Column(StringLength = 100)]
		public string School { get; set; }

		[JsonProperty, Column(StringLength = 30)]
		public string Telephone { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Uid { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Updated_at { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string UserName { get; set; }

	}

}
