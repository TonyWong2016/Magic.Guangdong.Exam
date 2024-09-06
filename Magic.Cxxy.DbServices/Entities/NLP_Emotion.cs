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
	public partial class NLP_Emotion {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// 表示分类的置信度，取值范围[0,1]
		/// </summary>
		[JsonProperty]
		public double? Confidence { get; set; }

		/// <summary>
		/// 表示属于消极类别的概率，取值范围[0,1]
		/// </summary>
		[JsonProperty]
		public double? Negative_prob { get; set; }

		/// <summary>
		/// 表示属于积极类别的概率 ，取值范围[0,1]
		/// </summary>
		[JsonProperty]
		public double? Positive_prob { get; set; }

		[JsonProperty]
		public Guid? QuestionId { get; set; }

		/// <summary>
		/// 2-填空，3-论述
		/// </summary>
		[JsonProperty]
		public int? QuestionType { get; set; }

		/// <summary>
		/// 表示情感极性分类结果，0:负向，1:中性，2:正向
		/// </summary>
		[JsonProperty]
		public int? Sentiment { get; set; }

		/// <summary>
		/// 分析的文本，最大2048字节
		/// </summary>
		[JsonProperty, Column(StringLength = 1024)]
		public string Text { get; set; }

	}

}
