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
	public partial class Code_City {

		/// <summary>
		/// 城区ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public int CityID { get; set; }

		/// <summary>
		/// 城区名称
		/// </summary>
		[JsonProperty, Column(StringLength = 15, IsNullable = false)]
		public string CityName { get; set; }

		/// <summary>
		/// 所属省级行政区ID
		/// </summary>
		[JsonProperty]
		public int ProvinceID { get; set; }

		[JsonProperty]
		public int? StatusID { get; set; } = 1;

	}

}
