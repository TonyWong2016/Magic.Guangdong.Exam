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
	/// 课程用户关联表
	/// </summary>
	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Course_UserRecord {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int RecordID { get; set; }

		[JsonProperty]
		public Guid CourseID { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		/// <summary>
		/// 课程期别ID
		/// </summary>
		[JsonProperty]
		public int ProcessID { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string UserID { get; set; }

	}

}
