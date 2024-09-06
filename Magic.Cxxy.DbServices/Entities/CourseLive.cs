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
	public partial class CourseLive {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid LiveID { get; set; } = Guid.NewGuid();

		/// <summary>
		/// 是否允许聊天发弹幕，0-不允许，1-允许
		/// </summary>
		[JsonProperty]
		public int AllowChat { get; set; } = 0;

		/// <summary>
		/// 是否允许视频连线，0-不允许，1-允许
		/// </summary>
		[JsonProperty]
		public int AllowConn { get; set; } = 0;

		/// <summary>
		/// 允许交流，0-不允许，1-允许
		/// </summary>
		[JsonProperty]
		public int AllowMsg { get; set; } = 0;

		/// <summary>
		/// 直播结束后，是否归入回放页面，1-归入，0-不归入
		/// </summary>
		[JsonProperty]
		public int? AutoLesson { get; set; } = 1;

		[JsonProperty]
		public int CommentCnt { get; set; } = 0;

		[JsonProperty]
		public Guid? CourseID { get; set; }

		/// <summary>
		/// 直播封面
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string Cover { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		/// <summary>
		/// 直播时长
		/// </summary>
		[JsonProperty]
		public double Duration { get; set; }

		/// <summary>
		/// 该字段为计算页面展示时长的字段，仅限展示
		/// </summary>
		[JsonProperty]
		public double DurationDisplay { get; set; } = 1d;

		/// <summary>
		/// 录像文件存放地址
		/// </summary>
		[JsonProperty, Column(StringLength = 1000)]
		public string FileAddress { get; set; }

		[JsonProperty]
		public int LikeCnt { get; set; } = 0;

		/// <summary>
		/// 直播地点
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string LiveAddress { get; set; }

		[JsonProperty, Column(DbType = "ntext")]
		public string LiveDetail { get; set; }

		/// <summary>
		/// 直播内容简介
		/// </summary>
		[JsonProperty, Column(StringLength = 1500)]
		public string LiveIntro { get; set; }

		[JsonProperty]
		public DateTime LiveTime { get; set; }

		/// <summary>
		/// 课程开放类型，目前只有两种，0-面向所有用户开放，1-面向指定用户开发（需验证用户id，符合校验规则则可观看）
		/// </summary>
		[JsonProperty]
		public int OpenType { get; set; } = 0;

		/// <summary>
		/// 播放地址（m3u8格式的拉流文件地址，无token，在程序中生成）
		/// </summary>
		[JsonProperty, Column(StringLength = 300)]
		public string PlayAddress { get; set; }

		/// <summary>
		/// 直播类型，1-直播课，2-录播课
		/// </summary>
		[JsonProperty]
		public int PlayType { get; set; } = 1;

		/// <summary>
		/// 推流地址（源地址，无token，需在程序中生成）-》注意：该地址修改成手机端播放地址，推流地址无需记录，待此次湖北直播过后修改字段名
		/// </summary>
		[JsonProperty, Column(StringLength = 300)]
		public string PushAddress { get; set; }

		[JsonProperty]
		public int QaCnt { get; set; } = 0;

		[JsonProperty]
		public int ReportCnt { get; set; } = 0;

		/// <summary>
		/// 直播房间（allowconn为1的直播，该值不为空）
		/// </summary>
		[JsonProperty]
		public int? RoomNum { get; set; }

		[JsonProperty]
		public int ShareCnt { get; set; } = 0;

		[JsonProperty]
		public double StarsAvg { get; set; } = 0d;

		[JsonProperty]
		public int StarsCnt { get; set; } = 0;

		/// <summary>
		/// 0-未上线（不显示），1-上线（显示）
		/// </summary>
		[JsonProperty]
		public int Status { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string ThumbCover { get; set; }

		[JsonProperty, Column(StringLength = 150)]
		public string Title { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Updated_at { get; set; }

		[JsonProperty]
		public int WatchCnt { get; set; } = 0;

	}

}
