using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Province {

		/// <summary>
		/// 省级行政区ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public int Id { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

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

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

	}

}
