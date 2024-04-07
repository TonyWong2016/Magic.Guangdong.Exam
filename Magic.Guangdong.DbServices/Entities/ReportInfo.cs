using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class ReportInfo {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty]
		public Guid AccountId { get; set; }

		[JsonProperty]
		public long? ActivityId { get; set; } = 0;

		[JsonProperty]
		public int CityId { get; set; } = 0;

        [JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[JsonProperty]
		public int DistrictId { get; set; } = 0;

        [JsonProperty, Column(DbType = "varchar(50)")]
		public string Email { get; set; } = string.Empty;

		[JsonProperty]
		public Guid? ExamId { get; set; } = Guid.Empty;

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string IdCard { get; set; } = string.Empty;

        [JsonProperty, Column(StringLength = 500)]
		public string Job { get; set; } = string.Empty;

        [JsonProperty, Column(DbType = "varchar(50)")]
		public string Mobile { get; set; } = string.Empty;

        [JsonProperty, Column(StringLength = 50)]
		public string Name { get; set; } = string.Empty;

        [JsonProperty, Column(StringLength = 500)]
		public string OtherInfo { get; set; } = string.Empty;

        [JsonProperty]
		public int ProvinceId { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonProperty]
        public long UnitId { get; set; } = 0;
    }

}
