using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class VIW_ExamMain {

		[JsonProperty]
		public DateTime AddDate { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string AddUserID { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string AnswerContent { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string CorrectAnswer { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string DataMarks { get; set; }

		[JsonProperty]
		public double? Difficulty { get; set; }

		[JsonProperty]
		public int? DisplayType { get; set; }

		[JsonProperty]
		public DateTime? EndTime { get; set; }

		[JsonProperty]
		public int ERLinkID { get; set; }

		[JsonProperty]
		public int ExamID { get; set; }

		[JsonProperty, Column(StringLength = 4000)]
		public string ExamIntro { get; set; }

		[JsonProperty]
		public int? ExamLongs { get; set; }

		[JsonProperty, Column(StringLength = 150, IsNullable = false)]
		public string ExamName { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string ExamScoreSum { get; set; }

		[JsonProperty]
		public int HasSubProblem { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string HostUnit { get; set; }

		[JsonProperty]
		public int IsLimited { get; set; }

		[JsonProperty]
		public int? IsPublic { get; set; }

		[JsonProperty]
		public int? KnowledgeID { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string LimitedRules { get; set; }

		[JsonProperty]
		public int MainStatus { get; set; }

		[JsonProperty]
		public int ParentID { get; set; }

		[JsonProperty, Column(StringLength = 550)]
		public string PhotoUrl { get; set; }

		[JsonProperty]
		public long? ProblemID { get; set; }

		[JsonProperty, Column(StringLength = 300)]
		public string ProblemName { get; set; }

		[JsonProperty]
		public int? ProblemType { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string RangeID { get; set; }

		[JsonProperty]
		public double? Score { get; set; }

		[JsonProperty]
		public int? SessionID { get; set; }

		[JsonProperty, Column(StringLength = 10, IsNullable = false)]
		public string SessionYear { get; set; }

		[JsonProperty]
		public int? ShowOrder { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string SortCaption { get; set; }

		[JsonProperty]
		public int? SortID { get; set; }

		[JsonProperty]
		public DateTime? StartTime { get; set; }

		[JsonProperty]
		public int Status { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string SubjectCodeID { get; set; }

		[JsonProperty]
		public int TeamID { get; set; }

	}

}
