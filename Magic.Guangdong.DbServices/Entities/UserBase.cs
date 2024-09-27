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

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class UserBase {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(StringLength = 200)]
		public string Address { get; set; } = "";

		[JsonProperty]
		public int Age { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)", IsNullable = false)]
		public string Email { get; set; } = "";

		[JsonProperty, Column(DbType = "varchar(500)", IsNullable = false)]
		public string IdCard { get; set; } = "";

		[JsonProperty, Column(StringLength = 200)]
		public string Job { get; set; } = "";

		[JsonProperty, Column(DbType = "varchar(500)", IsNullable = false)]
		public string Mobile { get; set; } = "";

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string Name { get; set; } = "";

		[JsonProperty]
		public int ProvinceId { get; set; } = 0;

        [JsonProperty]
        public int CityId { get; set; } = 0;

        [JsonProperty]
        public int DistrictId { get; set; } = 0;

        [JsonProperty]
		public int Sex { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
		[JsonProperty]
        public int IsDeleted { get; set; } = 0;

        [JsonProperty]
        public string Password { get; set; }

        [JsonProperty]
        public string KeySecret { get; set; } = Assistant.Utils.GenerateRandomCodePro(16, 3);

        [JsonProperty]
        public string KeyId { get; set; } = Assistant.Utils.GenerateRandomCodePro(16, 3);

        /// <summary>
        /// 账号可用等级，default-仅用户名密码，email-另支持邮箱，mobile-另支持手机，email,mobile-邮箱手机都支持
        /// </summary>
        [JsonProperty]
		public string AvailableLevel { get; set; } = "default";

		[JsonProperty]
		public string Avatar { get; set; } = "/images/avatar.png";

		/// <summary>
		/// 账号id
		/// </summary>
		[JsonProperty]
		public string AccountId { get; set; }

        [JsonProperty]
        public string PrefixIdcard { get; set; }
        [JsonProperty]
        public string SuffixIdcard { get; set; }
        [JsonProperty]
        public string PrefixMobile { get; set; }
        [JsonProperty]
        public string SuffixMobile { get; set; }

		[JsonProperty]
		public int CardType { get; set; } = 0;

		[JsonProperty]
		public int PhoneType { get; set;} = 0;

        [JsonProperty]
        public string HashIdcard { get; set; }
        [JsonProperty]
        public string HashMobile { get; set; }

		[JsonProperty]
		public int IsSecurity { get; set; }
    }

}
