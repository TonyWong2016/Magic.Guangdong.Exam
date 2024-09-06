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
	public partial class Science_MainPrideItemPool {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int id { get; set; }

		[JsonProperty]
		public int PrideItemID { get; set; }

		[JsonProperty]
		public int StatusID { get; set; } = 1;

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string Time { get; set; } = "";

		[JsonProperty, Column(StringLength = 50)]
		public string TimeDate { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string TimeHour { get; set; }

	}

}
