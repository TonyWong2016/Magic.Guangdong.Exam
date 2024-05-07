using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class TeacherExamAssignView {

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string AssociationId { get; set; }

		[JsonProperty]
		public Guid? AttachmentId { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Email { get; set; }

		[JsonProperty]
		public DateTime ExamEndTime { get; set; }

		[JsonProperty]
		public Guid ExamId { get; set; }

		[JsonProperty]
		public double ExamLimitTime { get; set; }

		[JsonProperty]
		public DateTime ExamStartTime { get; set; }

		[JsonProperty]
		public int ExamStatus { get; set; }

		[JsonProperty, Column(DbType = "varchar(200)", IsNullable = false)]
		public string ExamTitle { get; set; }

		[JsonProperty]
		public int ExamType { get; set; }

		[JsonProperty]
		public long Id { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Mobile { get; set; }

		[JsonProperty]
		public Guid TeacherId { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string TeacherName { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string TeachNo { get; set; }

	}

}
