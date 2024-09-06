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
	/// 课程表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class CourseInfo {

		/// <summary>
		/// 主键 课程ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public Guid CourseID { get; set; }

		/// <summary>
		/// 学术学科
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Academicdiscipline { get; set; }

		/// <summary>
		/// 显示图片  4:3配图
		/// </summary>
		[JsonProperty, Column(StringLength = 300)]
		public string AttachUrl { get; set; }

		/// <summary>
		/// 3:4配图
		/// </summary>
		[JsonProperty, Column(StringLength = 10)]
		public string AttachUrl1 { get; set; }

		/// <summary>
		/// 1:1配图
		/// </summary>
		[JsonProperty, Column(StringLength = 10)]
		public string AttachUrl2 { get; set; }

		/// <summary>
		/// 关注
		/// </summary>
		[JsonProperty]
		public int AttentionCount { get; set; } = 0;

		/// <summary>
		/// 平均分
		/// </summary>
		[JsonProperty]
		public double AverageScore { get; set; } = 0d;

		/// <summary>
		/// 审核人id
		/// </summary>
		[JsonProperty]
		public int? CheckedUserID { get; set; }

		/// <summary>
		/// 收藏
		/// </summary>
		[JsonProperty]
		public int CollectionCount { get; set; } = 0;

		/// <summary>
		/// 所属那个栏目
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string ColumnID { get; set; }

		/// <summary>
		/// 评论
		/// </summary>
		[JsonProperty]
		public int CommentCount { get; set; } = 0;

		[JsonProperty]
		public int ComplexityAverageScore { get; set; } = 0;

		/// <summary>
		/// 难易程度  难3分  中2分  易1分
		/// </summary>
		[JsonProperty]
		public int ComplexityScore { get; set; } = 0;

		/// <summary>
		/// 版权单位ID
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string CopyrightUnitID4 { get; set; }

		/// <summary>
		/// 版权单位名称
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string CopyrightUnitIDName4 { get; set; }

		/// <summary>
		/// 课程摘要
		/// </summary>
		[JsonProperty, Column(DbType = "ntext")]
		public string CourseAbstract { get; set; }

		/// <summary>
		/// 课程地点
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string CourseAddress { get; set; }

		/// <summary>
		/// 地图坐标
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(100)")]
		public string CourseAddressPoint { get; set; }

		/// <summary>
		/// 课程类别  0知识传授  1技能实践   2能力提高
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseCategory { get; set; }

		/// <summary>
		/// 课程计划
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseClassProject { get; set; }

		/// <summary>
		/// 课时详细计划
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseClassProjectDetail { get; set; }

		/// <summary>
		/// 课程介绍
		/// </summary>
		[JsonProperty, Column(DbType = "ntext")]
		public string CourseContent { get; set; }

		/// <summary>
		/// 课程结束时间
		/// </summary>
		[JsonProperty]
		public DateTime? CourseDateEnd { get; set; }

		/// <summary>
		/// 课程时间格式
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseDateFormat { get; set; }

		/// <summary>
		/// 课程开始时间
		/// </summary>
		[JsonProperty]
		public DateTime? CourseDateStart { get; set; }

		/// <summary>
		/// 课程领域ID   S科学  T技术  E工程  M数学  A艺术  R人文  Q其他
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseFieldID { get; set; }

		/// <summary>
		/// 课程领域名称   S科学  T技术  E工程  M数学  A艺术  R人文  Q其他
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string CourseFieldName { get; set; }

		/// <summary>
		/// 适用年级
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseGradeLevel { get; set; }

		/// <summary>
		/// 适用年级开始
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseGradeLevel1 { get; set; }

		/// <summary>
		/// 适用年级截止
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseGradeLevel2 { get; set; }

		/// <summary>
		/// 授课语言  0普通话   1地方话  2英文   3法语  4其他
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseLanager { get; set; }

		/// <summary>
		/// 课程学段  0幼儿园 1小学 2初中 3高中 4大学本科  5研究生
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseLearning { get; set; }

		/// <summary>
		/// 课程方式   0线上  1线下  2线上线下
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseModel { get; set; }

		/// <summary>
		/// 费用
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseMoney { get; set; }

		/// <summary>
		/// 课程主名称
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(300)")]
		public string CourseName1 { get; set; }

		/// <summary>
		/// 课程辅名称 暂时不用 备用字段
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(300)")]
		public string CourseName2 { get; set; }

		/// <summary>
		/// 面向对象
		/// </summary>
		[JsonProperty, Column(StringLength = 300)]
		public string CourseObject { get; set; }

		/// <summary>
		/// 课程图片
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(400)")]
		public string CoursePicture { get; set; }

		/// <summary>
		/// 课程名额
		/// </summary>
		[JsonProperty]
		public int? CoursePlacesCount { get; set; }

		/// <summary>
		/// 课程名额规格 0 无限制 1有名额限制
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CoursePlacesFormat { get; set; }

		/// <summary>
		/// 课程所属期别
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CourseStage { get; set; }

		/// <summary>
		/// 课程学科ID
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(200)")]
		public string CourseSubject { get; set; }

		/// <summary>
		/// 课程学科名称
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string CourseSubjectName { get; set; }

		/// <summary>
		/// 课程主题关键词
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string CourseTopic { get; set; }

		/// <summary>
		/// 课程类型，0-普通课程，1-直播课程。
		/// </summary>
		[JsonProperty]
		public int? CourseType { get; set; } = 0;

		/// <summary>
		/// 课程网站地址
		/// </summary>
		[JsonProperty, Column(StringLength = 200)]
		public string CourseUrl { get; set; }

		/// <summary>
		/// 创建日期
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string CreateDate { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreateUserID { get; set; }

		[JsonProperty]
		public double CurriculumDesignAverageScore { get; set; } = 0d;

		/// <summary>
		/// 课程设计评分
		/// </summary>
		[JsonProperty]
		public double CurriculumDesignScore { get; set; } = 0d;

		/// <summary>
		/// 类型 聚焦课堂
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Expand { get; set; }

		/// <summary>
		/// 转发
		/// </summary>
		[JsonProperty]
		public int ForwardingCount { get; set; } = 0;

		[JsonProperty]
		public double IntellectualAverageScore { get; set; } = 0d;

		/// <summary>
		/// 知识内容评分
		/// </summary>
		[JsonProperty]
		public double IntellectualScore { get; set; } = 0d;

		/// <summary>
		/// 审核是否通过  0不通过  1通过  
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(10)")]
		public string IsApproved { get; set; }

		/// <summary>
		/// 是否验证登录，1-验证，0-不验证
		/// </summary>
		[JsonProperty]
		public int? IsCheck { get; set; } = 1;

		/// <summary>
		/// 是否已审核   0未审核  1审核
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(10)")]
		public string IsChecked { get; set; }

		/// <summary>
		/// 是否删除  0删除  1有效
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(10)")]
		public string IsDelete { get; set; }

		/// <summary>
		/// 课程是否公开，0-不公开，1-公开，不公开则为非面向全国的课程，只有对应地区可以看到
		/// </summary>
		[JsonProperty]
		public int? IsPublic { get; set; } = 0;

		/// <summary>
		/// 是否发布审核   0否  1是
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(10)")]
		public string IsReleased { get; set; }

		/// <summary>
		/// 组织单位ID
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string OrganizationUnitID1 { get; set; }

		/// <summary>
		/// 组织单位名称
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string OrganizationUnitIDName1 { get; set; }

		/// <summary>
		/// 参与模式  0免费  1收费  2会员  3推广  4自由报名  5授权听课
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ParticipationModel { get; set; }

		/// <summary>
		/// 播放限制，0-无限制，1-根据开课时间限制（未到开课时间不可播放），其他限制条件待添加
		/// </summary>
		[JsonProperty]
		public int PlayRestrictions { get; set; } = 0;

		/// <summary>
		/// 预习资料
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string PreviewResource { get; set; } = "";

		/// <summary>
		/// 提供单位ID
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string ProvideUnitID3 { get; set; }

		/// <summary>
		/// 提供单位名称
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string ProvideUnitIDName3 { get; set; }

		/// <summary>
		/// 推荐
		/// </summary>
		[JsonProperty]
		public int RecommendedCount { get; set; } = 0;

		/// <summary>
		/// 发布时间
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ReleaseDate { get; set; }

		/// <summary>
		/// 发布单位ID
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ReleaseUnitID5 { get; set; }

		/// <summary>
		/// 发布单位名称
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(200)")]
		public string ReleaseUnitIDName5 { get; set; }

		/// <summary>
		/// 评分次数
		/// </summary>
		[JsonProperty]
		public int ScoreCount { get; set; } = 0;

		/// <summary>
		/// 分享
		/// </summary>
		[JsonProperty]
		public int ShareCount { get; set; } = 0;

		/// <summary>
		/// 来源单位ID
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string SourceUnitID2 { get; set; }

		/// <summary>
		/// 来源单位名称
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string SourceUnitIDName2 { get; set; }

		[JsonProperty]
		public int? SpkID { get; set; }

		/// <summary>
		/// 1-显示，0-不显示
		/// </summary>
		[JsonProperty]
		public int? Status { get; set; } = 1;

		[JsonProperty]
		public double TeacherLevelAverageScore { get; set; } = 0d;

		/// <summary>
		/// 教师水平评分
		/// </summary>
		[JsonProperty]
		public double TeacherLevelScore { get; set; } = 0d;

		[JsonProperty]
		public double TeachingEffectAverageScore { get; set; } = 0d;

		/// <summary>
		/// 教学效果评分
		/// </summary>
		[JsonProperty]
		public double TeachingEffectScore { get; set; } = 0d;

		/// <summary>
		/// 总分
		/// </summary>
		[JsonProperty]
		public double TotalScore { get; set; } = 0d;

	}

}
