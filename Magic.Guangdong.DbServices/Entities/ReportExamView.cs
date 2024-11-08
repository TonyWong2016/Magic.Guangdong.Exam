using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class ReportExamView {

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string AccountId { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string AssociationId { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string AssociationTitle { get; set; }

		[JsonProperty]
		public int? Audit { get; set; }

		[JsonProperty]
		public double? BaseDuration { get; set; }

		[JsonProperty]
		public double? BaseScore { get; set; }

		[JsonProperty]
		public int CardType { get; set; }

		[JsonProperty, Column(StringLength = 1500)]
		public string Description { get; set; }

		[JsonProperty, Column(DbType = "varchar(150)", IsNullable = false)]
		public string Email { get; set; }

		[JsonProperty]
		public DateTime? EndTime { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string ExamAddress { get; set; }

		[JsonProperty]
		public Guid ExamId { get; set; }

		[JsonProperty]
		public int ExamType { get; set; }

		[JsonProperty, Column(DbType = "money")]
		public decimal? Expenses { get; set; }

		[JsonProperty, Column(DbType = "varchar(2000)")]
		public string ExtraInfo { get; set; }

        [JsonProperty]
        public string PageConfig { get; set; }

        [JsonProperty]
		public long FileId { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string GroupCode { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string IdCard { get; set; }

		[JsonProperty]
		public int? IsStrict { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string Mobile { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string Name { get; set; }

		[JsonProperty]
		public Guid? OrderId { get; set; }

		[JsonProperty]
		public int? OrderIndex { get; set; }

		[JsonProperty]
		public int? Quota { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string Remark { get; set; }

		[JsonProperty]
		public long ReportId { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ReportNumber { get; set; }

		[JsonProperty]
		public int ReportStatus { get; set; }

		[JsonProperty]
		public int ReportStep { get; set; }

		[JsonProperty]
		public DateTime ReportTime { get; set; }

		[JsonProperty]
		public DateTime? StartTime { get; set; }

		[JsonProperty]
		public int? Status { get; set; }

		[JsonProperty, Column(DbType = "varchar(200)")]
		public string Title { get; set; }
        /// <summary>
        /// 参与测试的次数
        /// 0-没参加
        /// 1-参加了一次考试/测试，考试一般只有1次
        /// >1-参加了多次考试/测试，测试一般有多次
        /// </summary>
        [JsonProperty]
        public int TestedTime { get; set; }

        /// <summary>
        /// 联系方式可用程度
        /// 0-未验证
        /// 1-邮箱验证可用
        /// 2-手机号可用
        /// 3-邮箱和手机号都可用
        /// </summary>
        [JsonProperty]
        public int ConnAvailable { get; set; }

        //[JsonProperty]
        //public Guid AttachmentId { get; set; }

        [JsonProperty]
        public string SecurityIdCard { get; set; }

        [JsonProperty]
        public string SecurityMobile { get; set; }

        [JsonProperty]
        public int PhoneType { get; set; } = 0;

        [JsonProperty]
        public string HashIdcard { get; set; }
        [JsonProperty]
        public string HashMobile { get; set; }

        [JsonProperty]
        public int IsSecurity { get; set; } = 0;

        /// <summary>
        /// 是否允许独立访问
        /// </summary>
        [JsonProperty]
        public int IndependentAccess { get; set; } = 0;

        /// <summary>
        /// 绑定的评分标准
        /// </summary>
        [JsonProperty]
        public long SchemeId { get; set; } = 0;

        /// <summary>
        /// 是否需要登录
        /// </summary>
        [JsonProperty]
        public int LoginRequired { get; set; } = 1;
    }

}
