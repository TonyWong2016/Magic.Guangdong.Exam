﻿using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Proc_StatisticRecord {

		[JsonProperty, Column(Name = "@end")]
		public DateTime end { get; set; }

		[JsonProperty, Column(Name = "@from_sql", DbType = "varchar(1000)", IsNullable = false)]
		public string from_sql { get; set; }

		[JsonProperty, Column(Name = "@now")]
		public DateTime now { get; set; }

		[JsonProperty, Column(Name = "@start")]
		public DateTime start { get; set; }

		[JsonProperty, Column(Name = "@year")]
		public int year { get; set; }

	}

}
