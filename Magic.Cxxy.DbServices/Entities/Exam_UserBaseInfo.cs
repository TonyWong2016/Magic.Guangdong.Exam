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
	/// 答题用户基本信息表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Exam_UserBaseInfo {

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? AddDate { get; set; }

		/// <summary>
		/// 地市ID
		/// </summary>
		[JsonProperty, Column(StringLength = 10)]
		public string CityID { get; set; }

		/// <summary>
		/// 班级ID
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string ClassID { get; set; }

		/// <summary>
		/// 区县ID
		/// </summary>
		[JsonProperty, Column(StringLength = 10)]
		public string CountyID { get; set; }

		/// <summary>
		/// 邮箱
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string Email { get; set; } = "";

		/// <summary>
		/// 年级ID
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string GradeID { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string ICNO { get; set; } = "";

		[JsonProperty]
		public int? ICNOType { get; set; } = 0;

		/// <summary>
		/// 手机号
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string Mobile { get; set; } = "";

		[JsonProperty]
		public int? Ori_UID { get; set; }

		/// <summary>
		/// 图片地址 用于生成徽章
		/// </summary>
		[JsonProperty, Column(StringLength = 150)]
		public string PicUrl { get; set; }

		/// <summary>
		/// 省份ID
		/// </summary>
		[JsonProperty, Column(StringLength = 10)]
		public string ProvinceID { get; set; }

		/// <summary>
		/// 学历组 对应小学 2 初中 3 高中  4   初中和高中对应到中学组  TeamID
		/// </summary>
		[JsonProperty]
		public int? QualificationID { get; set; } = 0;

		/// <summary>
		/// 性别
		/// </summary>
		[JsonProperty]
		public int? SexID { get; set; }

		/// <summary>
		/// 学历组
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string TeamID { get; set; }

		/// <summary>
		/// 用户ID
		/// </summary>
		[JsonProperty]
		public int UID { get; set; }

		/// <summary>
		/// 单位名称
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string UnitCaption { get; set; }

		/// <summary>
		/// 单位ID
		/// </summary>
		[JsonProperty]
		public int? UnitiD { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string UserName { get; set; }

	}

}
