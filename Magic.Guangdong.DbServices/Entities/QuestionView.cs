using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class QuestionView {

		[JsonProperty, Column(DbType = "varchar(MAX)")]
		public string Analysis { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Author { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ColumnId { get; set; }

		[JsonProperty]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; }

		[JsonProperty, Column(DbType = "varchar(20)", IsNullable = false)]
		public string Degree { get; set; }

		[JsonProperty]
		public long Id { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; }

		[JsonProperty]
		public int IsOpen { get; set; }

		[JsonProperty]
		public int? Objective { get; set; }

		[JsonProperty, Column(DbType = "varchar(150)")]
		public string Remark { get; set; }

		[JsonProperty]
		public double Score { get; set; }

		[JsonProperty]
		public int? SingleAnswer { get; set; }

		[JsonProperty]
		public Guid SubjectId { get; set; }

		[JsonProperty, Column(DbType = "varchar(150)")]
		public string SubjectName { get; set; }

		[JsonProperty, Column(DbType = "varchar(MAX)")]
		public string Title { get; set; }

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string TitleText { get; set; }

		[JsonProperty]
		public Guid TypeId { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string TypeName { get; set; }

		[JsonProperty]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

	}

}
