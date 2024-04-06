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
	public partial class ReportAttribute {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty]
		public long ActivityId { get; set; }

		[JsonProperty, Column(DbType = "varchar(4000)")]
		public string DataFilterJson { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string DataSource { get; set; }

		[JsonProperty]
		public Guid? FileId { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty, Column(StringLength = 500)]
		public string Limit { get; set; }

		[JsonProperty, Column(StringLength = 100, IsNullable = false)]
		public string Name { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string ShortName { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Type { get; set; }

	}

}
