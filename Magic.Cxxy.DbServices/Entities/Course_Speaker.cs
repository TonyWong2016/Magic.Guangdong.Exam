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
	public partial class Course_Speaker {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; }

		[JsonProperty]
		public Guid CourseID { get; set; }

		[JsonProperty]
		public int SpkID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty]
		public int? Order_Num { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string Remark { get; set; }

		/// <summary>
		/// 1-主讲，2-助讲，3-主持人，4-嘉宾
		/// </summary>
		[JsonProperty]
		public int SpkType { get; set; } = 1;

	}

}
