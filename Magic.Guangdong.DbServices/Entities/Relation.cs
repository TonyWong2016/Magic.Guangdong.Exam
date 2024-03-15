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
	public partial class Relation {

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; }

		[JsonProperty]
		public Guid ExamId { get; set; }

		[JsonProperty]
		public Guid Id { get; set; } = NewId.NextGuid();

        [JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty]
		public double ItemScore { get; set; } = 0d;

		[JsonProperty]
		public Guid PaperId { get; set; }

		[JsonProperty]
		public long QuestionId { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string Remark { get; set; }

		[JsonProperty]
		public long TagId { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

	}

}
