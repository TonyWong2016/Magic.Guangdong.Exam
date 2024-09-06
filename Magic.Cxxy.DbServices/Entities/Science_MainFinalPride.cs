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
	/// 奖项记录表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Science_MainFinalPride {

		/// <summary>
		/// 逐渐ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int PrideLogID { get; set; }

		/// <summary>
		/// 添加时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 用户OpenID
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string OpenID { get; set; }

		/// <summary>
		/// 奖项ID 对应 Science_MainPrideItem
		/// </summary>
		[JsonProperty]
		public int PrideItemID { get; set; }

	}

}
