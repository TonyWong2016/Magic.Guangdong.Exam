﻿using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class UserBase {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string Address { get; set; } = "";

		[JsonProperty]
		public int Age { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)", IsNullable = false)]
		public string Email { get; set; } = "";

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string IdCard { get; set; } = "";

		[JsonProperty, Column(StringLength = 200)]
		public string Job { get; set; } = "";

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Mobile { get; set; } = "";

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string Name { get; set; } = "";

		[JsonProperty]
		public int? ProvinceCode { get; set; } = 0;

		[JsonProperty]
		public int Sex { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

	}

}