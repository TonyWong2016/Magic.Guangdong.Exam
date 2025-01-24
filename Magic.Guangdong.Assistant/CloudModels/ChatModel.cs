using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.CloudModels
{
    public class ChatModel
    {
        public string prompt { get; set; }

        public string admin { get; set; }

        public string model { get; set; }

        public bool chatType { get; set; } = true;

        public bool isStream { get; set; } = true;

        public string initiator { get; set; }

        public string HunyuanModel
        {
            get
            {
                return "hunyuan-" + model;
            }
        }

        //public Message[] messages { get; set; }
        public string messages { get; set; }

        public string toolStr { get; set; }

        public ChatTool[]? tools
        {
            get
            {
                if (string.IsNullOrEmpty(toolStr) || toolStr.Length<3)
                    return null;
                return System.Text.Json.JsonSerializer.Deserialize<ChatTool[]>(toolStr);
            }
        }

        
    }

   

    public class ChatTool
    {
        public string type { get; set; } = "chat";

        public DsToolFunction toolFunction { get; set; }

    }

    public class Function
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public object Parameters { get; set; }
    }

    public class ChatForChecking
    {
        public string prompt { get; set; }

        public string admin { get; set; }


        public ChatMsgLog[]? messages { get; set; }
    }

    public class ChatMsgLog
    {
        public string message { get; set; }

        public string role { get; set; }
    }
}

