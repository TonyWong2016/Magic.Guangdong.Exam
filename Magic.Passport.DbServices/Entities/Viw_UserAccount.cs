using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace Magic.Passport.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Viw_UserAccount {

		[JsonProperty, Column(StringLength = 200, IsNullable = false)]
		public string Account { get; set; }

		[JsonProperty]
		public int AccountID { get; set; }

		[JsonProperty]
		public int AccountTypeID { get; set; }

		[JsonProperty]
		public int? ActiveCode { get; set; }

		[JsonProperty]
		public bool? Deleted { get; set; }

		[JsonProperty]
		public bool? EnabledStatus { get; set; }

		[JsonProperty]
		public bool IsActive { get; set; }

		[JsonProperty]
		public DateTime? LastLoginDate { get; set; }

		[JsonProperty]
		public int? LoginCount { get; set; }

		[JsonProperty, Column(StringLength = 256)]
		public string PasswordKey { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string PrivateKey { get; set; }

		[JsonProperty]
		public int UID { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string UserName { get; set; }

		[JsonProperty]
		public Guid? UUID { get; set; }

	}

}