using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class ScoreScheme
    {

        [JsonProperty, Column(IsPrimary = true)]
        public long Id { get; set; } = YitIdHelper.NextId();

        /// <summary>
        /// 答对后评分动作，正数代表得分基础上正向相乘，比如这里给1，题目分数是2分，那答对后就得2分（1*2），给2就是得4分（2*2），以此类推
        /// </summary>
        [JsonProperty]
        public double CorrectAction { get; set; } = 1d;

        [JsonProperty, Column(InsertValueSql = "getdate()")]
        public DateTime? CreatedAt { get; set; }=DateTime.Now;

        [JsonProperty, Column(StringLength = 50)]
        public string CreatedBy { get; set; } = "system";

        [JsonProperty, Column(StringLength = 500)]
        public string Title { get; set; }

        [JsonProperty, Column(StringLength = 500)]
        public string Description { get; set; }

        /// <summary>
        /// 不答得评分动作，正数代表得分基础上正向相乘，比如这里给0，题目分数是2分，那不答题后就得0分（0*2），给-1就是扣2分（-1*2），以此类推
        /// </summary>
        [JsonProperty]
        public double EmptyAction { get; set; } = 0d;

        [JsonProperty, Column(InsertValueSql = "getdate()")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        [JsonProperty, Column(StringLength = 50)]
        public string UpdatedBy { get; set; } = "system";

        /// <summary>
        /// 答错后评分动作，正数代表得分基础上正向相乘，比如这里给0，题目分数是2分，那答错后就得0分（0*2），给-1就是扣2分（-1*2），以此类推
        /// </summary>
        [JsonProperty]
        public double WrongAction { get; set; } = 0d;

        [JsonProperty]
        public int? IsDeleted { get; set; } = 0;
    }

}
