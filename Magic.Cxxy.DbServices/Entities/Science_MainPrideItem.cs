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
	/// 奖项表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Science_MainPrideItem {

		/// <summary>
		/// 奖项ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int PrideItemID { get; set; }

		[JsonProperty]
		public int? CompleteCount { get; set; }

		/// <summary>
		/// 排序ID
		/// </summary>
		[JsonProperty]
		public int? OrderIndex { get; set; }

		/// <summary>
		/// 奖项名称
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string PrideCapton { get; set; }

		[JsonProperty]
		public int? PrideCount { get; set; }

		/// <summary>
		/// 奖项简介
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string PrideIntro { get; set; }

	}

}
