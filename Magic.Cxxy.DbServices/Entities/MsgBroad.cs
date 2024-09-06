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
	public partial class MsgBroad {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid MsgID { get; set; } = Guid.NewGuid();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(StringLength = 200, IsNullable = false)]
		public string Detail { get; set; }

		[JsonProperty]
		public int? IsLike { get; set; } = 1;

		[JsonProperty]
		public Guid LiveID { get; set; }

		/// <summary>
		/// 父id（标识回复的内容）
		/// </summary>
		[JsonProperty]
		public Guid? ParentID { get; set; }

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string SpeakerFace { get; set; }

		[JsonProperty]
		public int? Status { get; set; } = 0;

		/// <summary>
		/// 1-自己的发言，2-嘉宾的回复，3-主持人的发言
		/// </summary>
		[JsonProperty]
		public int Type { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Updated_at { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string UserID { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string UserName { get; set; }

	}

}
