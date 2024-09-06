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
	public partial class UserProfile {

		[JsonProperty, Column(IsPrimary = true, InsertValueSql = "abs(checksum(newid()))")]
		public long Id { get; set; }

		[JsonProperty, Column(DbType = "varchar(200)")]
		public string Address { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Birthday { get; set; }

		/// <summary>
		/// 城市
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string City { get; set; }

		[JsonProperty]
		public int? CityID { get; set; }

		/// <summary>
		/// 国家
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Country1 { get; set; } = "中国";

		/// <summary>
		/// 乡村
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Country2 { get; set; }

		[JsonProperty]
		public int? Country2ID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Duty { get; set; }

		[JsonProperty, Column(DbType = "varchar(150)")]
		public string Email { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string GoodAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string IdentityNo { get; set; }

		[JsonProperty, Column(DbType = "varchar(30)")]
		public string Mobile { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Name { get; set; }

		[JsonProperty, Column(DbType = "varchar(150)")]
		public string NickName { get; set; }

		/// <summary>
		/// 省份
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Province { get; set; }

		[JsonProperty]
		public int? ProvinceID { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string Remark { get; set; }

		[JsonProperty]
		public int? Sex { get; set; }

		[JsonProperty]
		public int Uid { get; set; }

		[JsonProperty]
		public int? UnitID { get; set; }

		[JsonProperty, Column(DbType = "varchar(200)")]
		public string UnitName { get; set; }

		[JsonProperty]
		public int? UnitType { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Updated_at { get; set; }

	}

}
