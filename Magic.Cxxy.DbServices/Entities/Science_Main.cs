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
	/// 科学之夜用户信息记录表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Science_Main {

		/// <summary>
		/// 主键ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public int SNMID { get; set; }

		/// <summary>
		/// 添加时间 数据库记录
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? AddDate { get; set; }

		/// <summary>
		/// 邮寄地址
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string Address { get; set; }

		/// <summary>
		/// 地市区
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string city { get; set; }

		/// <summary>
		/// 国家
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string country { get; set; }

		/// <summary>
		/// 已答题次数 默认为0
		/// </summary>
		[JsonProperty]
		public int? ExamCount { get; set; } = 0;

		/// <summary>
		/// 答题领取红包 第一次
		/// </summary>
		[JsonProperty]
		public int? ExamMoney1 { get; set; } = 0;

		/// <summary>
		/// 答题领取红包第二次
		/// </summary>
		[JsonProperty]
		public int? ExamMoney2 { get; set; } = 0;

		[JsonProperty]
		public int? FinalPride { get; set; } = 0;

		/// <summary>
		/// 最终大奖时间
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string FinalPrideTime { get; set; }

		/// <summary>
		/// 微信头像地址
		/// </summary>
		[JsonProperty, Column(StringLength = 200)]
		public string headimgurl { get; set; }

		/// <summary>
		/// 默认为0 分享完成为1
		/// </summary>
		[JsonProperty]
		public int? IsShare { get; set; } = 0;

		/// <summary>
		/// 是否投票 默认为0  1为已投票
		/// </summary>
		[JsonProperty]
		public int? IsVote { get; set; } = 0;

		/// <summary>
		/// 语言
		/// </summary>
		[JsonProperty, Column(StringLength = 20)]
		public string language { get; set; }

		/// <summary>
		/// 本地头像地址 从微信下载后 备用字段
		/// </summary>
		[JsonProperty, Column(StringLength = 200)]
		public string LocalHeadImageUrl { get; set; }

		/// <summary>
		/// 手机号
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string Mobile { get; set; }

		/// <summary>
		/// 昵称
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string nickname { get; set; }

		/// <summary>
		/// 用户OpenID 河北科技馆微信号
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string OpenID { get; set; }

		[JsonProperty]
		public int? Ori_SNMID { get; set; } = 0;

		/// <summary>
		/// 邮政编码
		/// </summary>
		[JsonProperty, Column(StringLength = 20)]
		public string PostCode { get; set; }

		/// <summary>
		/// 省份
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string province { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string RealName { get; set; }

		/// <summary>
		/// 推荐用户OpenID 如没有则为空 
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string ROpenID { get; set; } = "";

		/// <summary>
		/// 活动届次iD
		/// </summary>
		[JsonProperty]
		public int? SessionID { get; set; } = 1;

		/// <summary>
		/// 性别
		/// </summary>
		[JsonProperty, Column(StringLength = 20)]
		public string sex { get; set; }

		/// <summary>
		/// 默认为0  分享出去如果有用户关注则为1
		/// </summary>
		[JsonProperty]
		public int? ShareSucceed { get; set; } = 0;

		/// <summary>
		/// 分享时间
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string ShareTime { get; set; } = "";

		/// <summary>
		/// 关注公众号时间
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string subscribe_time { get; set; }

		/// <summary>
		/// 唯一标识ID
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string unionid { get; set; } = "";

		/// <summary>
		/// 投票是否领取红包
		/// </summary>
		[JsonProperty]
		public int? VoteMoney { get; set; } = 0;

		/// <summary>
		/// 投票时间
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string VoteTime { get; set; } = "";

	}

}
