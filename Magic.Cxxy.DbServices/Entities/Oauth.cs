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
	public partial class Oauth {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid id { get; set; } = Guid.NewGuid();

		[JsonProperty]
		public Guid? account_id { get; set; }

		[JsonProperty, Column(DbType = "varchar(200)")]
		public string avatar { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? created_at { get; set; }

		[JsonProperty, Column(DbType = "varchar(20)")]
		public string mobile { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string nick_name { get; set; }

		[JsonProperty, Column(StringLength = 30)]
		public string oauth_name { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string open_id { get; set; }

		/// <summary>
		/// 1-微信，2-支付宝，3-qq
		/// </summary>
		[JsonProperty]
		public int? type { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string unionid { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? updated_at { get; set; }

	}

}
