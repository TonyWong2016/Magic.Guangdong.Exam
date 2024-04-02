using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Guid AdminId { get; set; }

    }
}
