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
	public partial class Relation_2023 {

		[JsonProperty]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; }

		[JsonProperty]
		public Guid ExamId { get; set; }

		[JsonProperty]
		public Guid Id { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; }

		[JsonProperty]
		public double ItemScore { get; set; }

		[JsonProperty]
		public Guid PaperId { get; set; }

		[JsonProperty]
		public long QuestionId { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string Remark { get; set; }

		[JsonProperty]
		public long TagId { get; set; }

		[JsonProperty]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

	}

}
