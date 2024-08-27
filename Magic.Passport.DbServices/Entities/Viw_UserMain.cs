using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace Magic.Passport.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Viw_UserMain {

		[JsonProperty, Column(StringLength = -2)]
		public string Accounts { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Email { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string IdentityNo { get; set; }

		[JsonProperty]
		public int LoginCount { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Mobile { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Name { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string SchoolName { get; set; }

		[JsonProperty]
		public int? SchoolProvinceID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string TelePhone { get; set; }

		[JsonProperty]
		public int UID { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string UserName { get; set; }

	}

}
