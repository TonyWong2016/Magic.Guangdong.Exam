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

        /// <summary>
        /// 审查状态
        /// 这个是服务端控制的
        /// </summary>
        [JsonProperty]
		public ReportStatus Status { get; set; } = ReportStatus.UnChecked;

        /// <summary>
        /// 报名进度，
        /// 根据用户报名状态来修改，在客户端更改，服务端不会修改这个状态，即便是后台给他退款了，
        /// 这个状态也不由服务端的操作而更改
		/// 但该字段可能被自动服务修改，比如订单长时间未支付，后台服务会取关闭订单，同时将该
		/// 字段修改为Failed
        /// </summary>
        [JsonProperty]
		public ReportStep Step { get; set; } = ReportStep.Reported;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		[JsonProperty]
		public string AccountId { get; set; }

		[JsonProperty]
		public Guid OrderId { get; set; }

		/// <summary>
		/// 参与测试的次数
		/// 0-没参加
		/// 1-参加了一次考试/测试，考试一般只有1次
		/// >1-参加了多次考试/测试，测试一般有多次
		/// </summary>
		[JsonProperty]
		public int TestedTime { get; set; } = 0;
	}

	public enum ReportStep
	{
		Reported,//报名了
		Paied,//付款了
		Failed,//失败了,订单长时间没支付或者报名审核没通过或者其他原因
		//Tested//考过试了,这个不要更新了
	}

	/// <summary>
	/// 审查状态
	/// 这个是后台根据实际情况修改的
	/// </summary>
	public enum ReportStatus
	{
		//通过
		Succeed,
		//不通过
		Failed,
		//待审核
		UnChecked,
		//已退款
		Refunded
	}

}
