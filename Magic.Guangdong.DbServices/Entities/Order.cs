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
		public Guid ExamId { get; set; }

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
		public DateTime? PayTime { get; set; }

		/// <summary>
		/// 0-未支付成功，1-微信，2-支付宝，3-银联（暂不支持），4-银行卡转账（暂不支持），5-其他
		/// </summary>
		[JsonProperty]
		public int PayType { get; set; } = 0;

		[JsonProperty, Column(StringLength = 50)]
		public string Remark { get; set; } = string.Empty;

		/// <summary>
		/// 0-支付成功，1-待支付，2-支付失败，3-订单过期（10分钟），4-订单取消/作废
		/// </summary>
		[JsonProperty]
		public int? Status { get; set; } = 1;

		[JsonProperty, Column(StringLength = 150)]
		public string Subject { get; set; }

		/// <summary>
		/// 第三方支付成功后回调的订单号
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(100)")]
		public string TradeNo { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }

}
