using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Permission {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[JsonProperty, Column(DbType = "varchar(500)", IsNullable = false)]
		public string DataFilterJson { get; set; } = "";

		[JsonProperty, Column(DbType = "varchar(200)")]
		public string Description { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Name { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonProperty]
        public int IsDeleted { get; set; } = 0;

		[JsonProperty]
		public long ParentId { get; set; } = 0;

		[JsonProperty]
		public int Depth { get; set; } = 0;

		[JsonProperty]
		public int Type { get; set; } = 0;

		[JsonProperty]
        public string Controller { get; set; }

        [JsonProperty]
        public string Action { get; set; }

        [JsonProperty]
        public string Area { get; set; }

        [JsonProperty]
        public int Status { get; set; } = 0;
    }

}
