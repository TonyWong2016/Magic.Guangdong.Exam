﻿using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class ReportInfo {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty]
		public string AccountId { get; set; }

		[JsonProperty]
		public long ActivityId { get; set; } = 0;

		[JsonProperty]
		public int CityId { get; set; } = 0;

        [JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[JsonProperty]
		public int? DistrictId { get; set; } = 0;

        [JsonProperty, Column(DbType = "varchar(50)")]
		public string Email { get; set; } = string.Empty;

		[JsonProperty]
		public Guid? ExamId { get; set; } = Guid.Empty;

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string IdCard { get; set; } = string.Empty;

        [JsonProperty, Column(StringLength = 500)]
		public string Job { get; set; } = string.Empty;

        [JsonProperty, Column(DbType = "varchar(500)")]
		public string Mobile { get; set; } = string.Empty;

        [JsonProperty, Column(StringLength = 50)]
		public string Name { get; set; } = string.Empty;

        [JsonProperty, Column(StringLength = 500)]
		public string OtherInfo { get; set; } = string.Empty;

        [JsonProperty]
		public int ProvinceId { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonProperty]
        public long UnitId { get; set; } = 0;

		[JsonProperty,Column(StringLength = 200)]
		public string Address { get; set; } = "无";

		[JsonProperty]
		public string ReportNumber { get; set; } = string.Empty;

		public CardType CardType { get; set; } = CardType.China;

		/// <summary>
		/// 联系方式可用程度
		/// 0-未验证
		/// 1-邮箱验证可用
		/// 2-手机号可用
		/// 3-邮箱和手机号都可用
		/// </summary>
		[JsonProperty]
		public ConnAvailable ConnAvailable { get; set; } = ConnAvailable.None;

		[JsonProperty]
		public long FileId { get; set; } = 0;

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 照片地址
		/// </summary>
		[JsonProperty, Column(StringLength = 350)]
		public string Photo { get; set; }

        [JsonProperty]
        public string PrefixIdcard { get; set; }
        [JsonProperty]
        public string SuffixIdcard { get; set; }
        [JsonProperty]
        public string PrefixMobile { get; set; }
        [JsonProperty]
        public string SuffixMobile { get; set; }

        [JsonProperty]
        public int PhoneType { get; set; } = 0;

        [JsonProperty]
        public string HashIdcard { get; set; }
        [JsonProperty]
        public string HashMobile { get; set; }

		[JsonProperty]
		public int IsSecurity { get; set; } = 0;
        [JsonProperty]
        public long TagId { get; set; } = 0;
    }

	public enum CardType
	{
		China,
		HongKong,
		Macao,
		Taiwan,
		Passport,
		Other
	}

	public enum ConnAvailable
	{
		None,
		Email,
		Mobile,
		EmailMobile
	}

}
