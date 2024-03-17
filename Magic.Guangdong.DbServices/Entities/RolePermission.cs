using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class RolePermission {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty]
		public long PremissionId { get; set; }

		[JsonProperty]
		public long RoleId { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

        [JsonProperty]
        public int IsDeleted { get; set; } = 0;
    }

}
