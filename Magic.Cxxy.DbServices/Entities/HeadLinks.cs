using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class HeadLinks {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[JsonProperty]
		public Guid AttachID { get; set; }

		[JsonProperty]
		public Guid? CourseID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty]
		public Guid? LiveID { get; set; }

		[JsonProperty]
		public Guid? LocationID { get; set; }

		/// <summary>
		/// 在哪个页面显示，1-首页，2-列表页
		/// </summary>
		[JsonProperty]
		public int? Page { get; set; } = 1;

		[JsonProperty, Column(StringLength = 200)]
		public string Remark { get; set; }

		[JsonProperty]
		public int Status { get; set; } = 0;

		[JsonProperty]
		public Guid? SubjectID { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string Title { get; set; }

		/// <summary>
		/// 0-课程，1-直播，2-专题
		/// </summary>
		[JsonProperty]
		public int Type { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string Url { get; set; }

	}

}
