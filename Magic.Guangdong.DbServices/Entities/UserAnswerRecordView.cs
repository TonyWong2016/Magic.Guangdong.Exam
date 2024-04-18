using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class UserAnswerRecordView {

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string ReportId { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string AssociationId { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string AssociationTitle { get; set; }

		[JsonProperty]
		public int CheatCnt { get; set; }

		[JsonProperty]
		public int Complated { get; set; }

		[JsonProperty]
		public int ComplatedMode { get; set; }

		[JsonProperty]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; }

		[JsonProperty]
		public DateTime? EndTime { get; set; }

		[JsonProperty]
		public Guid ExamId { get; set; }

		[JsonProperty]
		public int? ExamStatus { get; set; }

		[JsonProperty, Column(DbType = "varchar(200)")]
		public string ExamTitle { get; set; }

		[JsonProperty]
		public int? ExamType { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string GroupCode { get; set; }

		[JsonProperty]
		public long Id { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string IdNumber { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; }

		[JsonProperty]
		public int? IsStrict { get; set; }

		[JsonProperty]
		public DateTime LimitedTime { get; set; }

		[JsonProperty]
		public int Marked { get; set; }

		[JsonProperty]
		public int? OpenResult { get; set; }

		[JsonProperty]
		public DateTime? PaperCreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string PaperDegree { get; set; }

		[JsonProperty]
		public double? PaperDuration { get; set; }

		[JsonProperty]
		public Guid PaperId { get; set; }

		[JsonProperty]
		public int? PaperStatus { get; set; }

		[JsonProperty, Column(DbType = "varchar(150)")]
		public string PaperTitle { get; set; }

		[JsonProperty]
		public int? PaperType { get; set; }

		[JsonProperty, Column(DbType = "varchar(1000)")]
		public string Remark { get; set; }

		[JsonProperty]
		public double Score { get; set; }

		[JsonProperty]
		public int Stage { get; set; }

		[JsonProperty]
		public DateTime? StartTime { get; set; }

		[JsonProperty, Column(DbType = "varchar(MAX)", IsNullable = false)]
		public string SubmitAnswer { get; set; }

		[JsonProperty]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

		[JsonProperty]
		public double UsedTime { get; set; }

		[JsonProperty]
        public string AccountId { get; set; }

        [JsonProperty, Column(DbType = "varchar(150)")]
		public string UserName { get; set; }

		[JsonProperty]
		public int IdType { get; set; }

	}

}
