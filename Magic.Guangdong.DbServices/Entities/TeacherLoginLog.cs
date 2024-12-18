﻿using FreeSql.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using Yitter.IdGenerator;
using MassTransit;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class TeacherLoginLog {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonProperty]
		public DateTime LoginTime { get; set; }=DateTime.Now;

		[JsonProperty]
        public Guid TeacherId { get; set; }

        [JsonProperty, Column(DbType = "varchar(50)")]
		public string TokenHash { get; set; }

		[JsonProperty, Column(DbType = "varchar(20)", IsNullable = false)]
		public string TokenVersion { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }

}
