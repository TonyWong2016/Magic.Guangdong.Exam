
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Common;
using TencentCloud.Hunyuan.V20230901.Models;

namespace Magic.Guangdong.Assistant.CloudModels
{
    public static class HunyuanModels
    {
        public const string Lite = "hunyuan-lite";//free
        public const string Standard = "hunyuan-standard";//高性价比
        public const string Standard256k = "hunyuan-standard-256k";//中高性价比，功能平衡，推荐
        public const string Pro = "hunyuan-pro";//略贵
        public const string Large = "hunyuan-larg";//略贵
        public const string Turbo = "hunyuan-turbo";//略贵

        public const string Vision = "hunyuan-vision";//多模态，功能完善，贵，https://cloud.tencent.com/document/api/1729/105969

        public const string Translation = "hunyuan-translation";//翻译专用，https://cloud.tencent.com/document/api/1729/113395
        public const string TranslationLite = "hunyuan-translation-lite";//翻译专用(轻量级)
        //更多模型定义，可以参加腾讯混元官网：https://cloud.tencent.com/product/hunyuan
    }

    /// <summary>
    /// 公共参数
    /// </summary>
    public record HunyuanCommonParams
    {
        public string Mod { get; set; } = "hunyuan";

        public string Version { get; set; } = "2023-09-01";

        public string Action { get; set; } = "ChatCompletions";

        public string Region { get; set; } = "ap-beijing";

        public string Endpoint { get; set; } = "hunyuan.tencentcloudapi.com";

        public HunyuanCommonParams(string mod, string version, string action, string region, string endpoint)
        {
            Mod = mod;
            Version = version;
            Action = action;
            Region = region;
            Endpoint = endpoint;
        }

        public HunyuanCommonParams()
        {
            Mod = "hunyuan";
            Version = "2023-09-01";
            Action = "ChatCompletions";
            Region = "ap-beijing";
            Endpoint = "hunyuan.tencentcloudapi.com";
        }
    }

    

    /// <summary>
    /// 输入参数
    /// </summary>
    public class HunyuanChatParams
    {        

        public string Model { get; set; } = HunyuanModels.Standard256k;

        public Message[] messages { get; set; }

        public bool Stream { get; set; } = true;

        //----------以下都是非必填的，https://cloud.tencent.com/document/api/1729/105701--------------//
        /// <summary>
        /// 流式输出审核开关。
        /// </summary>
        public bool? StreamModeration { get; set; }
        /// <summary>
        ///  影响输出文本的多样性。模型已有默认参数，不传值时使用各模型推荐值，不推荐用户修改。
        ///  取值区间为 [0.0, 1.0]。取值越大，生成文本的多样性越强。
        /// </summary>
        public float? TopP { get; set; }

        /// <summary>
        /// 影响模型输出多样性，模型已有默认参数，不传值时使用各模型推荐值，不推荐用户修改。
        /// 取值区间为 [0.0, 2.0]。较高的数值会使输出更加多样化和不可预测，而较低的数值会使其更加集中和确定。
        /// </summary>
        public float? Temperature { get; set; }

        /// <summary>
        /// 功能增强（如搜索）开关。
        /// hunyuan-lite 无功能增强（如搜索）能力，该参数对 hunyuan-lite 版本不生效。
        /// </summary>
        public bool? EnableEnhancement { get; set; }

        /// <summary>
        /// 可调用的工具列表，仅对 hunyuan-pro、hunyuan-turbo、hunyuan-functioncall 模型生效。
        /// </summary>
        public Tool[]? Tools { get; set; }
        /// <summary>
        ///  1. 仅对 hunyuan-pro、hunyuan-turbo、hunyuan-functioncall 模型生效。
        ///  2. none：不调用工具；auto：模型自行选择生成回复或调用工具；custom：强制模型调用指定的工具。
        ///  3. 未设置时，默认值为auto
        ///  示例值：auto
        /// </summary>
        public string? ToolChoice { get; set; }

        /// <summary>
        /// 强制模型调用指定的工具，当参数ToolChoice为custom时，此参数为必填
        /// </summary>
        public Tool? CustomTool { get; set; }

