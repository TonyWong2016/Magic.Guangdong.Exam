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
        // 使用依赖注入容器中的配置服务
        public static IConfiguration _configuration { get; set; }

        // 在应用启动时（如Startup.cs的ConfigureServices方法）注入配置
        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public static string GetSectionValue(string key)
        {
            return _configuration.GetSection(key).Value ?? "";
        }

        public static string[] GetSectionValues(string key)
        {
            return _configuration.GetSection(key).GetChildren().Select(u => u.Value).ToArray();
        }

        public static IConfigurationSection[] GetSections(string key)
        {
            return _configuration.GetSection(key).GetChildren().ToArray();
        }

        public static IConfigurationSection GetSection(string key)
        {
            return _configuration.GetSection(key);
        }
    }
}
