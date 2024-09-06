using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	/// <summary>
	/// 课程进度表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class CourseProcess {

		/// <summary>
		/// 进程ID   主键 自增长
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int ID { get; set; }

		/// <summary>
		/// 课程ID  外键
		/// </summary>
		[JsonProperty]
		public Guid? CourseID { get; set; }

		[JsonProperty, Column(DbType = "date", InsertValueSql = "getdate()")]
		public DateTime CreateDate { get; set; }

		/// <summary>
		/// 结束时间
		/// </summary>
		[JsonProperty]
		public DateTime? EndDate { get; set; }

		/// <summary>
		/// 期别排序 默认为1 第一期
		/// </summary>
		[JsonProperty]
		public int? ProcessOrderNum { get; set; } = 1;

		/// <summary>
		/// 课程状态，0-未上线，1-报名中，2-名额已满（未开始），3-已开始，4-已结束
		/// </summary>
		[JsonProperty]
		public int ProcessStatus { get; set; } = 0;

		/// <summary>
		/// 进程标题
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ProcessTitle { get; set; }

		/// <summary>
		/// 开始时间
		/// </summary>
		[JsonProperty]
		public DateTime? StartDate { get; set; }

	}

}
