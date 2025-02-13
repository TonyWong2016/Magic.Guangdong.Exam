﻿using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class UserAnswerRecordView {

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string AccountId { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string AssociationId { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string AssociationTitle { get; set; }

		[JsonProperty]
		public int? CardType { get; set; }

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

		[JsonProperty, Column(DbType = "varchar(150)")]
		public string Email { get; set; }

		[JsonProperty]
		public DateTime? EndTime { get; set; }

		[JsonProperty]
		public Guid ExamId { get; set; }

		[JsonProperty]
		public int? ExamStatus { get; set; }

		[JsonProperty, Column(DbType = "varchar(200)")]
		public string ExamTitle { get; set; }

		[JsonProperty]
		public int ExamType { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string GroupCode { get; set; }

		[JsonProperty]
		public long Id { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string IdCard { get; set; }

		[JsonProperty]
		public string SecurityIdCard { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)", IsNullable = false)]
		public string IdNumber { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; }

		[JsonProperty]
		public int? IsStrict { get; set; }

		[JsonProperty]
		public DateTime LimitedTime { get; set; }

		[JsonProperty]
		public int Marked { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Mobile { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Name { get; set; }

		[JsonProperty]
		public int OpenResult { get; set; }

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

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string ReportId { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ReportNumber { get; set; }

		[JsonProperty]
		public double Score { get; set; }

        [JsonProperty]
        public double ObjectiveScore { get; set; } = 0d;

        [JsonProperty]
        public double PaperScore { get; set; }

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
		public Guid AttachmentId { get; set; }

        [JsonProperty]
        public int MarkStatus { get; set; }

        [JsonProperty, Column(StringLength = 300)]
        public string ReportParticipants { get; set; }

        [JsonProperty, Column(StringLength = 100)]
        public string ReportProjectNo { get; set; }

        [JsonProperty, Column(StringLength = 500)]
        public string ReportSchools { get; set; }

        [JsonProperty, Column(StringLength = 300)]
        public string ReportTeachers { get; set; }

        [JsonProperty, Column(StringLength = 300)]
        public string ReportTeamName { get; set; }

        [JsonProperty, Column(StringLength = 300)]
        public string ReportGroupName { get; set; }

        [JsonProperty]
		public long TagId { get; set; } = 0;

        [JsonProperty]
        public int IncludeSubjective { get; set; } = 0;
    }

}
