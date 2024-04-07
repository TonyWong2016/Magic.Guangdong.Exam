using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Guangdong.DbServices.Entities {

	/// <summary>
	/// 区县表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class District {

		/// <summary>
		/// 区县ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public int Id { get; set; }

		/// <summary>
		/// 城市ID
		/// </summary>
		[JsonProperty]
		public int CityId { get; set; }

		/// <summary>
		/// 区县名称
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string DistrictName { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 显示状态 如果为1 则显示 如果为2 则不显示 默认为1
		/// </summary>
		[JsonProperty]
		public int? Status { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

	}

}
