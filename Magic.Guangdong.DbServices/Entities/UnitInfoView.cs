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
	public partial class UnitInfoView {
        /// <summary>
        /// 机构ID
        /// </summary>
        [JsonProperty]
        public long Id { get; set; }

        [JsonProperty]
		public DateTime CreatedAt { get; set; }

        [JsonProperty]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty, Column(StringLength = 50)]
		public string Address { get; set; }

		[JsonProperty]
		public int UnitStatus { get; set; }

		[JsonProperty]
		public int CityId { get; set; }

		[JsonProperty, Column(StringLength = 15, IsNullable = false)]
		public string CityName { get; set; }

		[JsonProperty]
		public int DistrictId { get; set; }

		//[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		//public string DistrictName { get; set; }

		[JsonProperty, Column(StringLength = 20)]
		public string Fax { get; set; }

		[JsonProperty]
		public int? IsDeleted { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string OrganizationCode { get; set; }

		[JsonProperty, Column(StringLength = 6)]
		public string PostCode { get; set; }

		[JsonProperty]
		public int ProvinceId { get; set; }

		[JsonProperty, Column(StringLength = 15, IsNullable = false)]
		public string ProvinceShortName { get; set; }

		[JsonProperty]
		public int Status { get; set; }

		[JsonProperty, Column(StringLength = 20)]
		public string Telephone { get; set; }

		[JsonProperty, Column(StringLength = 100, IsNullable = false)]
		public string UnitCaption { get; set; }

		[JsonProperty, Column(StringLength = 2000)]
		public string UnitIntroduction { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string UnitNo { get; set; }

		[JsonProperty]
		public int UnitType { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string UnitUrl { get; set; }

        [JsonProperty, Column(StringLength = 50)]
        public string LegalPerson { get; set; } = "";

		[JsonProperty, Column(StringLength = 50)]
		public string OriginNo { get; set; } = "0";

    }

}
