using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using Yitter.IdGenerator;
using FreeSql.Internal;
using MassTransit;

namespace Magic.Guangdong.DbServices.Entities
{
    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]

    public partial class QuestionRecord
    {
        [JsonProperty, Column(IsPrimary = true)]
        public long Id { get; set; } = YitIdHelper.NextId();

        [JsonProperty]
        public long RecordId { get; set; }


        [JsonProperty,Column(DbType = "varchar(50)")]
        public string AccountId { get; set; }

        [JsonProperty]
        public long QuestionId { get; set; }

        /// <summary>
        /// 答案的id，这里不设置成long，是因为有多选这种情况
        /// </summary>
        [JsonProperty]
        public string UserAnswerId { get; set; }

        [JsonProperty]
        public string UserAnswerContent { get; set; }

        [JsonProperty]
        public int IsCorrect { get; set; }

        [JsonProperty]
        public int IsDeleted { get; set; } = 0;

        [JsonProperty, Column(InsertValueSql = "getdate()")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [JsonProperty]
        public string Version { get; set; }=NewId.NextGuid().ToString("N");
    }
}
