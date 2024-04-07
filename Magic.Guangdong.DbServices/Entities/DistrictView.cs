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
	public partial class DistrictView {

		[JsonProperty]
		public int CityId { get; set; }

		[JsonProperty, Column(StringLength = 15, IsNullable = false)]
		public string CityName { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string DistrictName { get; set; }

		[JsonProperty]
		public DateTime CreatedAt { get; set; }

		[JsonProperty]
		public int Id { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; }

		[JsonProperty]
		public int ProvinceId { get; set; }

		[JsonProperty, Column(StringLength = 15, IsNullable = false)]
		public string ProvinceShortName { get; set; }

		[JsonProperty]
		public int? Status { get; set; }

		[JsonProperty]
		public DateTime UpdatedAt { get; set; }

	}

}
