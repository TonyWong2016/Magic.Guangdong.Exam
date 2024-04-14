using FreeSql.DataAnnotations;
using MassTransit;
using Newtonsoft.Json;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Order {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = NewId.NextGuid();

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string AccountId { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[JsonProperty]
		public long ReportId { get; set; }

		/// <summary>
		/// 支付金额
		/// </summary>
		[JsonProperty, Column(DbType = "money")]
		public decimal Expenses { get; set; } = 0M;

		/// <summary>
		/// 发票Id,0-没开，其他-对应发票表里的记录
		/// </summary>
		[JsonProperty]
		public long InvoiceId { get; set; } = 0;

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 支付时间，当且仅当paytype为0时显示
		/// </summary>
		[JsonProperty]
		public string PayTime { get; set; }

		/// <summary>
		/// 0-未支付成功，1-微信，2-支付宝，3-银联（暂不支持），4-银行卡转账（暂不支持），5-其他
		/// </summary>
		[JsonProperty]
		public PayType PayType { get; set; } = PayType.Unpaid;

		[JsonProperty, Column(StringLength = 50)]
		public string Remark { get; set; } = string.Empty;

		/// <summary>
		/// 0-支付成功，1-待支付，2-支付失败，3-订单过期（10分钟），4-订单取消/作废
		/// </summary>
		[JsonProperty]
		public OrderStatus Status { get; set; } = OrderStatus.Unpaid;

		[JsonProperty, Column(StringLength = 150)]
		public string Subject { get; set; }

		/// <summary>
		/// 支付成功后支付平台回调的订单号
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(100)")]
		public string TradeNo { get; set; }

		/// <summary>
		/// 系统自己生成的订单号，支付宝里对应的字段就是OutTradeNo，微信啥的还不知道
		/// </summary>
        [JsonProperty, Column(DbType = "varchar(100)")]
        public string OutTradeNo { get; set; }

        [JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonProperty, Column(InsertValueSql = "getdate()")]
        public DateTime ExpiredTime { get; set; } = DateTime.Now.AddMinutes(10);

		/// <summary>
		/// 退款账单
		/// RE开头+OrderID去掉-
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(100)")]
		public string RefundNo { get; set; }
	}

	public enum OrderStatus { 
		Paid, //支付成功，
        Unpaid, // 待支付
		Expired, //过期，注意，订单过期要支持自动检测，10分钟内不支付自动过期，过期后要同步更新reportProcess里的状态
        Faild, //支付失败
		Refund,//退款
    }

	public enum PayType
	{
		Unpaid,
		WeChatPay,//微信支付
		AliPay,//支付宝
		UnionPay,//银联
		BankTransfer,//银行卡转账
		Other//其他
	}
}
