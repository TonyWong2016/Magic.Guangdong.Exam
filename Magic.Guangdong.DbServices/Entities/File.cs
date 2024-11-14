using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class File {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()", IsNullable = false)]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		/// <summary>
		/// 文件名称
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(150)", IsNullable = false)]
		public string Name { get; set; } = Assistant.Utils.GenerateRandomCodePro(10);

        /// <summary>
        /// 文件名称
        /// </summary>
        [JsonProperty, Column(DbType = "varchar(20)", IsNullable = false)]
        public string Ext { get; set; }="";


		/// <summary>
		/// 文件大小
		/// </summary>
		[JsonProperty, Column(IsNullable = false)]
		public long Size { get; set; } = 0;

		/// <summary>
		/// 文件类型，不是后缀，是业务类型
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(20)", IsNullable = false)]
		public string Type { get; set; } = "";

		/// <summary>
		/// 谁上传的
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(80)", IsNullable = false)]
		public string AccountId { get; set; } = "";

        [JsonProperty, Column(InsertValueSql = "getdate()", IsNullable = false)]
		public DateTime updatedAt { get; set; } = DateTime.Now;

		/// <summary>
		/// 存储路径
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(300)", IsNullable = false)]
		public string Path { get; set; } = "";

        /// <summary>
        /// 访问地址（短链）
        /// </summary>
        [JsonProperty, Column(DbType = "varchar(300)", IsNullable = false)]
		public string ShortUrl { get; set; } = "";

		/// <summary>
		/// 是否删除
		/// </summary>
		[JsonProperty, Column(IsNullable = false)]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 其他模型的业务id
		/// </summary>

		[JsonProperty, Column(IsNullable = false)]
		public string ConnId { get; set; } = "";

		/// <summary>
		/// 其他业务模型名称
		/// </summary>
		[JsonProperty]
		public string ConnName { get; set; } = "";

        [JsonProperty]
        public string Md5 { get; set; } = "";

        [JsonProperty]
        public string Remark { get; set; } = "";
    }

}
