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
	/// 试卷问题表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Exam_Problem {

		/// <summary>
		/// 问题ID 主键 自增
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public long ProblemID { get; set; }

		/// <summary>
		/// 添加时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 添加人ID
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string AddUserID { get; set; }

		/// <summary>
		/// 正确答案解析
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string AnswerContent { get; set; }

		/// <summary>
		/// 正确答案 多选题 多个的话 用分号隔开 
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string CorrectAnswer { get; set; }

		/// <summary>
		/// 数据标签范围 例如湖北答题 就备注湖北省青少年网络科普知识线上答题
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string DataMarks { get; set; }

		/// <summary>
		/// 难度 精度
		/// </summary>
		[JsonProperty]
		public double? Difficulty { get; set; }

		/// <summary>
		/// 显示形式 1图片 2文字
		/// </summary>
		[JsonProperty]
		public int? DisplayType { get; set; }

		/// <summary>
		/// 是否有子题  如果有子题则为父问题
		/// </summary>
		[JsonProperty]
		public int HasSubProblem { get; set; } = 0;

		/// <summary>
		///  暂不使用
		/// </summary>
		[JsonProperty]
		public int? KnowledgeID { get; set; }

		/// <summary>
		/// 父问题ID  父问题没有选项 只包含子问题
		/// </summary>
		[JsonProperty]
		public int ParentID { get; set; } = 0;

		/// <summary>
		/// 题目配图地址
		/// </summary>
		[JsonProperty, Column(StringLength = 550)]
		public string PhotoUrl { get; set; }

		/// <summary>
		/// 问题名称
		/// </summary>
		[JsonProperty, Column(StringLength = 300)]
		public string ProblemName { get; set; }

		/// <summary>
		/// 问题类型 1单选 2多选 3判断
		/// </summary>
		[JsonProperty]
		public int? ProblemType { get; set; }

		/// <summary>
		/// 原始分值 这里如果试卷大题有分数则优先用试卷答题分数计算
		/// </summary>
		[JsonProperty]
		public double? Score { get; set; }

		/// <summary>
		/// 问题分类 对应 Exam_ProblemSort
		/// </summary>
		[JsonProperty]
		public int? SortID { get; set; }

		/// <summary>
		/// 状态 1启用 2禁用
		/// </summary>
		[JsonProperty]
		public int Status { get; set; } = 1;

		/// <summary>
		/// 学科分类 暂不使用
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string SubjectCodeID { get; set; }

		/// <summary>
		/// 题目组别,2-小学，3-初中，4-高中
		/// </summary>
		[JsonProperty]
		public int TeamID { get; set; } = 2;

	}

}
