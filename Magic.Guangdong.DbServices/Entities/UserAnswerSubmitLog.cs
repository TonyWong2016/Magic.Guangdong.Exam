using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class UserAnswerSubmitLog {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty]
		public int ComplatedMode { get; set; } = 0;

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty, Column(DbType = "varchar(MAX)")]
		public string SubmitAnswer { get; set; } = "";

		[JsonProperty]
		public long Urid { get; set; }

		[JsonProperty]
		public string CapMsgId { get; set; } = "";

        [JsonProperty]
        public string CapInstance { get; set; } = "";

        [JsonProperty]
        public string CapSenttime { get; set; } = "";
    }

}
