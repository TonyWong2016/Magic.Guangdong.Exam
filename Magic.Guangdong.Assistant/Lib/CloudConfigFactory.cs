using Magic.Guangdong.Assistant.CloudModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Magic.Guangdong.Assistant.Lib
{
    public class CloudConfigFactory
    {
        private readonly List<CloudConfig> _cloudConfigs;

        public CloudConfigFactory(List<CloudConfig> cloudConfigs)
        {
            _cloudConfigs = cloudConfigs;
        }

        public CloudConfig? GetConfigByAppId(string appId)
        {
            if (_cloudConfigs.Count == 0 || !_cloudConfigs.Any(u => u.AppId.Equals(appId, StringComparison.OrdinalIgnoreCase)))
                return null;
            return _cloudConfigs.FirstOrDefault(config => config.AppId.Equals(appId, StringComparison.OrdinalIgnoreCase));
        }

        public CloudConfig? GetConfigByPurpose(string purpose,string firm="baidu")
        {
            if (_cloudConfigs.Count == 0 || !_cloudConfigs.Any(config => config.Purpose.Equals(purpose, StringComparison.OrdinalIgnoreCase) && config.Firm.Equals(firm,StringComparison.OrdinalIgnoreCase)))
                return null;
            return _cloudConfigs.FirstOrDefault(config => config.Purpose.Equals(purpose, StringComparison.OrdinalIgnoreCase) && config.Firm.Equals(firm, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<TokenDto?> GetBaiduCloudToken(CloudPurpose purpose)
        {
            try
            {
                var config = GetConfigByPurpose(purpose.ToString(), "baidu");
                if (config == null)
                    return null;
                string authHost = "https://aip.baidubce.com/oauth/2.0/token";
                HttpClient client = new HttpClient();
                List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
                paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                paraList.Add(new KeyValuePair<string, string>("client_id", config.AK));
                paraList.Add(new KeyValuePair<string, string>("client_secret", config.SK));
                HttpResponseMessage response = await client.PostAsync(authHost, new FormUrlEncodedContent(paraList));
                string ret = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Logger.Error($"获取accesstoken失败,{ret}");
                    return null;
                }                
                return JsonConvert.DeserializeObject<TokenDto>(ret);
                
            }
            catch (Exception ex) { 
                Logger.Error(ex);
                return null;
            }
        }

        public List<CloudConfig> GetConfigs()
        {
            return _cloudConfigs;
        }
    }

    /// <summary>
    /// 云服务用途
    /// </summary>
    public enum CloudPurpose
    {
        OCR,
        Other
    }

    
}
