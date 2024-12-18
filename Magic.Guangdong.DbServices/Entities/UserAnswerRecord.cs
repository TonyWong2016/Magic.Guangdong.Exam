﻿using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class UserAnswerRecord {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty]
		public long ReportId { get; set; } = 0;

		/// <summary>
		/// 作弊次数
		/// </summary>
		[JsonProperty]
		public int CheatCnt { get; set; } = 0;

		/// <summary>
		/// 是否完成考试
		/// </summary>
		[JsonProperty]
		public ExamComplated Complated { get; set; } = ExamComplated.No;

		/// <summary>
		/// 交卷方式，0-自主交卷，1-到时间自动交卷，2-作弊次数过多强制交卷
		/// </summary>
		[JsonProperty]
		public ExamComplatedMode ComplatedMode { get; set; } = ExamComplatedMode.Self;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; }

		[JsonProperty]
		public Guid ExamId { get; set; }

		//这里统一存准考证号，如果要查询其他信息，可以关联其他表格查询
		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string IdNumber { get; set; } = "";

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty]
		public DateTime LimitedTime { get; set; }

		[JsonProperty]
		public ExamMarked Marked { get; set; } = ExamMarked.No;

        

        [JsonProperty]
		public Guid PaperId { get; set; }

		[JsonProperty, Column(DbType = "varchar(1000)")]
		public string Remark { get; set; }

		[JsonProperty]
		public double ObjectiveScore { get; set; } = 0d;

        [JsonProperty]
        public double Score { get; set; } = 0d;

        /// <summary>
        /// 第几次参加（一般在练习模式会有多次，考试模式只有一次）
        /// </summary>
        [JsonProperty]
		public int Stage { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(MAX)", IsNullable = false)]
		public string SubmitAnswer { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

		/// <summary>
		/// 答题已经消耗的时长
		/// </summary>
		[JsonProperty]
		public double UsedTime { get; set; } = 0d;

		[JsonProperty]
        public string AccountId { get; set; }

        [JsonProperty, Column(DbType = "varchar(150)")]
		public string UserName { get; set; }

		

    }

	public enum ExamComplated
	{
		No,
		Yes,
		Cancle
	}
	public enum ExamComplatedMode
	{
		Self,
		Timeup,
		Force,
		Cheat,
		Auto//草稿自动保存的状态
	}

	public enum ExamMarked
	{
		No,
		Part,//判分了，但不代表结束了，因为客观题是自动判，主观题是人工判
		All,//都判完了
	}

    

    public enum IdType
	{
		IdCard,
		ReportNumber,
		Other
	}
}
