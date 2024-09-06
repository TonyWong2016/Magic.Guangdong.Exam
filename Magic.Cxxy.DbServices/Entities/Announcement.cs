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
	public partial class Announcement {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		[JsonProperty]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(StringLength = 2000)]
		public string Detail { get; set; }

		[JsonProperty]
		public DateTime? End_date { get; set; }

		[JsonProperty]
		public int? IsPublic { get; set; } = 0;

		[JsonProperty]
		public Guid? LocationID { get; set; }

		[JsonProperty, Column(StringLength = 30)]
		public string Operator { get; set; }

		/// <summary>
		/// 重点内容
		/// </summary>
		[JsonProperty, Column(StringLength = 100)]
		public string Point { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Remark { get; set; }

		[JsonProperty]
		public DateTime? Start_date { get; set; }

		/// <summary>
		/// 0-不展示，1-展示
		/// </summary>
		[JsonProperty]
		public int? Status { get; set; } = 1;

		[JsonProperty, Column(StringLength = 100)]
		public string SubTitle { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Title { get; set; }

		/// <summary>
		/// 0-普通公告，1-升级公告
		/// </summary>
		[JsonProperty]
		public int? Type { get; set; } = 0;

		[JsonProperty]
		public DateTime? Updated_at { get; set; }

	}

}
