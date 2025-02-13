﻿using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	/// <summary>
	/// 科学之夜用户红包领取记录表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Science_MainMoneyRecord {

		/// <summary>
		/// 红包领取记录ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int RecordID { get; set; }

		/// <summary>
		/// 领取时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 默认为分 100 为1元
		/// </summary>
		[JsonProperty]
		public int MoneySum { get; set; }

		/// <summary>
		/// 1为 答题红包1 2为答题红包2 3为投票红包
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string MoneyType { get; set; }

		/// <summary>
		/// 用户OpenID
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string OpenID { get; set; }

	}

}
