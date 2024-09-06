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
	public partial class Code_Province {

		/// <summary>
		/// 省级行政区ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public int ProvinceID { get; set; }

		/// <summary>
		/// 行政区编码
		/// </summary>
		[JsonProperty, Column(StringLength = 2, IsNullable = false)]
		public string ProvinceCode { get; set; }

		/// <summary>
		/// 行政区名称
		/// </summary>
		[JsonProperty, Column(StringLength = 20, IsNullable = false)]
		public string ProvinceName { get; set; }

		/// <summary>
		/// 行政区短称
		/// </summary>
		[JsonProperty, Column(StringLength = 15, IsNullable = false)]
		public string ProvinceShortName { get; set; }

	}

}
