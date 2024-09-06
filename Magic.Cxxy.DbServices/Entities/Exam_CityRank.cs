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
	public partial class Exam_CityRank {

		/// <summary>
		/// 地区排名ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int CityRankID { get; set; }

		/// <summary>
		/// 添加时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 地区名称
		/// </summary>
		[JsonProperty, Column(StringLength = 100, IsNullable = false)]
		public string CityCaption { get; set; } = "";

		/// <summary>
		/// 城市ID
		/// </summary>
		[JsonProperty, Column(DbType = "nchar(10)", IsNullable = false)]
		public string CityID { get; set; } = "";

		[JsonProperty]
		public int? ExamID { get; set; } = 0;

		/// <summary>
		/// 是否过期，旧数据也留着，万一要是脑热要趋势图呢（默认0  过期1）
		/// </summary>
		[JsonProperty]
		public int IsPast { get; set; } = 0;

		/// <summary>
		/// 分数
		/// </summary>
		[JsonProperty]
		public double Score { get; set; } = 0d;

		/// <summary>
		/// 名次
		/// </summary>
		[JsonProperty]
		public int Sort { get; set; } = 0;

		/// <summary>
		/// 学生数量
		/// </summary>
		[JsonProperty]
		public int StudentNum { get; set; } = 0;

		/// <summary>
		/// 趋势（默认和不变0  上升1  下降2）
		/// </summary>
		[JsonProperty]
		public int Trend { get; set; } = 0;

		/// <summary>
		/// 学校数量
		/// </summary>
		[JsonProperty]
		public int UnitNum { get; set; } = 0;

	}

}
