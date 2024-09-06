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
	public partial class NLP_Lexer {

		[JsonProperty, Column(IsPrimary = true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[JsonProperty, Column(StringLength = 500)]
		public string Basic_words { get; set; }

		/// <summary>
		/// 字节级length
		/// </summary>
		[JsonProperty]
		public int? Byte_length { get; set; } = 0;

		/// <summary>
		/// 在text中的字节级offset
		/// </summary>
		[JsonProperty]
		public int? Byte_offset { get; set; } = 0;

		[JsonProperty, Column(StringLength = 50)]
		public string Format { get; set; }

		/// <summary>
		/// 词汇的字符串
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string Item { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string Loc_details { get; set; }

		/// <summary>
		/// 命名实体类型，命名实体识别算法使用。词性标注算法中，此项为空串
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string Ne { get; set; }

		/// <summary>
		/// 词性，词性标注算法使用。命名实体识别算法中，此项为空串
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string Pos { get; set; }

		/// <summary>
		/// 问题id
		/// </summary>
		[JsonProperty]
		public Guid QuestionID { get; set; }

		/// <summary>
		/// 2-填空，3-论述
		/// </summary>
		[JsonProperty]
		public int QuestionType { get; set; } = 3;

		/// <summary>
		/// 链指到知识库的URI，只对命名实体有效。对于非命名实体和链接不到知识库的命名实体，此项为空串
		/// </summary>
		[JsonProperty, Column(StringLength = 500)]
		public string Uri { get; set; }

	}

}
