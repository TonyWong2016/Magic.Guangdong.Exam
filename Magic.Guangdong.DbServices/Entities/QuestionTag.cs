using FreeSql.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using Yitter.IdGenerator;
using MassTransit;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class QuestionTag {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty]
		public Guid PaperId { get; set; }

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string Remark { get; set; }

		[JsonProperty, Column(DbType = "varchar(300)")]
		public string Title { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

		[JsonProperty]
		public double Accuracy { get; set; }

		[JsonProperty]
		public long QuestionId { get; set; }

        [JsonProperty]
        public string Version { get; set; } = NewId.NextGuid().ToString("N");
    }

}
