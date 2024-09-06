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
	public partial class Exam_MainMoneyListLog {

		/// <summary>
		/// 红包查询日志ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int ListLogID { get; set; }

		/// <summary>
		/// 活动名称
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string act_name { get; set; }

		/// <summary>
		/// 添加时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? adddate { get; set; }

		/// <summary>
		/// 所有xml信息
		/// </summary>
		[JsonProperty, Column(StringLength = 4000)]
		public string allInfo { get; set; }

		/// <summary>
		/// 金额
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string amount { get; set; }

		/// <summary>
		/// 红包单号
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string detail_id { get; set; }

		/// <summary>
		/// 商户订单号
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string mch_billno { get; set; }

		/// <summary>
		/// 领取红包的Openid
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string openid { get; set; }

		/// <summary>
		/// 领取时间
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string rcv_time { get; set; }

		/// <summary>
		/// 红包退款金额
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string refund_amount { get; set; }

		/// <summary>
		/// 红包退款时间
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string refund_time { get; set; }

		/// <summary>
		/// 红包发送时间
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string send_time { get; set; }

		/// <summary>
		/// 红包状态
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string status { get; set; }

		/// <summary>
		/// 红包金额
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string total_amount { get; set; }

		/// <summary>
		/// 红包个数
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string total_num { get; set; }

	}

}
