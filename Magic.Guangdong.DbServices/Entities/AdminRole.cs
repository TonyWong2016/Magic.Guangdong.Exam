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
    public partial class AdminRole
    {

        [JsonProperty, Column(IsPrimary = true)]
        public long Id { get; set; } = YitIdHelper.NextId();

        [JsonProperty, Column(InsertValueSql = "getdate()")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty]
        public Guid AdminId { get; set; }

        [JsonProperty]
        public long RoleId { get; set; }

        [JsonProperty, Column(InsertValueSql = "getdate()")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty]
        public int IsDeleted { get; set; } = 0;

    }
}
