using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace Magic.Passport.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class User_Account {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int AccountID { get; set; }

		[JsonProperty, Column(StringLength = 200, IsNullable = false)]
		public string Account { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string AccountName { get; set; }

		[JsonProperty]
		public int AccountTypeID { get; set; }

		[JsonProperty]
		public bool IsActive { get; set; } = false;

		[JsonProperty, Column(StringLength = 500)]
		public string RegisterUrl { get; set; }

		[JsonProperty]
		public int UID { get; set; }

	}

}
