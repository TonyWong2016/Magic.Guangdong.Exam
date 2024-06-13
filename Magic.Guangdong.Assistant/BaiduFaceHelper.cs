using EasyCaching.Core;
using Magic.Guangdong.Assistant.IService;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace Magic.Guangdong.Assistant
{
    public class BaiduFaceHelper: IBaiduFaceHelper
    {
        private readonly IRedisCachingProvider redis;
        public BaiduFaceHelper(IRedisCachingProvider redis)
        {
            this.redis = redis;
        }
        /// <summary>
        /// 获取accesstoken
        /// https://ai.baidu.com/ai-doc/REFERENCE/Ck3dwjhhu
        /// </summary>
        /// <returns></returns>
        public async Task<string> getAccessToken()
        {
            try
            {
                if (await redis.KeyExistsAsync("baiduFaceToken"))
                {
                    return await redis.StringGetAsync("baiduFaceToken");
                }
                string clientId = ConfigurationHelper.GetSectionValue("faceApiKey");
                string clientSecret = ConfigurationHelper.GetSectionValue("faceSecret");
                string authHost = "https://aip.baidubce.com/oauth/2.0/token";
                HttpClient client = new HttpClient();
                List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
                paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                paraList.Add(new KeyValuePair<string, string>("client_id", clientId));
                paraList.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
                HttpResponseMessage response = await client.PostAsync(authHost, new FormUrlEncodedContent(paraList));
                string ret= await response.Content.ReadAsStringAsync();
                //文档上介绍token有效期是7天，这里保守保留3天
                await redis.StringSetAsync("baiduFaceToken", ret, DateTime.Now.AddDays(3) - DateTime.Now);
                
                return ret;
            }
            catch(Exception ex)
            {
                Logger.Error($"获取accesstoken失败,{ex.Message},{ex.StackTrace}");
                return "获取AccessToken失败";
            }
        }

        /// <summary>
        /// 人脸检测
        /// https://ai.baidu.com/ai-doc/FACE/yk37c1u4t#%E8%BF%94%E5%9B%9E%E8%AF%B4%E6%98%8E
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> faceDetect(CloudModels.FaceDetect model)
        {
            if (model.image_type=="BASE64" && !string.IsNullOrEmpty(model.image) && model.image.Contains(','))
            {
                model.image = model.image.Split(',')[1];
            }
            return await httpPost(JsonHelper.JsonSerialize(model), "https://aip.baidubce.com/rest/2.0/face/v3/detect?access_token=");

        }
        /// <summary>
        /// 人脸注册
        /// https://ai.baidu.com/ai-doc/FACE/7k37c1twu#%E8%AF%B7%E6%B1%82%E8%AF%B4%E6%98%8E
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> faceAdd(CloudModels.FaceAdd model)
        {
            if (model.image_type == "BASE64" && !string.IsNullOrEmpty(model.image) && model.image.Contains(','))
            {
                model.image = model.image.Split(',')[1];
            }
            return await httpPost(JsonHelper.JsonSerialize(model), "https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/add?access_token=");
        }
        /// <summary>
        /// 人脸更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> faceUpdate(CloudModels.FaceUpdate model)
        {
            if (model.image_type == "BASE64" && !string.IsNullOrEmpty(model.image) && model.image.Contains(','))
            {
                model.image = model.image.Split(',')[1];
            }
            return await httpPost(JsonHelper.JsonSerialize(model), "https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/update?access_token=");
        }
        /// <summary>
        /// 删除人脸
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> faceDelete(CloudModels.FaceDelete model)
        {
            return await httpPost(JsonHelper.JsonSerialize(model), "https://aip.baidubce.com/rest/2.0/face/v3/faceset/face/delete?access_token=");
        }

        /// <summary>
        /// 人脸搜索
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> faceQuery(CloudModels.FaceQuery model)
        {
            if (model.image_type == "BASE64" && !string.IsNullOrEmpty(model.image) && model.image.Contains(','))
            {
                model.image = model.image.Split(',')[1];
            }
            return await httpPost(JsonHelper.JsonSerialize(model), "https://aip.baidubce.com/rest/2.0/face/v3/search?access_token=");
        }


        /// <summary>
        /// 人脸比对
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task<string> faceMatch(CloudModels.FaceMatch[] models)
        {
            if (models.Length != 2)
            {
                return "参数错误,请确保传入2组数据进行比对";
            }
            foreach(var model in models)
            {
                if (!string.IsNullOrEmpty(model.image) && model.image.Contains(','))
                {
                    model.image = model.image.Split(',')[1];
                }
            }
            string str = JsonHelper.JsonSerialize(models);
            return await httpPost(JsonHelper.JsonSerialize(models), "https://aip.baidubce.com/rest/2.0/face/v3/match?access_token=");

        }
        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="str"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public async Task<string> httpPost(string str,string host)
        {
            var Json = JObject.Parse(await getAccessToken());
            string token = Json["access_token"].ToString();
            host += token;
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;

            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            await request.GetRequestStream().WriteAsync(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string result = await reader.ReadToEndAsync();
            return result;
        }
    }
}