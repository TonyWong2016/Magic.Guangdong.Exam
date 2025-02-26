using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class MonitorRule {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

        [JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? CreatedAt { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Description { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 0-可用，1-不可用
		/// </summary>
		[JsonProperty]
		public MonitorRuleStatus Status { get; set; } = MonitorRuleStatus.Enabled;

		/// <summary>
		/// 推流路数，最少是1
		/// </summary>
		[JsonProperty]
		public int? StreamCount { get; set; } = 1;

		/// <summary>
		/// 是否需要混流,0-不需要，1-需要；需要的话，StreamCount至少是2
		/// </summary>
		[JsonProperty]
		public int? StreamMix { get; set; } = 0;

		[JsonProperty, Column(StringLength = 500)]
		public string Title { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? UpdatedAt { get; set; }

	}

    public enum MonitorRuleStatus
    {
        Enabled,
        Disabled
    }

}
