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
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class UnitInfo {

		/// <summary>
		/// 机构ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

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
		public string? CreatedBy { get; set; } = "";
		/// <summary>
		/// 机构地址
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string Address { get; set; } = "";

        /// <summary>
        ///  企业经营状态 0-存续（默认） 1-在业 2-吊销 3-注销 4-迁出 5-迁入 6-停业 7-清算 8-其他
        /// </summary>
        [JsonProperty]
		public BusinessStatus UnitStatus { get; set; } = BusinessStatus.Ongoing;

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
		public string? Fax { get; set; } = "";

        /// <summary>
        /// 0是未删除 1为已删除
        /// </summary>
        [JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 组织机构代码
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string OrganizationCode { get; set; } = "";

		/// <summary>
		/// 邮政编码
		/// </summary>
		[JsonProperty, Column(StringLength = 6)]
		public string? PostCode { get; set; } = "";

        [JsonProperty]
		public int ProvinceId { get; set; } = 0;

		/// <summary>
		/// 数据状态
		/// </summary>
		[JsonProperty]
		public int Status { get; set; } = 1;

		/// <summary>
		/// 电话号码
		/// </summary>
		[JsonProperty, Column(StringLength = 20)]
		public string? Telephone { get; set; } = "";

        /// <summary>
        /// 机构名称
        /// </summary>
        [JsonProperty, Column(StringLength = 100, IsNullable = false)]
		public string UnitCaption { get; set; } = "";

        [JsonProperty, Column(StringLength = 2000)]
		public string? UnitIntroduction { get; set; } = "";

        /// <summary>
        /// 机构编号/注册号
        /// </summary>
        [JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string? UnitNo { get; set; } = "";

        /// <summary>
        /// 机构类型
        /// </summary>
        [JsonProperty]
		public UnitType UnitType { get; set; } = UnitType.Default;

		[JsonProperty, Column(StringLength = 100)]
		public string? UnitUrl { get; set; } = "";

        [JsonProperty, Column(StringLength = 50)]
        public string LegalPerson { get; set; } = "";

        /// <summary>
        /// 同步参数，指代system库中[Unit_Infomation]表的主键编号
        /// </summary>
        [JsonProperty]
        public long OriginNo { get; set; }

    }

    /// <summary>
	/// 单位类型
    /// SocialGroup	社会团体
	/// School	学校
	/// ScientificResearchInstitution	科研机构
	/// AgencyUnit	机关单位
	/// GovernmentAffiliatedInstitutions	事业单位
	/// OffCampusOrganization	校外机构
	/// Enterprises	企业
	/// Other	其他
    /// </summary>
    public enum UnitType
	{
        Default,
        SocialGroup,
		School,
		ScientificResearchInstitution,
		AgencyUnit,
        GovernmentAffiliatedInstitutions,
        OffCampusOrganization,
        Enterprises,
		Other,		
    }

    /// <summary>
    /// 企业经营状态
    /// 0-存续
    /// 1-在业
    /// 2-吊销
    /// 3-注销
    /// 4-迁出
    /// 5-迁入
    /// 6-停业
	/// 7-清算
	/// 8-其他
    /// </summary>
    public enum BusinessStatus
    {
        //[Description("存续")]
        //[EnumMember(Value = "存续")]
        Ongoing,

        //[Description("在业")]
        // [EnumMember(Value = "在业")]
        Operational,

        //  [Description("吊销")]
        // [EnumMember(Value = "吊销")]
        Revoked,

        //[Description("注销")]
        // [EnumMember(Value = "注销")]
        Cancelled,

        // [Description("迁出")]
        // [EnumMember(Value = "迁出")]
        MovedOut,

        //[Description("迁入")]
        // [EnumMember(Value = "迁入")]
        MovedIn,

        // [Description("停业")]
        //[EnumMember(Value = "停业")]
        Suspended,

        // [Description("清算")]
        // [EnumMember(Value = "清算")]
        Liquidation,

        //[Description("其他")]
        //[EnumMember(Value = "其他")]
        Other
    }
}
