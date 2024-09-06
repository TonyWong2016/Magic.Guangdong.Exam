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
	public partial class NLP_Keyword {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[JsonProperty]
		public Guid? QuestionId { get; set; }

		/// <summary>
		/// 2-填空，3-论述
		/// </summary>
		[JsonProperty]
		public int? QuestionType { get; set; }

		/// <summary>
		/// 权重值
		/// </summary>
		[JsonProperty]
		public double? Score { get; set; }

		/// <summary>
		/// 关键词标签
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string Tag { get; set; }

	}

}
