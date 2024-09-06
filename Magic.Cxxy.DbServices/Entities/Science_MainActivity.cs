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
	/// 科学之夜活动表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Science_MainActivity {

		/// <summary>
		/// 活动ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int ActivityID { get; set; }

		/// <summary>
		/// 活动简介
		/// </summary>
		[JsonProperty, Column(StringLength = 1500)]
		public string ActivityIntro { get; set; }

		/// <summary>
		/// 活动标题
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string ActivityTitle { get; set; }

		/// <summary>
		/// 活动届次ID 默认为0
		/// </summary>
		[JsonProperty]
		public int? SessionID { get; set; } = 0;

	}

}
