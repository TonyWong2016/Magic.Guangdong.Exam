using FreeSql.DatabaseModel;
using System;
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
	public partial class ReportCheckHistory {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty]
		public Guid AdminId { get; set; } = Guid.Empty;

		[JsonProperty, Column(StringLength = 200, IsNullable = false)]
		public string CheckRemark { get; set; } = "待审";

		/// <summary>
		/// 0-通过，1-未通过，2-待审核，3-退款
		/// </summary>
		[JsonProperty]
		public CheckStatus CheckStatus { get; set; } = CheckStatus.UnChecked;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty]
		public long ReportId { get; set; }

	}

	public enum CheckStatus
	{
		Passed,
		UnPassed,
		UnChecked,
	}

}
