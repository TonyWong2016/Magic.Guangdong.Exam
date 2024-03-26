using FreeSql.DataAnnotations;
using FreeSql.Internal;
using MassTransit;
using Newtonsoft.Json;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Admin {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = NewId.NextGuid();

		[JsonProperty]
		public string Password { get; set; }

		[JsonProperty]
		public string KeySecret { get; set; } = Assistant.Utils.GenerateRandomCodePro(32, 3);

        [JsonProperty]
        public string KeyId { get; set; } = Assistant.Utils.GenerateRandomCodePro(32, 3);

        [JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(150)", IsNullable = false)]
		public string Description { get; set; }

		[JsonProperty, Column(DbType = "varchar(150)", IsNullable = false)]
		public string Email { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Mobile { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Name { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

        [JsonProperty]
        public int IsDeleted { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string? NickName { get; set; } = "";

		[JsonProperty]
		public string Version { get; set; }

        [JsonProperty]
        public int Status { get; set; } = 0;
    }

}
