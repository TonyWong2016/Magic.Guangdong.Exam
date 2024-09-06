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
	public partial class GraphicLive {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		[JsonProperty, Column(StringLength = 100)]
		public string Address { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Created_at { get; set; }

		[JsonProperty, Column(StringLength = 400)]
		public string Img { get; set; }

		[JsonProperty]
		public int LikeCnt { get; set; } = 0;

		[JsonProperty]
		public Guid LiveID { get; set; }

		[JsonProperty, Column(StringLength = 20)]
		public string Operator { get; set; }

		[JsonProperty]
		public int UnlikeCnt { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime Updated_at { get; set; }

		[JsonProperty, Column(StringLength = 400)]
		public string Video { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string Words { get; set; }

	}

}
