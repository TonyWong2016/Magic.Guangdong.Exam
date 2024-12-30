using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.CloudModels
{
    public class AiConfig
    {
        [JsonProperty("IsOpenaiApi")]
        public string IsOpenaiApi { get; set; }

        [JsonProperty("Model")]
        public string Model { get; set; }
        [JsonProperty("SecretKey")]
        public string SecretKey {  get; set; }
        [JsonProperty("SecretId")]
        public string SecretId { get; set; }
        [JsonProperty("AppId")]
        public string AppId { get; set; }

        /// <summary>
        /// IsOpenaiApi为yes时，ApiKey为必填项
        /// </summary>
        [JsonProperty("ApiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("BaseUrl")]
        public string BaseUrl { get; set; }

        //public string Token { get; set; }
    }


    public class ChatModel
    {
        public string prompt { get; set; }

        public string admin { get; set; }

        public string model { get; set; }

        public bool chatType { get; set; } = true;

        public bool isStream { get; set; } = true;

        public string initiator { get; set; }

        public string finalModel
        {
            get
            {
                return "hunyuan-" + model;
            }
        }

        //public Message[] messages { get; set; }
        public string messages { get; set; }
    }
}
