using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class ChatRecord {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; }

		[JsonProperty]
		public Guid? AdminId { get; set; }

		[JsonProperty]
		public DateTime? CreatedAt { get; set; }

		[JsonProperty, Column(StringLength = 200)]
		public string FunctionName { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty, Column(StringLength = -2)]
		public string PromptContent { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string ResponseContent { get; set; }

		[JsonProperty]
		public int Status { get; set; } = 0;

		[JsonProperty]
		public long TopicId { get; set; } = 0;

	}

}
