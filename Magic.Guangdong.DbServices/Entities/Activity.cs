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
	public partial class Activity {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[JsonProperty]
		public string Description { get; set; } = "无";

		[JsonProperty]
		public DateTime EndTime { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty]
		public DateTime StartTime { get; set; }

		[JsonProperty]
		public int Status { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(200)", IsNullable = false)]
		public string Title { get; set; } = "";

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		/// <summary>
		/// 报名表需要填写的内容
		/// </summary>
		[JsonProperty]
		public string FieldJson { get; set; } = "{}";

		/// <summary>
		/// 活动报名的名额
		/// </summary>
		[JsonProperty]
		public int Quota { get; set; } = 0;

		/// <summary>
		/// 费用
		/// </summary>
		[JsonProperty, Column(DbType = "money")]
		public decimal Expenses { get; set; } = 0;

        /// <summary>
        /// 宣传封面
        /// </summary>
        [JsonProperty]
        public string Cover { get; set; } = "/images/cover.jpg";
    }

}
