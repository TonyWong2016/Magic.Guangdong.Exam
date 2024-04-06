using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{
    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class ReportInfo
    {
        [JsonProperty, Column(IsPrimary = true)]
        public long Id { get; set; }= YitIdHelper.NextId();

        [JsonProperty]
        public Guid ExamId { get; set; } = Guid.Empty;

        [JsonProperty]
        public long ActivityId { get; set; } = 0;

        [JsonProperty, Column(StringLength = 100)]
        public string Name { get; set; }= string.Empty;

        [JsonProperty]
        public string IdCard { get; set; }= string.Empty;

        [JsonProperty]
        public string Mobile { get; set; }= string.Empty;

        [JsonProperty,Column(StringLength = 500)]
        public string Job {  get; set; }= string.Empty;

        [JsonProperty]
        public string Province { get; set; }=string.Empty;

        [JsonProperty]
        public string City { get; set; } = string.Empty;

        [JsonProperty]
        public string District {  get; set; } = string.Empty;

        [JsonProperty] 
        public string OtherInfo { get; set; } = string.Empty;

        [JsonProperty]
        public Guid AccountId {  get; set; } = Guid.Empty;
        
        [JsonProperty]
        public DateTime CreatedAt {  get; set; } = DateTime.Now;

        [JsonProperty]
        public DateTime UpdatedAt {  get; set; } = DateTime.Now;
    }
}
