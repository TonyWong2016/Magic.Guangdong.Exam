using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class Simulation2
    {

        [JsonProperty, Column(IsPrimary = true)]
        public long Id { get; set; } = YitIdHelper.NextId();

        [JsonProperty]
        public int ComplatedMode { get; set; } = 0;

        [JsonProperty, Column(InsertValueSql = "getdate()")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty]
        public long Sid { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string SubmitAnswer { get; set; } = "";

    }

}
