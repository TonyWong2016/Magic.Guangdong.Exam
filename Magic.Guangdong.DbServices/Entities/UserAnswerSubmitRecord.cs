using FreeSql.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class UserAnswerSubmitRecord
    {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty, Column(StringLength = 500)]
		public string ObjectiveAnswer { get; set; }

		[JsonProperty]
		public long QuestionId { get; set; }

		[JsonProperty]
		public long RecordId { get; set; }

		[JsonProperty, Column(StringLength = 500)]
		public string Remark { get; set; }

		[JsonProperty]
		public string SubjectiveAnswer { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty]
		public int IsSubjective { get; set; } = 0;
	}

}
