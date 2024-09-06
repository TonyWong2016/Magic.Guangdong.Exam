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
	public partial class Log_Records {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int LogID { get; set; }

		[JsonProperty, Column(DbType = "text")]
		public string Exception { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime LogDate { get; set; }

		[JsonProperty, Column(DbType = "varchar(256)", IsNullable = false)]
		public string Logger { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string LogLevel { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string LogType { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string MachineIp { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string MachineName { get; set; }

		[JsonProperty, Column(DbType = "text")]
		public string Message { get; set; }

		[JsonProperty, Column(DbType = "varchar(10)")]
		public string NetRequestMethod { get; set; }

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string NetRequestUrl { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string NetUserAuthtype { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string NetUserIdentity { get; set; }

		[JsonProperty, Column(DbType = "varchar(10)")]
		public string NetUserIsauthenticated { get; set; }

	}

}
