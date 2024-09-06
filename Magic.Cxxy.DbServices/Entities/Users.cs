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
	public partial class Users {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid id { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string address { get; set; }

		[JsonProperty]
		public DateTime? backDate { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string backFrom { get; set; }

		[JsonProperty]
		public DateTime? birthday { get; set; }

		[JsonProperty, Column(StringLength = 10)]
		public string carNumber { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime created_at { get; set; }

		[JsonProperty, Column(StringLength = 2)]
		public string gender { get; set; }

		[JsonProperty, Column(StringLength = 30, IsNullable = false)]
		public string idCard { get; set; }

		[JsonProperty, Column(StringLength = 20)]
		public string name { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string remark { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string shift { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime updated_at { get; set; }

	}

}
