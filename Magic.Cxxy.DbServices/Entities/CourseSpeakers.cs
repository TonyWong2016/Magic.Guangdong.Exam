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
	/// 课程主讲助教表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class CourseSpeakers {

		/// <summary>
		/// 教师ID 自增
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int SpkID { get; set; }

		/// <summary>
		/// 课程ID(错误的关系，修改好后删掉该字段)
		/// </summary>
		[JsonProperty]
		public Guid? CourseID { get; set; }

		[JsonProperty, Column(DbType = "date", InsertValueSql = "getdate()")]
		public DateTime CreateDate { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Email { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Mobile { get; set; }

		/// <summary>
		/// 排序
		/// </summary>
		[JsonProperty]
		public int? OrderNum { get; set; }

		/// <summary>
		/// 讲课人地址
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string SpeakerAddress { get; set; }

		/// <summary>
		/// 讲课人职务 填写
		/// </summary>
		[JsonProperty, Column(StringLength = 200)]
		public string SpeakerDuty { get; set; }

		/// <summary>
		/// 讲课人简介
		/// </summary>
		[JsonProperty, Column(StringLength = 1200)]
		public string SpeakerIntro { get; set; }

		/// <summary>
		/// 讲师姓名
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(150)")]
		public string SpeakerName { get; set; }

		/// <summary>
		/// 讲课人头像
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string SpeakerPicture { get; set; }

		/// <summary>
		/// 讲课人职称 标准字典
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string SpeakerPTitleID { get; set; }

		/// <summary>
		/// 讲课人分类 1为主讲人 2为助教
		/// </summary>
		[JsonProperty]
		public int SpeakerType { get; set; } = 1;

		/// <summary>
		/// 讲课人单位名称
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string SpeakerUnitCaption { get; set; }

		/// <summary>
		/// 讲课人单位ID
		/// </summary>
		[JsonProperty]
		public int? SpeakerUnitID { get; set; }

	}

}
