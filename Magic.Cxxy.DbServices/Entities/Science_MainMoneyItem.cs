using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	/// <summary>
	/// 红包奖池表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Science_MainMoneyItem {

		/// <summary>
		/// 红包ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int MoneyID { get; set; }

		/// <summary>
		/// 默认为分 1元是 100
		/// </summary>
		[JsonProperty]
		public int MoneySum { get; set; }

		/// <summary>
		/// 是否启用 1为启用 2为禁用
		/// </summary>
		[JsonProperty]
		public int StatusID { get; set; } = 1;

	}

}
