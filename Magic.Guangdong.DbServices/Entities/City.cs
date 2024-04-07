using FreeSql.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class City {

		/// <summary>
		/// 城区ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public int Id { get; set; }
		
		/// <summary>
		/// 城区名称
		/// </summary>
		[JsonProperty, Column(StringLength = 15, IsNullable = false)]
		public string CityName { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 所属省级行政区ID
		/// </summary>
		[JsonProperty]
		public int ProvinceId { get; set; }

		[JsonProperty]
		public int? Status { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

	}

}
