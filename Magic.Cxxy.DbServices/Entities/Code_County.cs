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
	public partial class Code_County {

		/// <summary>
		/// 区县ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public int CountyID { get; set; }

		/// <summary>
		/// 城市ID
		/// </summary>
		[JsonProperty]
		public int CityID { get; set; }

		/// <summary>
		/// 区县名称
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string CountyName { get; set; }

		/// <summary>
		/// 显示状态 如果为1 则显示 如果为2 则不显示 默认为1
		/// </summary>
		[JsonProperty]
		public int? StatusID { get; set; } = 1;

	}

}
