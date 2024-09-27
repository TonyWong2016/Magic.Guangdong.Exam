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

		[JsonProperty, Column(StringLength = 15)]
		public string CityName { get; set; }

		[JsonProperty]
		public int ConnAvailable { get; set; }

		[JsonProperty]
		public DateTime CreatedAt { get; set; }

		[JsonProperty]
		public int? DistrictId { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string DistrictName { get; set; }

		[JsonProperty, Column(DbType = "varchar(150)", IsNullable = false)]
		public string Email { get; set; }

		[JsonProperty]
		public Guid? ExamId { get; set; }

		[JsonProperty]
		public long FileId { get; set; }

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

		[JsonProperty]
		public Guid? OrderId { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string OtherInfo { get; set; }

		[JsonProperty]
		public int ProvinceId { get; set; }

		[JsonProperty, Column(StringLength = 20)]
		public string ProvinceName { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ReportNumber { get; set; }

		[JsonProperty]
		public int ReportStatus { get; set; }

		[JsonProperty]
		public int ReportStep { get; set; }

        /// <summary>
        /// 参与测试的次数
        /// 0-没参加
        /// 1-参加了一次考试/测试，考试一般只有1次
        /// >1-参加了多次考试/测试，测试一般有多次
        /// </summary>
        [JsonProperty]
        public int TestedTime { get; set; }

        /// <summary>
        /// 照片地址
        /// </summary>
        [JsonProperty, Column(StringLength = 350)]
        public string Photo { get; set; }

		[JsonProperty]
		public string SecurityIdCard { get; set; }

		[JsonProperty]
		public string SecurityMobile {  get; set; }
        [JsonProperty]
        public string HashIdCard { get; set; }
        [JsonProperty]
        public string HashMobile { get; set; }

		[JsonProperty]
		public int IsSecurity { get; set; }

        [JsonProperty]
        public string SuffixIdCard { get; set; }
        [JsonProperty]
        public string SuffixMobile { get; set; }
    }

}
