using FreeSql.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using MassTransit;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class ReportProcess {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty]
		public long ActivityId { get; set; }

		[JsonProperty, Column(IsPrimary = true)]
		public Guid ExamId { get; set; } = NewId.NextGuid();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		
        [JsonProperty]
        public long ReportId { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

        [JsonProperty]
		public ReportStatus Status { get; set; } = ReportStatus.UnChecked;

		[JsonProperty]
		public ReportStep Step { get; set; } = ReportStep.Reported;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		[JsonProperty]
		public string AccountId { get; set; }

		[JsonProperty]
		public Guid OrderId { get; set; }

	}

	public enum ReportStep
	{
		Reported,//报名了
		Paied,//付款了
		Failed,//失败了,订单长时间没支付或者报名审核没通过或者其他原因
	}

	public enum ReportStatus
	{
		Succeed,
		Failed,
		UnChecked		
	}

}
