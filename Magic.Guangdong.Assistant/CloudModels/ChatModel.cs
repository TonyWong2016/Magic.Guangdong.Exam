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


        public SimpleTool tool { get; set; }

        
    }

    public class SimpleTool
    {
        public string type { get; set; } = "function";

        /// <summary>
        /// 序列化后的函数，注意，调用的时候需要自行根据实际场景进行反序列化
        /// </summary>
        public string serializedFunction { get; set; } = "";

    }

    public class Tool<T>
    {
        public string type { get; set; } = "function";

        public CustomFunction<T> function { get; set; }
    }

    public class CustomFunction<T>
    {
        public string name { get; set; }

        public string description { get; set; }

        public FunctionParameters<T> parameters { get; set; }
    }

    public class FunctionParameters<T> 
    {
        public string type { get; set; } = "object";

        public T properties {  get; set; }
    }

    
}

