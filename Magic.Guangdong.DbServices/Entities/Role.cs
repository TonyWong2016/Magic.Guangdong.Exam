using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Role {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Description { get; set; } = "无";

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Name { get; set; }

		/// <summary>
		/// 0-普通角色，1-系统级角色
		/// </summary>
		[JsonProperty]
		public int Type { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonProperty]
        public int IsDeleted { get; set; } = 0;
    }

}
