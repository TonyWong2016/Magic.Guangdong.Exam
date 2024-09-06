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
	public partial class Exam_LimitUsers {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 用户账号
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(100)")]
		public string Account { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string RangeID { get; set; }

		/// <summary>
		/// 用户Id
		/// </summary>
		[JsonProperty]
		public int? UserId { get; set; }

	}

}
