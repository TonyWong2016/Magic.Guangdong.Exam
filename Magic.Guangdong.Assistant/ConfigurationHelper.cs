using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant
{
    public class ConfigurationHelper
    {
        private static IConfiguration _configuration;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        static ConfigurationHelper()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
            //在当前目录或者根目录中寻找appsettings.json文件
            //var fileName = "appsettings.json";
            ////var evname = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //var directory = AppContext.BaseDirectory;
            //directory = directory.Replace("\\", "/");

            //var filePath = $"{directory}/{fileName}";
            //if (!File.Exists(filePath))
            //{
            //    var length = directory.IndexOf("/bin");
            //    filePath = $"{directory.Substring(0, length)}/{fileName}";
            //}

            //var builder = ConfigurationBinder..
            //    .AddJsonFile(filePath, false, true);

            //_configuration = builder.Build();
        }

        public static string GetSectionValue(string key)
        {
            return _configuration.GetSection(key).Value;
        }

        public static string[] GetSectionValues(string key)
        {
            return _configuration.GetSection(key).GetChildren().Select(u => u.Value).ToArray();
        }
    }
}
