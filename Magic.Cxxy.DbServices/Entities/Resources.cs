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
	public partial class Resources {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid id { get; set; }

		[JsonProperty]
		public Guid? User_id { get; set; }

		[JsonProperty]
		public DateTime created_at { get; set; }

		[JsonProperty, Column(StringLength = 20)]
		public string extension_name { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string file_name { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string model_name { get; set; }

		[JsonProperty, Column(StringLength = 400, IsNullable = false)]
		public string path { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string remark { get; set; }

		[JsonProperty]
		public int res_type { get; set; }

	}

}
