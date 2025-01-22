using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.CloudModels
{
    public class CloudConfig
    {
        [JsonProperty("AppId")]
        public string AppId { get; set; }

        [JsonProperty("AK")]
        public string AK {  get; set; }

        [JsonProperty("SK")]
        public string SK { get; set; }

        [JsonProperty("Purpose")]
        public string Purpose { get; set; }

        [JsonProperty("Firm")]
        public string Firm { get; set; }
    }
}
