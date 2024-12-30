using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant
{
    public static class JsonHelper
    {
        /// <summary>
        /// Json 序列化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static string JsonSerialize(object value, params JsonConverter[] converters)
        {
            if (value != null)
            {
                if (converters != null && converters.Length > 0)
                {
                    return JsonConvert.SerializeObject(value, converters);
                }
                else
                {
                    if (value is DataSet)
                        return JsonConvert.SerializeObject(value, new DataSetConverter());
                    else if (value is DataTable)
                        return JsonConvert.SerializeObject(value, new DataTableConverter());
                    else
                        return JsonConvert.SerializeObject(value);
                }
            }
            return string.Empty;
        }

        public static string JsonSerialize2(object value)
        {
            if (value != null)
            {
                return System.Text.Json.JsonSerializer.Serialize(value);
            }
            return string.Empty;
        }

        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string value, params JsonConverter[] converters)
        {
            if (string.IsNullOrEmpty(value))
                return default(T);

            if (converters != null && converters.Length > 0)
            {
                return JsonConvert.DeserializeObject<T>(value, converters);
            }
            else
            {
                Type type = typeof(T);

                if (type == typeof(DataSet))
                {
                    return JsonConvert.DeserializeObject<T>(value, new DataSetConverter());
                }
                else if (type == typeof(DataTable))
                {
                    return JsonConvert.DeserializeObject<T>(value, new DataTableConverter());
                }
                return JsonConvert.DeserializeObject<T>(value);
            }
        }


        public static T JsonDeserialize2<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
                return default(T);
            return System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }

        /// <summary> 
        /// 将JSON文本转换成数据行 
        /// </summary> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>数据行的字典</returns>
        public static Dictionary<string, object> DataRowFromJSON(string jsonText)
        {
            return JsonDeserialize<Dictionary<string, object>>(jsonText);
        }

        /// <summary>
        /// 将传入的字符串中间部分字符替换成特殊字符
        /// </summary>
        /// <param name="value">需要替换的字符串</param>
        /// <param name="startLen">前保留长度，默认4</param>
        /// <param name="subLen">要替换的长度，默认4</param>
        /// <param name="specialChar">特殊字符，默认为*</param>
        /// <returns>替换后的结果</returns>
        public static string ReplaceWithSpecialChar(this string value, int startLen = 4, int subLen = 4, char specialChar = '*')
        {
            if (value.Length <= startLen + subLen) return value;

            string startStr = value.Substring(0, startLen);
            string endStr = value.Substring(startLen + subLen);
            string specialStr = new string(specialChar, subLen);

            return startStr + specialStr + endStr;
        }
    }
}
