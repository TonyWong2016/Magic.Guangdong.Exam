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
	public partial class LiveReport {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 授权码
		/// </summary>
		[JsonProperty, Column(DbType = "char(10)")]
		public string Auth_Code { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		/// <summary>
		/// 0-直接观看，1-分享观看
		/// </summary>
		[JsonProperty]
		public int Is_Share { get; set; } = 0;

		[JsonProperty]
		public Guid LiveID { get; set; }

		/// <summary>
		/// 报名的人数（实际意义不大，主要是为了满足一些集体观看的需求）
		/// </summary>
		[JsonProperty]
		public int? ReportNumber { get; set; } = 1;

		/// <summary>
		/// 分享者的id（当用户查看分享者分享的观看链接观看时，记录分享者的id）
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string SharerID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string UserID { get; set; }

	}

}
