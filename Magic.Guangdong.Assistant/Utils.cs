using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Magic.Guangdong.Assistant
{
    public class Utils
    {
        /// <summary>
        /// 时间戳计时开始时间
        /// </summary>
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// DateTime转换为10位时间戳（单位：秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>10位时间戳（单位：秒）</returns>
        public static long DateTimeToTimeStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - timeStampStartTime).TotalSeconds;
        }

        /// <summary>
        /// DateTime转换为13位时间戳（单位：毫秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>13位时间戳（单位：毫秒）</returns>
        public static long DateTimeToLongTimeStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
        }

        /// <summary>
        /// 10位时间戳（单位：秒）转换为DateTime
        /// </summary>
        /// <param name="timeStamp">10位时间戳（单位：秒）</param>
        /// <returns>DateTime</returns>
        public static DateTime TimeStampToDateTime(long timeStamp)
        {
            return timeStampStartTime.AddSeconds(timeStamp).ToLocalTime();
        }

        /// <summary>
        /// 13位时间戳（单位：毫秒）转换为DateTime
        /// </summary>
        /// <param name="longTimeStamp">13位时间戳（单位：毫秒）</param>
        /// <returns>DateTime</returns>
        public static DateTime LongTimeStampToDateTime(long longTimeStamp)
        {
            return timeStampStartTime.AddMilliseconds(longTimeStamp).ToLocalTime();
        }
        /// <summary>
        /// 10位时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static uint GetTimeStampUint(DateTime time)
        {
            return (uint)(DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }
        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        public static DateTime ConvertStringToDateTime(string timeStamp)
        {
            //DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return timeStampStartTime.Add(toNow);
        }
        /// <summary>
        ///生成制定位数的随机码（纯数字）
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomCode(int length)
        {
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var r = new Random(Guid.NewGuid().GetHashCode());
                result.Append(r.Next(0, 10));
            }
            return result.ToString();
        }
        /// <summary>
        ///生成制定位数的随机码（字母数字混合）
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomCodePro(int length, int type = 0)
        {
            string allNumWord = "0123456789ABCDEFGHIGKLMNOPQRSTUVWXYZ";
            if (type == 1)
            {
                allNumWord = "ABCDEFGHIGKLMNOPQRSTUVWXYZ";
            }
            else if (type == 2)
            {
                allNumWord = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ";
            }
            else if (type == 3)
            {
                allNumWord = "0123456789abcdefghijklmnopqrstuvwxyz";
            }
            var result = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                var r = new Random(Guid.NewGuid().GetHashCode());
                result.Append(allNumWord[r.Next(0, allNumWord.Length - 1)]);
            }
            return result.ToString();
        }

        public static string GenerateRandomCodeFast(int length, int type = 0)
        {
            string chars = "0123456789ABCDEFGHIGKLMNOPQRSTUVWXYZ";
            if (type == 1)
            {
                chars = "ABCDEFGHIGKLMNOPQRSTUVWXYZ";
            }
            else if (type == 2)
            {
                chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ";
            }
            else if (type == 3)
            {
                chars = "0123456789abcdefghijklmnopqrstuvwxyz";
            }
            Span<char> buffer = stackalloc char[length];
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                buffer[i] = chars[random.Next(chars.Length)];
            }

            return new string(buffer);
        }

        /// <summary>
        /// 字符串转Unicode 直接Byte的方式，单字节操作
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>Unicode编码后的字符串</returns>
        public static string StringToUnicode(string source)
        {
            var bytes = Encoding.Unicode.GetBytes(source);
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < bytes.Length; i += 2)
            {
                stringBuilder.AppendFormat("\\u{0:x2}{1:x2}", bytes[i + 1], bytes[i]);
            }
            return stringBuilder.ToString();
        }

        /// <summary>  
        /// 字符串转为UniCode码字符串    通过Char的方式，一个Char为两个字节
        /// </summary>  
        /// <param name="s"></param>  
        /// <returns></returns>  
        public static string StringToUnicode2(string s)
        {
            char[] charbuffers = s.ToCharArray();
            byte[] buffer;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < charbuffers.Length; i++)
            {
                buffer = System.Text.Encoding.Unicode.GetBytes(charbuffers[i].ToString());
                sb.Append(String.Format("\\u{0:X2}{1:X2}", buffer[1], buffer[0]));
            }
            return sb.ToString();
        }
        /// <summary>  
        /// Unicode字符串转为正常字符串  例如：1的Unicode为 \u0031
        /// </summary>  
        /// <param name="srcText"></param>  
        /// <returns></returns>  
        public static string UnicodeToString(string srcText)
        {
            string dst = "";
            string src = srcText;
            int len = srcText.Length / 6;
            for (int i = 0; i <= len - 1; i++)
            {
                string str = "";
                str = src.Substring(0, 6).Substring(2);
                src = src.Substring(6);
                byte[] bytes = new byte[2];
                bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                dst += Encoding.Unicode.GetString(bytes);
            }
            return dst;
        }

        public static string StripHTML(string html)
        {
            try
            {
                System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"<script[\s\S]+</script *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@" href *= *[\s\S]*script *:", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex3 = new System.Text.RegularExpressions.Regex(@" on[\s\S]*=", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex4 = new System.Text.RegularExpressions.Regex(@"<iframe[\s\S]+</iframe *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex5 = new System.Text.RegularExpressions.Regex(@"<frameset[\s\S]+</frameset *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex6 = new System.Text.RegularExpressions.Regex(@"\<img[^\>]+\>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex7 = new System.Text.RegularExpressions.Regex(@"</p>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Regex regex8 = new System.Text.RegularExpressions.Regex(@"<p>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                html = regex1.Replace(html, ""); //过滤<script></script>标记
                html = regex2.Replace(html, ""); //过滤href=javascript: (<A>) 属性
                html = regex3.Replace(html, " _disibledevent="); //过滤其它控件的on事件
                html = regex4.Replace(html, ""); //过滤iframe
                html = regex5.Replace(html, ""); //过滤frameset
                html = regex6.Replace(html, ""); //过滤frameset
                html = regex7.Replace(html, ""); //过滤frameset
                html = regex8.Replace(html, ""); //过滤frameset
                html = html.Replace(" ", "");
                html = html.Replace("</strong>", "");
                html = html.Replace("<strong>", "");
                html = html.Replace("\r", "");
                html = html.Replace("\n", "");
                html = html.Replace("'", "");
                html = html.Replace("\"", "");
                html = html.Replace("\t", "");
                return html;
            }
            catch
            {
                return html;
            }
        }

        /// <summary>
        /// 普通字符串转成base64
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string ToBase64Str(string Str)
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(Str);
            return Convert.ToBase64String(b);

        }
        /// <summary>
        /// 传入base64内容，转成普通字符串
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        //public static string FromBase64Str(String Str)
        //{
        //    byte[] b = Convert.FromBase64String(Str);
        //    return System.Text.Encoding.Default.GetString(b);
        //}

        public static string FromBase64Str(string input)
        {
            // 验证字符串是否为Base64编码
            if (IsBase64String(input))
            {
                try
                {
                    // 解码Base64字符串
                    byte[] bytes = Convert.FromBase64String(input);
                    return Encoding.Default.GetString(bytes);
                }
                catch (FormatException)
                {
                    // 如果解码失败，返回原始字符串
                    return input;
                }
            }
            else
            {
                // 如果不是Base64编码，直接返回原始字符串
                return input;
            }
        }

        private static bool IsBase64String(string input)
        {
            // 基本长度检查
            if (string.IsNullOrEmpty(input) || input.Length % 4 != 0)
            {
                return false;
            }

            // 检查字符是否符合Base64编码规则
            foreach (char c in input)
            {
                if (!(char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '='))
                {
                    return false;
                }
            }

            return true;
        }

        public static string EncodeUrlParam(string msg,bool base64=true)
        {
            if (string.IsNullOrEmpty(msg))
                return "";
            if (base64)
            {
                return ToBase64Str(HttpUtility.UrlEncode(msg, Encoding.UTF8));
            }
            return HttpUtility.UrlEncode(msg, Encoding.UTF8);
        }

        /// <summary>
        /// 获取当前日期是本年度的第几周
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentWeekOfYear()
        {
            // 获取当前日期
            DateTime today = DateTime.Today;

            // 使用指定的文化信息（如 ISO 8601 标准）来计算周数
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
            return calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static int GetCurrentWeekOfMonth(DateTime currentDate)
        {
            // 获取当前日期所在月份的第一天
            DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

            // 计算当前日期与该月第一天之间的天数差
            int daysSinceFirstDay = (currentDate - firstDayOfMonth).Days;

            // 计算已过的完整周数（向下取整，因为剩余不足一周的部分不算作一周）
            int weekNumber = (int)Math.Floor((double)daysSinceFirstDay / 7);

            // 返回结果，加上1是因为计数从0开始，我们需要从第1周开始计数
            return weekNumber + 1;
        }


    }
}
