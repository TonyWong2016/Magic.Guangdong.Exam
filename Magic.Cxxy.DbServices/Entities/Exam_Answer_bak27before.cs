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
	public partial class Exam_Answer_bak27before {

		[JsonProperty]
		public DateTime AddDate { get; set; }

		[JsonProperty]
		public double? AllScore { get; set; }

		[JsonProperty, Column(IsIdentity = true)]
		public int AnswerID { get; set; }

		[JsonProperty]
		public int AnswerType { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Badges { get; set; }

		[JsonProperty, Column(StringLength = 150)]
		public string Certificate { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string ExamEndDate { get; set; }

		[JsonProperty]
		public int ExamID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string ExamStartDate { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string ExamUseTime { get; set; }

		[JsonProperty]
		public int IsSubmit { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string MaxExamEndDate { get; set; }

		[JsonProperty]
		public int? Ori_UserID { get; set; }

		[JsonProperty]
		public int? SessionID { get; set; }

		[JsonProperty]
		public int? StepStatus1 { get; set; }

		[JsonProperty]
		public int? StepStatus2 { get; set; }

		[JsonProperty]
		public int? StepStatus3 { get; set; }

		[JsonProperty]
		public int? StepStatus4 { get; set; }

		[JsonProperty]
		public int UserID { get; set; }

	}

}
