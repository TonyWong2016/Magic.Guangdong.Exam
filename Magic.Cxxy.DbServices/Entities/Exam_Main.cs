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
	/// 答题活动主表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Exam_Main {

		/// <summary>
		/// 答题活动ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int ExamID { get; set; }

		/// <summary>
		/// 添加时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 添加人
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string AddUserID { get; set; }

		/// <summary>
		/// 答题结束时间
		/// </summary>
		[JsonProperty]
		public DateTime? EndTime { get; set; }

		/// <summary>
		/// 答题须知
		/// </summary>
		[JsonProperty, Column(DbType = "ntext")]
		public string ExamIntro { get; set; }

		/// <summary>
		/// 考试时长(分钟数)
		/// </summary>
		[JsonProperty]
		public int? ExamLongs { get; set; }

		/// <summary>
		/// 答题活动名称
		/// </summary>
		[JsonProperty, Column(StringLength = 150, IsNullable = false)]
		public string ExamName { get; set; }

		/// <summary>
		/// 答题总分
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string ExamScoreSum { get; set; }

		/// <summary>
		/// 尾部文件名称
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string FooterName { get; set; }

		/// <summary>
		/// 头文件 名称
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string HeaderName { get; set; }

		/// <summary>
		/// 主办单位名称
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string HostUnit { get; set; }

		/// <summary>
		/// 是否允许每日答题 0为否 1为是 
		/// </summary>
		[JsonProperty]
		public int? IsCanMore { get; set; } = 0;

		/// <summary>
		/// 是否有证书
		/// </summary>
		[JsonProperty]
		public int? IsCert { get; set; } = 0;

		/// <summary>
		/// 是否是受限访问 默认为0限制 1为 比如只接受某个课程的培训其他人访问不到
		/// </summary>
		[JsonProperty]
		public int IsLimited { get; set; } = 0;

		/// <summary>
		/// 是否抽红包
		/// </summary>
		[JsonProperty]
		public int? IsMoney { get; set; } = 0;

		/// <summary>
		/// 是否限制学历 比如 中学抽中学的题 小学抽小学的题 0为不限制
		/// </summary>
		[JsonProperty]
		public int? IsTeam { get; set; } = 0;

		/// <summary>
		/// 限制条件 例如限制报名课程用户 限制学生或教师
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string LimitedRules { get; set; }

		/// <summary>
		/// 图片名 例：logo.png
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string LogoName { get; set; }

		/// <summary>
		/// 哪个省份的答题系统，主要用于索取单位和学校
		/// </summary>
		[JsonProperty]
		public int? ProvinceID { get; set; }

		/// <summary>
		/// 省份ID合集 用于 京津冀
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ProvinceIDs { get; set; }

		/// <summary>
		/// 手机端访问量
		/// </summary>
		[JsonProperty]
		public int PV_Mobile { get; set; } = 0;

		/// <summary>
		/// PC端访问量
		/// </summary>
		[JsonProperty]
		public int PV_PC { get; set; } = 0;

		/// <summary>
		/// 所属地域 如 hebei  
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string RangeArea { get; set; }

		/// <summary>
		/// 范围ID 例如湖北 用湖北省份ID 
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string RangeID { get; set; }

		/// <summary>
		/// 活动届次（没用到，这里用它作为新旧答题区分，0旧，1新）
		/// </summary>
		[JsonProperty]
		public int? SessionID { get; set; } = 1;

		/// <summary>
		/// 活动年份
		/// </summary>
		[JsonProperty, Column(StringLength = 10, IsNullable = false)]
		public string SessionYear { get; set; }

		/// <summary>
		/// 题目种类ID 用逗号隔开
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(100)")]
		public string SortIDs { get; set; }

		/// <summary>
		/// 答题开始时间
		/// </summary>
		[JsonProperty]
		public DateTime? StartTime { get; set; }

		/// <summary>
		/// 0-禁用，1-启用
		/// </summary>
		[JsonProperty]
		public int Status { get; set; } = 1;

		/// <summary>
		/// 单位类型 2 学校
		/// </summary>
		[JsonProperty]
		public int? UnitType { get; set; }

		/// <summary>
		/// 微信图片地址
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string WeixinUrl { get; set; }

	}

}
