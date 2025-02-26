using FreeSql.DataAnnotations;
using Magic.Guangdong.Assistant;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Question {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		/// <summary>
		/// 题目解析
		/// </summary>
		[JsonProperty, Column(DbType = "nvarchar(MAX)")]
		public string Analysis { get; set; }

		/// <summary>
		/// 出题人
		/// </summary>
		[JsonProperty, Column(DbType = "nvarchar(50)")]
		public string Author { get; set; }

		

        [JsonProperty]
        public long ActivityId { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; }

		/// <summary>
		/// 题目难度，easy,normal,difficult三种，默认normal
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(20)", IsNullable = false)]
		public string Degree { get; set; } = "normal";

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 是否是公开考题，设置否的话，生成练习题的时候会避开抽到该题，默认是
		/// </summary>
		[JsonProperty]
		public IsOpen IsOpen { get; set; } = IsOpen.Yes;

		[JsonProperty, Column(DbType = "nvarchar(150)")]
		public string Remark { get; set; }

		[JsonProperty]
		public double Score { get; set; } = 0d;

		[JsonProperty]
		public Guid SubjectId { get; set; }

		[JsonProperty, Column(DbType = "nvarchar(MAX)")]
		public string Title { get; set; }

        /// <summary>
        /// title是通过富文本写入的，在列表页展示时，需要取消html字符，这里单独存一个500字符以内的纯文本标题用作列表展示
        /// </summary>
        [JsonProperty, Column(DbType = "varchar(500)")]
        public string TitleText
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Title))
                {
                    string txt = Utils.StripHTML(Title);
                    if (txt.Length > 500)
                        return txt.Substring(0, 500);
                    return txt;
                }
                return "";
            }
            set { }
        }
        [JsonProperty]
		public Guid TypeId { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

        /// <summary>
        /// 当题目所在考试已经设置了监控规则，则该规则失效
        /// </summary>
        [JsonProperty]
        public long MonitorRuleId { get; set; } = 0;

    }

	public enum IsOpen
	{
        All,//不设定，就是都包含
        Yes, 
		No
	}

}
