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
	/// 活动票选记录表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Science_MainActivityVote {

		/// <summary>
		/// 投票记录iD
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int VodeLogID { get; set; }

		/// <summary>
		/// 用户OpenID
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string OpenID { get; set; }

		/// <summary>
		/// 投票时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime VodeTime { get; set; }

		/// <summary>
		/// 投票选项id 用竖线隔开 |
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string VoteItem { get; set; }

		/// <summary>
		/// 三项投票名称1
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string VoteItem1 { get; set; }

		/// <summary>
		/// 三项投票名称2
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string VoteItem2 { get; set; }

		/// <summary>
		/// 三项投票名称3
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string VoteItem3 { get; set; }

	}

}
