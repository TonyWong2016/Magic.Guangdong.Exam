using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Menu {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[JsonProperty]
		public int Depth { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(200)", IsNullable = false)]
		public string Description { get; set; }

		[JsonProperty]
        public Guid CreatorId { get; set; }

		/// <summary>
		/// 状态
		/// 0-正常，1-不可用
		/// </summary>
		[JsonProperty]
		public int Status { get; set; } = 1;

        [JsonProperty]
		public int IsLeef { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Name { get; set; } = "";

		[JsonProperty]
		public long ParentId { get; set; } = 0;

		[JsonProperty]
		public long PermissionId { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(200)", IsNullable = false)]
		public string Router { get; set; } = "";

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonProperty]
        public int IsDeleted { get; set; } = 0;

	}

}
