using EasyCaching.Core;
using Magic.Guangdong.Assistant.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NPOI.HSSF.Util.HSSFColor;

namespace Magic.Guangdong.Assistant
{
    public class BaiduOcrHelper
    {
        private readonly IRedisCachingProvider _redis;
        private readonly CloudConfigFactory _cloudConfigFactory;
        public BaiduOcrHelper(IRedisCachingProvider redis,CloudConfigFactory cloudConfigFactory)
        {
            _redis = redis;
            _cloudConfigFactory = cloudConfigFactory;
        }

        public async Task<string> GetAccessToken()
        {
            try
            {
                var config = _cloudConfigFactory.GetConfigByPurpose("ocr");
                if (await _redis.KeyExistsAsync("baiduOcrToken"))
                {
                    return await _redis.StringGetAsync("baiduOcrToken");
                }
                //string clientId = ConfigurationHelper.GetSectionValue("faceApiKey");
                //string clientSecret = ConfigurationHelper.GetSectionValue("faceSecret");
                string authHost = "https://aip.baidubce.com/oauth/2.0/token";
                HttpClient client = new HttpClient();
                List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
                paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                paraList.Add(new KeyValuePair<string, string>("client_id", config.AK));
                paraList.Add(new KeyValuePair<string, string>("client_secret", config.SK));
                HttpResponseMessage response = await client.PostAsync(authHost, new FormUrlEncodedContent(paraList));
                string ret = await response.Content.ReadAsStringAsync();
                //文档上介绍token有效期是7天，这里保守保留3天
                await _redis.StringSetAsync("baiduOcrToken", ret, DateTime.Now.AddDays(3) - DateTime.Now);

                return ret;
            }
            catch (Exception ex)
            {
                Logger.Error($"获取accesstoken失败,{ex.Message},{ex.StackTrace}");
                return "获取AccessToken失败";
            }
        }
    }
}
