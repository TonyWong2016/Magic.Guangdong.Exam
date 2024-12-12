using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class KeyAction
    {

        [JsonProperty, Column(IsPrimary = true)]
        public long Id { get; set; } = YitIdHelper.NextId();

        [JsonProperty, Column(DbType = "varchar(100)")]
        public string Action { get; set; } = "";

        [JsonProperty, Column(DbType = "varchar(150)")]
        public string Router { get; set; } = "";

        [JsonProperty, Column(InsertValueSql = "getdate()")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonProperty, Column(DbType = "varchar(2000)")]
        public string Description { get; set; } = "";

        [JsonProperty]
        public Guid AdminId { get; set; } = Guid.Empty;

        [JsonProperty]
        public Guid AccountId { get; set; } = Guid.Empty;

        [JsonProperty]
        public int Type { get; set; } = 1;

        [JsonProperty]
        public string CapMsgId { get; set; } = "";

        [JsonProperty]
        public string CapInstance { get; set; } = "";

        [JsonProperty]
        public string CapSenttime { get; set; } = "";
        [JsonProperty]
        public DateTime ExpiredAt { get; set; } =  DateTime.Now.AddDays(Assistant.Utils.GetGlobalExpiredDay());
    }
}
