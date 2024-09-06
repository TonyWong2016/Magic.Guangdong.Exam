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
	public partial class SerialNumber {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		/// <summary>
		/// 绑定的活动
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(200)")]
		public string Activity { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string AdminName { get; set; }

		/// <summary>
		/// 备用地址（绑定授权号后如果需要邮寄以改地址为准）
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(300)")]
		public string BakAddress { get; set; } = "";

		/// <summary>
		/// 批次
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(10)")]
		public string Batch { get; set; }

		[JsonProperty, Column(DbType = "varchar(20)", IsNullable = false)]
		public string Code { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		/// <summary>
		/// 期数
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Period { get; set; } = "1";

		[JsonProperty]
		public int ProvinceId { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(200)")]
		public string Remark { get; set; }

		[JsonProperty]
		public long SeriesId { get; set; } = 0;

		/// <summary>
		/// 状态，0-未启用，1-启用，-1-作废
		/// </summary>
		[JsonProperty]
		public int Status { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty]
		public int UserId { get; set; } = 0;

	}

}
