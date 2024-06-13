namespace Magic.Guangdong.Assistant.CloudModels
{

    #region 通义千问请求相关模型
    public class QwenReqModel
    {
        /// <summary>
        /// 指定用于对话的通义千问模型名，
        /// 目前可选择
        /// qwen-turbo、qwen-plus、qwen-max、
        /// qwen-max-1201和qwen-max-longcontext。
        /// </summary>
        public string model { get; set; } = "qwen-turbo";

        public QwenInput input { get; set; }

        public QwenParameters parameters { get; set; }
    }

    public class QwenInput
    {
        /// <summary>
        /// 用户当前输入的期望模型执行指令，支持中英文。
        /// </summary>
        public string prompt { get; set; }
        /// <summary>
        /// 用户与模型的对话历史，对话接口未来都会有message传输，
        /// 不过prompt和history会持续兼容，
        /// list中的每个元素形式为{"role":角色, "content": 内容}。
        /// 角色当前可选值：system、user、assistant。
        /// 未来可以扩展到更多role。
        /// 如："input":{
        /// "messages":[
        /// {
        /// "role": "system",
        /// "content": "You are a helpful assistant."
        /// },
        /// {
        /// "role": "user",
        /// "content": "你好，附近哪里有博物馆？"
        /// }]
        /// }
        /// </summary>
        public List<QwenMessage> messages { get; set; }
    }

    public class QwenParameters
    {
        /// <summary>
        /// "text"表示旧版本的text
        /// "message"表示兼容openai的message
        /// </summary>
        public string result_format { get; set; } = "text";
        /// <summary>
        /// 生成时使用的随机数种子，用户控制模型生成内容的随机性。seed支持无符号64位整数，默认值为1234。在使用seed时，模型将尽可能生成相同或相似的结果，但目前不保证每次生成的结果完全相同。
        /// </summary>
        public int? seed { get; set; } = new Random().Next(1,65535);

        /// <summary>
        /// 用于限制模型生成token的数量，max_tokens设置的是生成上限，并不表示一定会生成这么多的token数量。其中qwen-turbo 最大值和默认值为1500， qwen-max、qwen-max-1201 、qwen-max-longcontext 和 qwen-plus最大值和默认值均为2000。
        /// </summary>
        public int? max_tokens { get; set; }
        /// <summary>
        /// 生成时，核采样方法的概率阈值。例如，取值为0.8时，仅保留累计概率之和大于等于0.8的概率分布中的token，作为随机采样的候选集。取值范围为（0,1.0)，取值越大，生成的随机性越高；取值越低，生成的随机性越低。默认值为0.8。注意，取值不要大于等于1
        /// </summary>
        public double top_p { get; set; } = 0.8;
        /// <summary>
        /// 生成时，采样候选集的大小。例如，取值为50时，仅将单次生成中得分最高的50个token组成随机采样的候选集。取值越大，生成的随机性越高；取值越小，生成的确定性越高。注意：如果top_k参数为空或者top_k的值大于100，表示不启用top_k策略，此时仅有top_p策略生效，默认是空。
        /// </summary>
        public int? top_k { get; set; } = 50;
        /// <summary>
        /// 用于控制模型生成时的重复度。提高repetition_penalty时可以降低模型生成的重复度。1.0表示不做惩罚。默认为1.1。
        /// </summary>
        public double repetition_penalty { get; set; } = 1.1;

        /// <summary>
        /// 用于控制随机性和多样性的程度。具体来说，temperature值控制了生成文本时对每个候选词的概率分布进行平滑的程度。较高的temperature值会降低概率分布的峰值，使得更多的低概率词被选择，生成结果更加多样化；而较低的temperature值则会增强概率分布的峰值，使得高概率词更容易被选择，生成结果更加确定。
        /// 取值范围：[0, 2)，系统默认值0.85。不建议取值为0，无意义。
        /// </summary>
        public double temperature { get; set; } = 0.85;

        /// <summary>
        /// stop参数用于实现内容生成过程的精确控制，在生成内容即将包含指定的字符串或token_ids时自动停止，生成内容不包含指定的内容。
        /// 例如，如果指定stop为"你好"，表示将要生成"你好"时停止；如果指定stop为[37763, 367]，表示将要生成"Observation"时停止。
        /// stop参数支持以list方式传入字符串数组或者token_ids数组，支持使用多个stop的场景。
        /// 说明
        /// list模式下不支持字符串和token_ids混用，list模式下元素类型要相同。
        /// </summary>
        public List<string> stop { get; set; } = null;

        /// <summary>
        /// 模型内置了互联网搜索服务，该参数控制模型在生成文本时是否参考使用互联网搜索结果。取值如下：
        /// true：启用互联网搜索，模型会将搜索结果作为文本生成过程中的参考信息，但模型会基于其内部逻辑“自行判断”是否使用互联网搜索结果。
        /// false（默认）：关闭互联网搜索。
        /// </summary>
        public bool enable_search { get; set; } = false;

        /// <summary>
        /// 用于控制流式输出模式，默认False，即后面内容会包含已经输出的内容；设置为True，将开启增量输出模式，后面输出不会包含已经输出的内容，您需要自行拼接整体输出，参考流式输出示例代码。
        /// 默认False：
        /// I
        /// I like
        /// i like apple
        /// True:
        /// I
        /// like
        /// apple
        /// 该参数只能与stream输出模式配合使用。
        /// </summary>
        public bool incremental_output { get; set; } = false;
    }

    public class QwenVlReqModel
    {
        public string model { get; set; }

        public QwenVlInput input { get; set; }

        public QwenVlParameters parameters { get; set; }
    }

    public class QwenVlInput
    {       
        /// <summary>
        
        /// </summary>
        public List<QwenVlMessage> messages { get; set; }
    }

    public class QwenVlMessage
    {
        public string role { get; set; }
        
        public List<QwenVlMessageContent> contents { get; set; }
    }

    public class QwenVlMessageContent
    {
        public string text { get; set; }

        public string image { get; set; }
    }

    public class QwenVlParameters
    {
        public double? top_p { get; set; } = 0.8;

        public int? top_k { get; set; } = 50;

        public int? seed { get; set; }

        public bool? incremental_output { get; set; }
    }
    #endregion


    #region 通义千问返回值响应相关模型
    /// <summary>
    /// 通义千问返回值响应模型
    /// </summary>
    public class QwenResp
    {
        public QwenOutput output { get; set; }

        public QwenChoise[] choices { get; set; }

        public QwenUsage usage { get; set; }

        public string request_id { get; set; }
    }

    public class QwenOutput
    {
        public string finish_reason { get; set; }

        public string text { get; set; }
    }

    public class QwenChoise
    {
        public string finish_reason { get; set; }

        public QwenMessage message { get; set; }
    }



    public class QwenUsage
    {
        public int output_tokens { get; set; }

        public int input_tokens { get; set; }

        public int total_tokens { get; set; }
    }
    #endregion

    public class QwenMessage
    {
        public string role { get; set; }

        public string content { get; set; }

    }

    public enum QwenSelectModel
    {
        turbo,
        plus,
        max,
        max_1201,
        max_longcontext
    };
}
