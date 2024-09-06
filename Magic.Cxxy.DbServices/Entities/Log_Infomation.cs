using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	/// <summary>
	/// 操作日志表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Log_Infomation {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int LogID { get; set; }

		/// <summary>
		/// 所属栏目
		/// </summary>
		[JsonProperty]
		public long ColumnID { get; set; }

		/// <summary>
		/// 操作类型(1、添加 2、修改 3、删除)
		/// </summary>
		[JsonProperty]
		public int DealType { get; set; }

		/// <summary>
		/// 日志内容
		/// </summary>
		[JsonProperty, Column(StringLength = 2000, IsNullable = false)]
		public string LogContent { get; set; }

		/// <summary>
		/// 操作日期
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime LogDate { get; set; }

		/// <summary>
		/// 操作者IP
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string RemoteIP { get; set; }

		/// <summary>
		/// 操作者ID
		/// </summary>
		[JsonProperty]
		public int UserID { get; set; }

	}

}
