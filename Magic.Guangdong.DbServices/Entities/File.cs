﻿using FreeSql.DataAnnotations;
using FreeSql.Internal;
using MassTransit;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class File {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = NewId.NextGuid();

		[JsonProperty, Column(InsertValueSql = "getdate()", IsNullable = false)]
		public DateTime CreatedAt { get; set; }

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
		public int Size { get; set; }

		/// <summary>
		/// 文件类型，不是后缀，是业务类型
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(20)", IsNullable = false)]
		public string Type { get; set; } = "";

		/// <summary>
		/// 谁上传的
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string UserId { get; set; } = "";

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
    }

}