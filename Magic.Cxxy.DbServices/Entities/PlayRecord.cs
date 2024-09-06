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
	public partial class PlayRecord {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[JsonProperty]
		public int? AccessCount { get; set; } = 1;

		/// <summary>
		/// 访问来源（从哪个地域的创新学院访问的）
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(60)")]
		public string AccessSource { get; set; }

		[JsonProperty]
		public Guid? CourseID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string IPAddress { get; set; }

		/// <summary>
		/// ip物理地址
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string IPPhyAddress { get; set; }

		/// <summary>
		/// 0-短时间观看，1-长时间观看
		/// </summary>
		[JsonProperty]
		public int? IsStable { get; set; } = 0;

		[JsonProperty]
		public Guid? LessonID { get; set; }

		[JsonProperty]
		public Guid? LiveID { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Location { get; set; }

		/// <summary>
		/// 播放时长
		/// </summary>
		[JsonProperty]
		public double? PlayTime { get; set; } = 0d;

		/// <summary>
		/// 0-录播记录，1-直播记录（从直播记录表里同步总量过来）
		/// </summary>
		[JsonProperty]
		public int RecordType { get; set; } = 0;

		[JsonProperty, Column(StringLength = 200)]
		public string Remark { get; set; }

		/// <summary>
		/// 查看终端，1-pc，2-手机
		/// </summary>
		[JsonProperty]
		public int? Terminal { get; set; } = 1;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Updated_at { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string UserID { get; set; }

		[JsonProperty, Column(StringLength = 300)]
		public string UserName { get; set; }

	}

}
