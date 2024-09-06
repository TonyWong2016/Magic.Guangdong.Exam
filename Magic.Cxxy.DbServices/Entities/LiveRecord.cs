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
	public partial class LiveRecord {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public long Id { get; set; }

		[JsonProperty]
		public int? AccessCount { get; set; } = 1;

		[JsonProperty, Column(DbType = "varchar(60)")]
		public string AccessSource { get; set; }

		[JsonProperty]
		public Guid? CourseID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string IP { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string IPPhyAddress { get; set; }

		[JsonProperty]
		public Guid? LiveID { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Location { get; set; }

		[JsonProperty]
		public double PlayTime { get; set; } = 0d;

		[JsonProperty, Column(StringLength = 200)]
		public string Remark { get; set; }

		[JsonProperty]
		public int Terminal { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UserID { get; set; }

		[JsonProperty, Column(StringLength = 300)]
		public string UserName { get; set; }

	}

}
