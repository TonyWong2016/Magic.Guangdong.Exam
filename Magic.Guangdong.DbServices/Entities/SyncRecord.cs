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

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class SyncRecord {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[JsonProperty]
		public long DataAmount { get; set; } = 0;

		[JsonProperty, Column(StringLength = 200)]
		public string DestModel { get; set; }

		[JsonProperty, Column(StringLength = 200, IsNullable = false)]
		public string Platform { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string TargetModel { get; set; }

		[JsonProperty]
		public int Times { get; set; } = 1;

		[JsonProperty, Column(StringLength = 500)]
		public string Usage { get; set; }

	}

}
