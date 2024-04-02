using FreeSql.DataAnnotations;
using Magic.Guangdong.Assistant;
using Newtonsoft.Json;
using System.Web;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class QuestionItem {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		/// <summary>
		/// 分析
		/// </summary>
		[JsonProperty, Column(DbType = "nvarchar(500)")]
		public string Analysis { get; set; } = "";

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Code { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatdAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; }

		[JsonProperty, Column(DbType = "nvarchar(1000)")]
		public string Description { get; set; }

        /// <summary>
        /// 选项内容（description）是通过富文本写入的，在列表页展示时，需要取消html字符，这里单独存一个500字符以内的纯文本标题用作列表展示
        /// </summary>
        [JsonProperty, Column(DbType = "varchar(500)")]
        public string DescriptionText
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Description))
                {
                    string txt = HttpUtility.HtmlDecode(Utils.StripHTML(Description));
                    if (txt.Length > 500)
                        return txt.Substring(0, 500);
                    return txt;
                }
                return "";
            }
            set { }
        }

        /// <summary>
        /// 是否是答案
        /// </summary>
        [JsonProperty]
		public int IsAnswer { get; set; } = 0;

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 是否是选项，如果是则生成试卷时渲染到页面，如过不是就不渲染，对应的description里就是答案了
		/// </summary>
		[JsonProperty]
		public int IsOption { get; set; } = 1;

		/// <summary>
		/// 顺序
		/// </summary>
		[JsonProperty]
		public int? OrderIndex { get; set; } = 0;

		[JsonProperty]
		public long QuestionId { get; set; } = 0;

		[JsonProperty, Column(DbType = "nvarchar(150)")]
		public string Remark { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

	}

}
