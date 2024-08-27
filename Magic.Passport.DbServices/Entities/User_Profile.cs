using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace Magic.Passport.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class User_Profile {

		[JsonProperty, Column(IsPrimary = true)]
		public int UID { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string AllergicSource { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Anamnesis { get; set; }

		[JsonProperty, Column(StringLength = 10)]
		public string AreaCode { get; set; }

		[JsonProperty]
		public int? AvatarID { get; set; }

		[JsonProperty, Column(DbType = "date")]
		public DateTime? Birthday { get; set; }

		[JsonProperty]
		public int? BloodTypeID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Class { get; set; }

		[JsonProperty]
		public int? ClothSizeID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Department { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Duty { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Email { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Fax { get; set; }

		[JsonProperty]
		public int? Grade { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string GraduationSchoolName { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string HealthCondition { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string IdentityNo { get; set; }

		[JsonProperty]
		public int? IdentityTypeID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string IMFacebook { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string IMLINE { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string IMQQ { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string IMTwitter { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string IMWeibo { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string IMWeixin { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Interest { get; set; }

		/// <summary>
		/// 过敏源 1无 2有
		/// </summary>
		[JsonProperty]
		public int? IsAllergic { get; set; }

		/// <summary>
		/// 健康状况 1健康 2急慢性疾病
		/// </summary>
		[JsonProperty]
		public int? IsHealthy { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string MealRequest { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Mobile { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Name { get; set; }

		[JsonProperty]
		public int? NationID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string NickName { get; set; }

		[JsonProperty]
		public int? OccupationID { get; set; }

		[JsonProperty]
		public int? PhotoID { get; set; }

		[JsonProperty]
		public int? PoliticalLandscapeID { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string PostAddress { get; set; }

		[JsonProperty, Column(StringLength = 10)]
		public string PostCode { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string ProfileNo { get; set; }

		[JsonProperty]
		public int? ProSerialID { get; set; }

		[JsonProperty]
		public int? ProTitleID { get; set; }

		[JsonProperty]
		public int? QualificationID { get; set; }

		[JsonProperty]
		public int? SchoolCityID { get; set; }

		[JsonProperty]
		public int? SchoolCountyID { get; set; }

		[JsonProperty]
		public int? SchoolID { get; set; }

		[JsonProperty]
		public int? SchoolLength { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string SchoolName { get; set; }

		[JsonProperty]
		public int? SchoolProvinceID { get; set; }

		[JsonProperty]
		public int? SexID { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string SocialAward { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string SocialDuty { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Specialty { get; set; }

		[JsonProperty]
		public int? StudentDutyID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string StudentNo { get; set; }

		[JsonProperty]
		public int? StudentQualificationID { get; set; }

		[JsonProperty]
		public bool? Sub1Submit { get; set; } = false;

		[JsonProperty]
		public DateTime? Sub1SubmitDate { get; set; }

		[JsonProperty]
		public bool? Sub2Submit { get; set; } = false;

		[JsonProperty]
		public DateTime? Sub2SubmitDate { get; set; }

		[JsonProperty]
		public bool? Sub3Submit { get; set; } = false;

		[JsonProperty]
		public DateTime? Sub3SubmitDate { get; set; }

		[JsonProperty]
		public bool? Sub4Submit { get; set; } = false;

		[JsonProperty]
		public DateTime? Sub4SubmitDate { get; set; }

		[JsonProperty]
		public bool? Sub5Submit { get; set; } = false;

		[JsonProperty]
		public DateTime? Sub5SubmitDate { get; set; }

		[JsonProperty]
		public bool? Sub6Submit { get; set; } = false;

		[JsonProperty]
		public DateTime? Sub6SubmitDate { get; set; }

		[JsonProperty]
		public bool? Sub7Submit { get; set; } = false;

		[JsonProperty]
		public DateTime? Sub7SubmitDate { get; set; }

		[JsonProperty]
		public bool? Sub8Submit { get; set; } = false;

		[JsonProperty]
		public DateTime? Sub8SubmitDate { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Subject { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string SubjectClass { get; set; }

		[JsonProperty, Column(StringLength = 1000)]
		public string Summary { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string TelePhone { get; set; }

		[JsonProperty]
		public int? UnitCityID { get; set; }

		[JsonProperty]
		public int? UnitCountyID { get; set; }

		[JsonProperty]
		public int? UnitID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string UnitName { get; set; }

		[JsonProperty]
		public int? UnitProvinceID { get; set; }

		[JsonProperty]
		public int? ValidateCheckStatus { get; set; }

		[JsonProperty]
		public int? ValidateID { get; set; }

		[JsonProperty]
		public bool? ValidateSubmit { get; set; } = false;

	}

}
