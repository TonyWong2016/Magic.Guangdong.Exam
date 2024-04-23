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
	public partial class ReportOrderView {

        [JsonProperty]
        public long Id { get; set; }

        [JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string AccountId { get; set; }

		[JsonProperty]
		public long ActivityId { get; set; }

		[JsonProperty]
		public DateTime CreatedAt { get; set; }

		[JsonProperty]
		public Guid? ExamId { get; set; }

		[JsonProperty, Column(DbType = "money")]
		public decimal Expenses { get; set; }

		

		[JsonProperty]
		public long InvoiceId { get; set; }

		[JsonProperty]
		public DateTime OrderCreatedAt { get; set; }

		[JsonProperty]
		public Guid? OrderId { get; set; }

		[JsonProperty]
		public int? OrderStatus { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string OutTradeNo { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string PayTime { get; set; }

		[JsonProperty]
		public int PayType { get; set; }

		[JsonProperty]
		public long ReportId { get; set; }

		[JsonProperty]
		public int ReportStatus { get; set; }

		[JsonProperty]
		public int Step { get; set; }

		[JsonProperty, Column(StringLength = 150)]
		public string Subject { get; set; }

		[JsonProperty]
		public DateTime UpdatedAt { get; set; }

        [JsonProperty]
        public DateTime ExpiredTime { get; set; }
        /// <summary>
        /// 参与测试的次数
        /// 0-没参加
        /// 1-参加了一次考试/测试，考试一般只有1次
        /// >1-参加了多次考试/测试，测试一般有多次
        /// </summary>
        [JsonProperty]
        public int TestedTime { get; set; }
    }

}
