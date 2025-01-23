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

    public class TokenDto
    {
        public string refresh_token { get; set;}

        public long expires_in { get; set;}

        public string scope { get; set;}

        public string session_key { get; set;}

        public string session_secret { get; set;}

        public string access_token { get; set;}
    }

    /// <summary>
    /// 百度OCR返回结果
    /// https://cloud.baidu.com/doc/OCR/s/1k3h7y3db#%E8%BF%94%E5%9B%9E%E8%AF%B4%E6%98%8E
    /// </summary>
    public class OcrResponseDto
    {
        /// <summary>
        /// 百度返回的日志ID
        /// </summary>
        public ulong log_id { get; set; }
        /// <summary>
        /// 识别方向
        /// 当 detect_direction=true 时返回该字段。
        /// - - 1：未定义，
        /// - 0：正向，
        /// - 1：逆时针90度，
        /// - 2：逆时针180度，
        /// - 3：逆时针270度
        /// </summary>
        public int direction { get; set; }
        /// <summary>
        /// 识别结果数，表示words_result的元素个数
        /// </summary>
        public uint words_result_num { get; set;}
        /// <summary>
        /// 识别结果数组
        /// </summary>
        public OcrWordsResult[] words_result { get; set; }
        /// <summary>
        /// 段落检测结果，当 paragraph=true 时返回该字段
        /// </summary>
        public OcrParagraphsResult[] paragraphs_result { get; set; }
        /// <summary>
        /// 识别结果数，表示 paragraphs_result的元素个数，当 paragraph=true 时返回该字段
        /// </summary>
        public uint paragraphs_result_num { get; set; }
        /// <summary>
        /// 传入PDF文件的总页数，当 pdf_file 参数有效时返回该字段
        /// </summary>
        public string pdf_file_size {  get; set;}
        /// <summary>
        /// 传入OFD文件的总页数，当 ofd_file 参数有效时返回该字段
        /// </summary>
        public string ofd_file_size { get; set;}
    }

    public class OcrWordsResult
    {
        /// <summary>
        /// 识别结果字符串
        /// </summary>
        public string words { get; set; }

        /// <summary>
        /// 识别结果中每一行的置信度值，包含average：行置信度平均值，variance：行置信度方差，min：行置信度最小值，当 probability=true 时返回该字段
        /// average大于0.9时，且min大于0.7，variance小于0.01时，可以认为识别结果正确，否则需要人工二次确认
        /// 设置阈值时，3个值有2个满足标准,另一个保本，就可以认为识别结果正确，否则需要人工二次确认
        /// 较高门槛时，三个值都需要满足需求，即前两个大于0.9，最后一个小于0.01，才算识别正确，否则需要人工二次确认
        /// </summary>
        public OcrProbability probability {  get; set; }

        public bool normalProbability
        {
            get
            {
                if (probability.average > 0.9 && probability.min > 0.7 && probability.variance < 0.1)
                    return true;
                if (probability.average > 0.9 && probability.min> 0.5 && probability.variance < 0.01)
                    return true;
                if (probability.average > 0.75 && probability.min > 0.9 && probability.variance < 0.01)
                    return true;
                return false;
            }
        }

        public bool strictProbability
        {
            get
            {
                return probability.average > 0.9 && probability.min > 0.8 && probability.variance < 0.01;
            }
        }
    }

    public class OcrProbability
    {
        /// <summary>
        /// 行置信度平均值
        /// 官方认为0.7-0.95属于可靠，低于0.7为不可靠，高于0.7算更可靠，但一般这个值要高于0.9才更可靠，也就是几乎不需要人工二次确认！
        /// </summary>
        public double average {  get; set; }
        /// <summary>
        /// 行置信度最小值
        /// 官方认为0.5-0.7属于可靠，低于0.5为不可靠，高于0.7算更可靠！
        /// </summary>
        public double min { get; set; }
        /// <summary>
        /// 行置信度方差
        /// 0-0.2 之间，越小越准确
        /// </summary>
        public double variance { get; set; }
    }

    public class OcrParagraphsResult
    {
        public int[] words_result_idx { get; set; }
    }
}
