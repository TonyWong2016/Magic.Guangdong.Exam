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
	public partial class UserMonitor {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string AccountId { get; set; }

		/// <summary>
		/// 所有返回值
		/// </summary>
		[JsonProperty, Column(StringLength = 4000)]
		public string AllJsonStr { get; set; }

		/// <summary>
		/// 推流域名
		/// </summary>
		[JsonProperty, Column(StringLength = 150)]
		public string App { get; set; }

		/// <summary>
		/// 推流路径
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string AppName { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? CreatedAt { get; set; }

		[JsonProperty]
		public long? EventTime { get; set; }

		/// <summary>
		/// 1-推流，0-断流，100-录制，更多参考tencentcloud官网
		/// </summary>
		[JsonProperty]
		public int? EventType { get; set; }

		[JsonProperty]
		public Guid? ExamId { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 根据推流信息推断出拉流地址，一般为https://playtx.xiaoxiaotong.org/{appname}/{streamid}{协议:.flv}
		/// </summary>
		[JsonProperty, Column(StringLength = 1000)]
		public string PlayAddress { get; set; }

		[JsonProperty]
		public long? RecordId { get; set; }

		/// <summary>
		/// 消息序列号，标识一次推流活动，一次推流活动会产生相同序列号的推流和断流消息
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string Sequence { get; set; }

		/// <summary>
		/// 流id
		/// </summary>
		[JsonProperty, Column(StringLength = 200)]
		public string StreamId { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? UpdatedAt { get; set; }

	}

}
