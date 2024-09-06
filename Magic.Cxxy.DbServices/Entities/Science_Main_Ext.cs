using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Science_Main_Ext {

		/// <summary>
		/// 用户OpenID 河北科技馆微信号
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string OpenID { get; set; }

		[JsonProperty]
		public int? Ori_SNMID { get; set; }

		/// <summary>
		/// 第一次答题分数
		/// </summary>
		[JsonProperty]
		public int Score1 { get; set; } = 0;

		/// <summary>
		/// 第二次答题分数
		/// </summary>
		[JsonProperty]
		public int Score2 { get; set; } = 0;

		/// <summary>
		/// Science_Main主表的主键ID
		/// </summary>
		[JsonProperty]
		public int SNMID { get; set; }

	}

}
