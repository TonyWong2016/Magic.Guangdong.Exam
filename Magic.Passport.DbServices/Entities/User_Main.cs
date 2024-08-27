using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace Magic.Passport.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class User_Main {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int UID { get; set; }

		[JsonProperty]
		public int? AvatarID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string ClientID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? CreateDate { get; set; }

		[JsonProperty]
		public int? DefaultAccountID { get; set; } = 0;

		[JsonProperty]
		public bool Deleted { get; set; } = false;

		[JsonProperty, Column(StringLength = 100)]
		public string Email { get; set; }

		[JsonProperty]
		public bool EnabledStatus { get; set; } = true;

		[JsonProperty]
		public DateTime? LastLoginDate { get; set; }

		[JsonProperty]
		public int LoginCount { get; set; } = 0;

		[JsonProperty, Column(StringLength = 100)]
		public string Mobile { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string NickName { get; set; }

		[JsonProperty, Column(StringLength = 256, IsNullable = false)]
		public string PasswordKey { get; set; }

		[JsonProperty, Column(StringLength = 100, IsNullable = false)]
		public string PrivateKey { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string RegisterRoot { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string RegisterUrl { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string UserName { get; set; }

		[JsonProperty]
		public Guid UUID { get; set; } = Guid.NewGuid();

	}

}
