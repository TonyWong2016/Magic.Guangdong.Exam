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
	public partial class CourseSeries {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty]
		public DateTime? Created_at { get; set; }

		/// <summary>
		/// 是否公开（不公开的话需要授权码才能报名活动，公开则不需要）
		/// </summary>
		[JsonProperty]
		public int? IsPublic { get; set; } = 1;

		[JsonProperty, Column(StringLength = 50)]
		public string LocationID { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Remark { get; set; }

		/// <summary>
		/// 必学课程，若为0，则系列内所有课程都为必学
		/// </summary>
		[JsonProperty]
		public int RequiredCnt { get; set; } = 0;

		/// <summary>
		/// 报名的关键字
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(20)")]
		public string SeriesKey { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string SeriesName { get; set; }

		/// <summary>
		/// 系列对应的邀请函可选择的单位类型
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UnitType { get; set; }

		[JsonProperty]
		public DateTime? Updated_at { get; set; }

	}

}