        /// <summary>
        /// 默认是false，在值为true且命中搜索时，接口会返回SearchInfo
        /// 示例值：false
        /// </summary>
        public bool? SearchInfo { get; set; }

        /// <summary>
        /// 搜索引文角标开关。
        /// 说明：
        /// 1. 配合EnableEnhancement和SearchInfo参数使用。打开后，回答中命中搜索的结果会在片段后增加角标标志，对应SearchInfo列表中的链接。
        /// 2. false：开关关闭，true：开关打开。
        /// 3. 未传值时默认开关关闭（false）。
        /// 示例值：false
        /// </summary>
        public bool? Citation { get; set; }

        /// <summary>
        /// 是否开启极速版搜索，默认false，不开启；在开启且命中搜索时，会启用极速版搜索，流式输出首字返回更快。
        /// 示例值：false
        /// </summary>
        public bool? EnableSpeedSearch { get; set; }

        /// <summary>
        /// 多媒体开关。
        /// 详细介绍请阅读 多媒体介绍 中的说明。
        /// 说明：
        /// 1. 该参数目前仅对白名单内用户生效，如您想体验该功能请 联系我们。
        /// 2. 该参数仅在功能增强（如搜索）开关开启（EnableEnhancement=true）并且极速版搜索开关关闭（EnableSpeedSearch=false）时生效。
        /// 3. hunyuan-lite 无多媒体能力，该参数对 hunyuan-lite 版本不生效。
        /// 4. 未传值时默认关闭。
        /// 5. 开启并搜索到对应的多媒体信息时，会输出对应的多媒体地址，可以定制个性化的图文消息。
        /// 示例值：false
        /// </summary>
        public bool? EnableMultimedia { get; set; }

        /// <summary>
        /// 是否开启深度研究该问题，默认是false，在值为true且命中深度研究该问题时，会返回深度研究该问题信息。
        /// 示例值：false
        /// </summary>
        public bool? EnableDeepSearch { get; set; }

        /// <summary>
        /// 说明： 
        /// 1. 确保模型的输出是可复现的。 
        /// 2. 取值区间为非0正整数，最大值10000。 
        /// 3. 非必要不建议使用，不合理的取值会影响效果。
        ///示例值：1
        /// </summary>
        public int? Seed { get; set; }

        /// <summary>
        /// 强制搜索增强开关。
        /// 说明：
        /// 1. 未传值时默认关闭。
        /// 2. 开启后，将强制走AI搜索，当AI搜索结果为空时，由大模型回复兜底话术。
        /// 示例值：false
        /// </summary>
        public bool? ForceSearchEnhancement { get; set; }
    }

    /// <summary>
    /// 输出参数
    /// </summary>
    public class HunyunChatResponse
    {
        /// <summary>
        /// 	Unix 时间戳，单位为秒。
        /// </summary>
        public int Created {  get; set; }

        /// <summary>
        /// Token 统计信息。
        /// </summary>
        public Usage Usage { get; set; }
        /// <summary>
        /// 	免责声明。
        /// </summary>
        public string Note { get; set; }

        public string Id { get; set; }
        /// <summary>
        /// 回复内容。
        /// </summary>
        public Choice[] Choices { get; set; }

        public ErrorMsg ErrorMsg { get; set; }

        /// <summary>
        /// 多轮会话风险审核，值为1时，表明存在信息安全风险，建议终止客户多轮会话。
        /// </summary>
        public string ModerationLevel { get; set; }

        public SearchInfo SearchInfo { get; set; }

        /// <summary>
        /// 多媒体信息。
        ///  说明：
        /// 1. 可以用多媒体信息替换回复内容里的占位符，得到完整的消息。
        /// 2. 可能会出现回复内容里存在占位符，但是因为审核等原因没有返回多媒体信息。
        /// </summary>
        public Replace[] Replaces { get; set; }

        public string RequestId { get; set; }
    }


    [JsonConverter(typeof(Converter))]
    public class CustomSSEResponse : AbstractSSEModel
    {
        public string Response;

        private class Converter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                var resp = new CustomSSEResponse();
                var jObject = JObject.Load(reader);
                resp.Response = jObject.ToString();
                return resp;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(CustomSSEResponse);
            }
        }
    }



}
