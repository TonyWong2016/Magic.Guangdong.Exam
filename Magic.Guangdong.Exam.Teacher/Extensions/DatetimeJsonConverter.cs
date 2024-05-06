using System;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Magic.Guangdong.Exam.Teacher.Extensions
{
    /// <summary>
    /// 统一时间转换
    /// </summary>
    internal class DatetimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParse(reader.GetString(), out DateTime date))
                    return date;
            }
            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy/MM/dd HH:mm:ss"));
        }
    }
}
