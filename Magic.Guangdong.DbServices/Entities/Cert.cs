using FreeSql.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Cert {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(DbType = "varchar(100)")]
		public string AccountId { get; set; }

		[JsonProperty, Column(StringLength = 150)]
		public string AwardName { get; set; }

		/// <summary>
		/// 证书上的原始内容
		/// </summary>
		[JsonProperty, Column(StringLength = -2, IsNullable = false)]
		public string CertContent { get; set; } = "[]";

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string CertNo { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty]
		public long? ReportId { get; set; }

		[JsonProperty]
		public CertStatus Status { get; set; } = CertStatus.Enable;

		[JsonProperty]
		public long TemplateId { get; set; }

		[JsonProperty, Column(StringLength = 100, IsNullable = false)]
		public string Title { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		[JsonProperty, Column(StringLength = 1000, IsNullable = false)]
		public string Url { get; set; }

	}

	public enum CertStatus
	{
		Enable,
		Disable,
	}
}
