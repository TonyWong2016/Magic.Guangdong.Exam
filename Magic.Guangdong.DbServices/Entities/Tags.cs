﻿using FreeSql.DatabaseModel;using System;
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
	public partial class Tags {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

        [JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? CreatedAt { get; set; } = DateTime.Now;

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string Title { get; set; } = "标签标题";

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    }

}
