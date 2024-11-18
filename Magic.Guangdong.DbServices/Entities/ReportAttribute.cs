using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class ReportAttribute {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonProperty, Column(InsertValueSql = "getdate()")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty, Column(StringLength = 2000)]
		public string Other { get; set; }

		[JsonProperty, Column(StringLength = 300, IsNullable = false)]
		public string Participants { get; set; }

		[JsonProperty, Column(StringLength = 100, IsNullable = false)]
		public string ProjectNo { get; set; }

		[JsonProperty]
		public long ReportId { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string Schools { get; set; }

		[JsonProperty, Column(StringLength = 300)]
		public string Teachers { get; set; }

		[JsonProperty, Column(StringLength = 300, IsNullable = false)]
		public string TeamName { get; set; }

        [JsonProperty, Column(StringLength = 300, IsNullable = false)]
        public string GroupName { get; set; }
    }

}
