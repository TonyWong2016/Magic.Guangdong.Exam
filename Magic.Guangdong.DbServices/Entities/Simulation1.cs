using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class Simulation1
    {

        [JsonProperty, Column(IsPrimary = true)]
        public long Id { get; set; } = YitIdHelper.NextId();

        [JsonProperty]
        public int ComplatedMode { get; set; } = 0;

        [JsonProperty, Column(InsertValueSql = "getdate()")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string SubmitAnswer { get; set; } = "";

        [JsonProperty, Column(InsertValueSql = "getdate()")]
        public DateTime UpdatedAt { get; set; }=DateTime.Now;
    }

}
