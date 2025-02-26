using FreeSql.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using System.ComponentModel;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class MonitorStreamConfig {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string InputJson { get; set; }

		/// <summary>
		/// 不填默认为0。
		/// 0表示输入流为音视频。
		/// 2表示输入流为图片。
		/// 3表示输入流为画布。
		/// 4表示输入流为音频。
		/// 5表示输入流为纯视频。
		/// </summary>
		[JsonProperty]
		public StreamInputType InputType { get; set; } = StreamInputType.AudioVideo;

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty]
		public long MonitorRuleId { get; set; } = 0;

		[JsonProperty]
		public int Status { get; set; } = 0;

		[JsonProperty, Column(StringLength = 100)]
		public string StreamName { get; set; }

		[JsonProperty, Column(StringLength = 80)]
		public string StreamSessionId { get; set; }

		/// <summary>
		/// 流类型，camera-摄像头，capture-屏幕分享，picture-图片，video-本地视频，audio-本地音频
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public StreamType StreamType { get; set; } = StreamType.None;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

	}


	public enum StreamType
	{
		[Description("无")]
        None,
        [Description("摄像头")]
        Camera,
        [Description("屏幕分享")]
        ScreenCapture,
        [Description("图片")]
        Picture,
        [Description("本地视频")]
        LocalVideo,
        [Description("本地音频")]
        LocalAudio
    }

	public enum StreamInputType
	{
        [Description("音视频")]
        AudioVideo = 0,
        [Description("图片")]
        Picture = 2,
        [Description("画布")]
        Canvas = 3,
        [Description("音频")]
        Audio = 4,
        [Description("纯视频")]
        Video = 5
    }
}
