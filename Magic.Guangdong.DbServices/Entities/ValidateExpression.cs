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
	public partial class ValidateExpression {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string ColumnId { get; set; } = "0";

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; } = "";

		[JsonProperty]
		public Guid? ExamId { get; set; }

		[JsonProperty, Column(DbType = "varchar(MAX)")]
		public string Expression { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty]
		public Guid? PaperId { get; set; }

		[JsonProperty, Column(DbType = "varchar(2000)")]
		public string Remark { get; set; } = "无";

		[JsonProperty]
		public int Status { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; } = "";

	}

}
