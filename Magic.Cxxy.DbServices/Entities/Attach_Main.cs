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
	/// 附件表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Attach_Main {

		/// <summary>
		/// 附件ID
		/// </summary>
		[JsonProperty, Column(IsPrimary = true)]
		public Guid AttachID { get; set; }

		/// <summary>
		/// 添加时间
		/// </summary>
		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime AddDate { get; set; }

		/// <summary>
		/// 附件标题
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string AttachCaption { get; set; }

		/// <summary>
		/// 附件后缀名
		/// </summary>
		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string AttachExtName { get; set; }

		/// <summary>
		/// 附件试看地址
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string AttachFielUrlFree { get; set; }

		/// <summary>
		/// 附件地址
		/// </summary>
		[JsonProperty, Column(StringLength = 500, IsNullable = false)]
		public string AttachFileUrl { get; set; }

		/// <summary>
		/// 附件配图地址
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string AttachImageUrl { get; set; }

		[JsonProperty]
		public double? FileSize { get; set; } = 0d;

		[JsonProperty]
		public Guid? LessonID { get; set; }

	}

}
