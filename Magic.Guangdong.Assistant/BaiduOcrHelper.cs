using EasyCaching.Core;
using Magic.Guangdong.Assistant.CloudModels;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Assistant.Lib;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static NPOI.HSSF.Util.HSSFColor;

namespace Magic.Guangdong.Assistant
{
    public class BaiduOcrHelper:IBaiduOcrHelper
    {
        private readonly IRedisCachingProvider _redis;
        private readonly CloudConfigFactory _cloudConfigFactory;
        public BaiduOcrHelper(IRedisCachingProvider redis,CloudConfigFactory cloudConfigFactory)
        {
            _redis = redis;
            _cloudConfigFactory = cloudConfigFactory;
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAccessToken()
        {
            try
            {
                //var config = _cloudConfigFactory.GetConfigByPurpose("ocr");
                if (await _redis.KeyExistsAsync("baiduOcrToken"))
                {
                    return await _redis.StringGetAsync("baiduOcrToken");
                }
                //string clientId = ConfigurationHelper.GetSectionValue("faceApiKey");
                //string clientSecret = ConfigurationHelper.GetSectionValue("faceSecret");
                //string authHost = "https://aip.baidubce.com/oauth/2.0/token";
                //HttpClient client = new HttpClient();
                //List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
                //paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                //paraList.Add(new KeyValuePair<string, string>("client_id", config.AK));
                //paraList.Add(new KeyValuePair<string, string>("client_secret", config.SK));
                //HttpResponseMessage response = await client.PostAsync(authHost, new FormUrlEncodedContent(paraList));
                //string ret = await response.Content.ReadAsStringAsync();
                var tokenObj = await _cloudConfigFactory.GetBaiduCloudToken(CloudPurpose.OCR);
                if (tokenObj == null)
                    return "error";
                //文档上介绍token有效期是7天，这里保守保留3天
                await _redis.StringSetAsync("baiduOcrToken", tokenObj.access_token, DateTime.Now.AddSeconds(tokenObj.expires_in) - DateTime.Now);

                return tokenObj.access_token;
            }
            catch (Exception ex)
            {
                Logger.Error($"获取accesstoken失败,{ex.Message},{ex.StackTrace}");
                return "获取AccessToken失败";
            }
        }

        /// <summary>
        /// 识别文档
        /// </summary>
        /// <param name="pathOrUrl"></param>
        /// <returns></returns>
        public async Task<OcrResponseDto> DocumentRecognition(string pathOrUrl)
        {
            string token = await GetAccessToken();
            HttpClient client = new HttpClient();
            //string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/accurate_basic?access_token=" + token;
            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic?access_token=" + token;
            var contentList = new List<KeyValuePair<string, string>>();
            if (Uri.IsWellFormedUriString(pathOrUrl, UriKind.Absolute))
            {
                // 如果是URL，则添加url参数
                contentList.Add(new KeyValuePair<string, string>("url", pathOrUrl));
            }
            else
            {
                // 否则认为是文件路径，读取并转换为base64字符串
                string fileContentBase64 = await Utils.GetFileBase64(pathOrUrl);
                contentList.Add(new KeyValuePair<string, string>("pdf_file", fileContentBase64));
            }
            // 添加其他固定参数
            contentList.AddRange(new[]
            {
                new KeyValuePair<string, string>("detect_direction", "true"),
                new KeyValuePair<string, string>("paragraph", "false"),
                new KeyValuePair<string, string>("probability", "true"),
                new KeyValuePair<string, string>("multidirectional_recognize", "false")
            });
            var content = new FormUrlEncodedContent(contentList);
            HttpResponseMessage response = await client.PostAsync(host, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
            return JsonHelper.JsonDeserialize<OcrResponseDto>(responseBody);
        }


    }
}
