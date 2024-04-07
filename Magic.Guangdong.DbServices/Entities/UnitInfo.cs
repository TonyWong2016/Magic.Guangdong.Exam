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
	public partial class UnitInfo {

		/// <summary>
		/// 机构ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		/// <summary>
		/// 添加日期
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改日期
        /// </summary>
        [JsonProperty, Column(InsertValueSql = "getdate()")]
        public DateTime UpdatedAt { get; set; }=DateTime.Now;

		/// <summary>
		/// 创建人
		/// </summary>
		[JsonProperty]
		public string CreatedBy { get; set; } = "";
        /// <summary>
        /// 机构地址
        /// </summary>
        [JsonProperty, Column(StringLength = 50)]
		public string Address { get; set; }

		[JsonProperty]
		public int? CheckStatus { get; set; } = 2;

		/// <summary>
		/// 所在地区
		/// </summary>
		[JsonProperty]
		public int CityId { get; set; } = 0;

		[JsonProperty]
		public int DistrictId { get; set; } = 0;

		/// <summary>
		/// 传真
		/// </summary>
		[JsonProperty, Column(StringLength = 20)]
		public string Fax { get; set; }

		/// <summary>
		/// 0是未删除 1为已删除
		/// </summary>
		[JsonProperty]
		public int? IsDeleted { get; set; } = 0;

		/// <summary>
		/// 组织机构代码
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string OrganizationCode { get; set; } = "";

		/// <summary>
		/// 邮政编码
		/// </summary>
		[JsonProperty, Column(StringLength = 6)]
		public string PostCode { get; set; }

		[JsonProperty]
		public int ProvinceId { get; set; } = 0;

		/// <summary>
		/// 机构状态
		/// </summary>
		[JsonProperty]
		public int Status { get; set; } = 1;

		/// <summary>
		/// 电话号码
		/// </summary>
		[JsonProperty, Column(StringLength = 20)]
		public string Telephone { get; set; }

		/// <summary>
		/// 机构名称
		/// </summary>
		[JsonProperty, Column(StringLength = 100, IsNullable = false)]
		public string UnitCaption { get; set; }

		[JsonProperty, Column(StringLength = 2000)]
		public string UnitIntroduction { get; set; }

		/// <summary>
		/// 机构编号
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string UnitNo { get; set; }

		/// <summary>
		/// 机构类型
		/// </summary>
		[JsonProperty]
		public int UnitType { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string UnitUrl { get; set; }

	}

}
