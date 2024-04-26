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
	public partial class CertTemplate {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(StringLength = -2)]
		public string ConfigJsonStrForImg { get; set; } = "[]";

		[JsonProperty, Column(StringLength = -2)]
		public string ConfigJsonStrForPdf { get; set; } = "[]";

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonProperty, Column(StringLength = 100)]
		public string CreatedBy { get; set; } = "system";

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		/// <summary>
		/// 0-没有被锁定，可以修改内容，1-被锁定，不可以修改，颁发证书后自动锁定
		/// </summary>
		[JsonProperty]
		public CertTemplateLockStatus IsLock { get; set; } = CertTemplateLockStatus.Unlock;

		[JsonProperty, Column(DbType = "varchar(500)")]
		public string Remark { get; set; }

		/// <summary>
		/// 0-可用，1-不可用
		/// </summary>
		[JsonProperty]
		public CertTemplateStatus Status { get; set; } = CertTemplateStatus.Enable;

		[JsonProperty, Column(StringLength = 100)]
		public string Title { get; set; }

		[JsonProperty]
		public CertTemplateType Type { get; set; } = CertTemplateType.Picture;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		[JsonProperty, Column(StringLength = 1000)]
		public string Url { get; set; }

        [JsonProperty]
        public long ActivityId { get; set; } = 0;

		[JsonProperty]
		public string CanvasJson { get; set; }

    }

	public enum CertTemplateType
	{
		Picture,
		Pdf,
		Other
	}

	public enum CertTemplateStatus
	{
		Enable,
		Disable,
	}

	public enum CertTemplateLockStatus
	{
		Unlock,
		Lock
	}
}
