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
	public partial class Question {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		/// <summary>
		/// 题目解析
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(MAX)")]
		public string Analysis { get; set; }

		/// <summary>
		/// 出题人
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string Author { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string ColumnId { get; set; } = "0";

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

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
		public int IsOpen { get; set; } = 1;

		[JsonProperty, Column(DbType = "varchar(150)")]
		public string Remark { get; set; }

		[JsonProperty]
		public double Score { get; set; } = 0d;

		[JsonProperty]
		public Guid SubjectId { get; set; }

		[JsonProperty, Column(DbType = "varchar(MAX)")]
		public string Title { get; set; }

		/// <summary>
		/// title是通过富文本写入的，在列表页展示时，需要取消html字符，这里单独存一个500字符以内的纯文本标题用作列表展示
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string TitleText { get; set; }

		[JsonProperty]
		public Guid TypeId { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string UpdatedBy { get; set; }

	}

}
