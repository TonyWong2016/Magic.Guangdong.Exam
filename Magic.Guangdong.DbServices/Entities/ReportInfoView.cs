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
	public partial class ReportInfoView {

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string AccountId { get; set; }

		[JsonProperty]
		public long ActivityId { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string Address { get; set; }

		[JsonProperty]
		public int CardType { get; set; }

		[JsonProperty]
		public int CityId { get; set; }

		[JsonProperty, Column(StringLength = 15, IsNullable = false)]
		public string CityName { get; set; }

		[JsonProperty]
		public int ConnAvailable { get; set; }

		[JsonProperty]
		public DateTime CreatedAt { get; set; }

		[JsonProperty]
		public int? DistrictId { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string DistrictName { get; set; }

		[JsonProperty, Column(DbType = "varchar(150)", IsNullable = false)]
		public string Email { get; set; }

		[JsonProperty]
		public Guid? ExamId { get; set; }

		[JsonProperty]
		public long Id { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string IdCard { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string Job { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Mobile { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string Name { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string OtherInfo { get; set; }

		[JsonProperty]
		public int ProvinceId { get; set; }

		[JsonProperty, Column(StringLength = 15, IsNullable = false)]
		public string ProvinceName { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ReportNumber { get; set; }

	}

}
