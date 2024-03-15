using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using MassTransit;

namespace Magic.Guangdong.DbServices.Entities
{

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Examination {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = NewId.NextGuid();

        [JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string AssociationId { get; set; } = "0";

		[JsonProperty, Column(DbType = "varchar(100)", IsNullable = false)]
		public string AssociationTitle { get; set; } = "无";

		/// <summary>
		/// 标准考试时长，单位：分钟，若生成得试卷没有指定时长，则沿用该时长
		/// </summary>
		[JsonProperty]
		public double BaseDuration { get; set; } = 15d;

		/// <summary>
		/// 标准总分数，若试卷未指定总分，则沿用该分数
		/// </summary>
		[JsonProperty]
		public double BaseScore { get; set; } = 100d;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; }

		[JsonProperty, Column(DbType = "varchar(MAX)")]
		public string Description { get; set; }

		[JsonProperty]
		public DateTime EndTime { get; set; }

		[JsonProperty]
		public int ExamType { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(MAX)")]
		public string ExtraInfo { get; set; }

		/// <summary>
		/// 聚合码，设置之后，可以生成聚合多个考试的二维码
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string GroupCode { get; set; } = "";

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 是否严格交卷，是的话超时提交将不给分，否的话答考试结束后有多少分给多少分，1-是，0-否
		/// </summary>
		[JsonProperty]
		public int IsStrict { get; set; } = 0;

		[JsonProperty]
		public int OrderIndex { get; set; } = 1;

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string Remark { get; set; }

		[JsonProperty]
		public DateTime StartTime { get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		[JsonProperty]
		public int Status { get; set; } = 1;

		[JsonProperty, Column(DbType = "varchar(200)", IsNullable = false)]
		public string Title { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

	}

}
