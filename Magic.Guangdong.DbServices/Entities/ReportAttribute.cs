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

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty, Column(StringLength = 1000)]
		public string Other { get; set; }

		[JsonProperty, Column(StringLength = 200, IsNullable = false)]
		public string Participants { get; set; }

		[JsonProperty, Column(StringLength = 100, IsNullable = false)]
		public string ProjectNo { get; set; }

		[JsonProperty]
		public long ReportId { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string School { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Teacher { get; set; }

		[JsonProperty, Column(StringLength = 200, IsNullable = false)]
		public string TeamName { get; set; }

	}

}
