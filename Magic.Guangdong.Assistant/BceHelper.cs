using BaiduBce;
using BaiduBce.Auth;
using BaiduBce.Services.Bos;
using BaiduBce.Services.Bos.Model;
using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Magic.Guangdong.Assistant
{
    public class BceHelper
    {
        
        #region 获取token
        /// <summary>
        /// 通过接口获取token
        /// </summary>
        /// <returns></returns>
        public string getAccessToken()
        {
            //Logger.Default.Debug("gettoken0");
            string clientId = ConfigurationHelper.GetSectionValue("BaiduNLP_AppKey");
            string clientSecret = ConfigurationHelper.GetSectionValue("BaiduNLP_Secret");
            //Logger.Default.Debug("gettoken1");
            string token = RedisHelper.Get("bd_ai_nlp_token");
            //Logger.Default.Debug("gettoken2" + token);
            string result = "";
            if (!string.IsNullOrEmpty(token))
            {
                result = token;
            }
            else
            {
                string authHost = "https://aip.baidubce.com/oauth/2.0/token";
                HttpClient client = new HttpClient();
                List<KeyValuePair<string, string>> paraList = new List<KeyValuePair<string, string>>(3);
                paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                paraList.Add(new KeyValuePair<string, string>("client_id", clientId));
                paraList.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
                HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
                result = response.Content.ReadAsStringAsync().Result;
                BdAIAuthModel model = JsonHelper.JsonDeserialize<BdAIAuthModel>(result);
                RedisHelper.Set("bd_ai_nlp_token", model.access_token, DateTime.Now.AddDays(1) - DateTime.Now);
                result = model.access_token;
            }
            //Logger.Default.Debug("gettoken3" + result);
            return result;
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        public void refreshAccessToken(int type = 1)
        {
            string clientId = ConfigurationHelper.GetSectionValue("BaiduNLP_AppKey");
            string clientSecret = ConfigurationHelper.GetSectionValue("BaiduNLP_Secret");
            string result = "";
            string authHost = "https://aip.baidubce.com/oauth/2.0/token";
            HttpClient client = new HttpClient();
            List<KeyValuePair<string, string>> paraList = new List<KeyValuePair<string, string>>(3);
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", clientId));
            paraList.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
            HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
            result = response.Content.ReadAsStringAsync().Result;
            BdAIAuthModel model = JsonHelper.JsonDeserialize<BdAIAuthModel>(result);
            RedisHelper.Set("bd_ai_nlp_token", model.access_token, DateTime.Now.AddDays(1) - DateTime.Now);
        }
        #endregion        

        #region baidu api
        /// <summary>
        /// 文字内容
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public string ContentCheck(string content)
        {
            //Logger.Default.Debug("check:1"+content);
            string token = getAccessToken();
            //Logger.Default.Debug("token:" + token);
            string host = "https://aip.baidubce.com/rest/2.0/antispam/v2/spam?access_token=" + token;
            return httpPost(host, "content=" + content, 1);
        }
        /// <summary>
        /// 语法分析
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //public string LexicalAnalysis(GrammarReqModel model)
        //{
        //    string token = getAccessToken();
        //    //string target = "https://aip.baidubce.com/rpc/2.0/nlp/v1/lexer?charset=UTF-8&access_token=" + token;
        //    string target = "https://aip.baidubce.com/rpc/2.0/nlp/v1/lexer?access_token=" + token;
        //    string str = JsonHelper.JsonSerialize(model);
        //    //return httpPost(target, str, 2);
        //    return httpPost(target, str, 3);
        //}

        public string ContentCheck2(string content)
        {
            string token = getAccessToken();
            string host = "https://aip.baidubce.com/rest/2.0/solution/v1/text_censor/v2/user_defined?access_token=" + token;
            return httpPost(host, "text=" + content, 1);
        }
        #endregion


        #region 网络请求
        /// <summary>
        /// 基础请求方法
        /// </summary>
        /// <param name="host"></param>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        private static string httpPost(string host, string jsonstr, int requestType = 0)
        {
            try
            {
                Encoding encoding = Encoding.Default;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
                request.Method = "post";
                request.KeepAlive = true;
                if (requestType == 1)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    encoding = Encoding.UTF8;
                }
                else if (requestType == 2)
                {
                    request.ContentType = "application/json;charset=utf-8";
                    encoding = Encoding.UTF8;
                }
                else if (requestType == 3)
                {
                    request.ContentType = "application/json;";
                    encoding = Encoding.GetEncoding("GBK");
                }
                byte[] buffer = encoding.GetBytes(jsonstr);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
                string result = reader.ReadToEnd();
                Logger.Debug("result:" + result);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("error:" + ex.Message);
                return "error:" + ex.Message;

            }
        }
        #endregion

        #region sdk 方法
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string UploadToBos(string filePath)
        {
            if (File.Exists(filePath))
            {
                FileInfo file = new FileInfo(filePath);
                if (file.Length / (1024 * 1024) < 100)
                {
                    //100M以内的直接推
                    return SingleUploadToBos(file);
                }
                else
                {
                    //大于100M的分块推
                    return MultiUploadToBos(file);
                }
            }
            else
                return "error";
        }


        /// <summary>
        /// 单独上传
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string SingleUploadToBos(FileInfo file)
        {
            try
            {
                BosClient client = GenerateBosClient();
                //FileInfo file = new FileInfo(filename);
                Stream inputStream = file.OpenRead();
                string filename = file.Name;
                PutObjectResponse putObjectFromFileResponse = client.PutObject("xiaoxiaotong", filename, inputStream);
                file.Delete();
                return GeneratePresignedUrl(client, "xiaoxiaotong", filename, -1);
            }
            catch (Exception ex)
            {
                Logger.Error($"同步至bos出错，error:{ex.Message},{ex.StackTrace}");
                throw;
            }
        }
        /// <summary>
        /// 小文件独立上传
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string UploadFileToBosSingle(string fileName, Stream data)
        {
            try
            {
                BosClient client = GenerateBosClient();
                PutObjectResponse putObjectFromFileResponse = client.PutObject("xiaoxiaotong", fileName, data);
                string url = GeneratePresignedUrl(client, "xiaoxiaotong", fileName, -1);
                url = url.Replace(".bd.", ".cdn.");//通过cdn下载
                return url;
            }
            catch (Exception ex)
            {
                Logger.Error($"同步至bos出错，error:{ex.Message},{ex.StackTrace}");
                return "";
            }
        }
        /// <summary>
        /// 获取链接
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bucketName"></param>
        /// <param name="objectKey"></param>
        /// <param name="expirationInSeconds"></param>
        /// <returns></returns>
        public static string GeneratePresignedUrl(BosClient client, string bucketName, string objectKey, int expirationInSeconds)
        {
            //指定用户需要获取的Object所在的Bucket名称、该Object名称、时间戳、URL的有效时长
            Uri url = client.GeneratePresignedUrl(bucketName, objectKey, expirationInSeconds);
            return url.AbsoluteUri;
        }

        /// <summary>
        /// 分块上传
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string MultiUploadToBos(FileInfo partFile)
        {
            
            try
            {
                BosClient client = GenerateBosClient();
                const string bucketName = "xiaoxiaotong"; //示例Bucket名称

                // 初始化：创建示例Bucket                
                //string objectKey = Path.GetFileName(filename);
                string objectKey = partFile.Name;
                // 1.开始Multipart Upload
                InitiateMultipartUploadRequest initiateMultipartUploadRequest =
                    new InitiateMultipartUploadRequest() { BucketName = bucketName, Key = objectKey };
                InitiateMultipartUploadResponse initiateMultipartUploadResponse =
                    client.InitiateMultipartUpload(initiateMultipartUploadRequest);

                // 2.获取Bucket内的Multipart Upload
                //ListMultipartUploadsRequest listMultipartUploadsRequest =
                //    new ListMultipartUploadsRequest() { BucketName = bucketName };
                //ListMultipartUploadsResponse listMultipartUploadsResponse =
                //    client.ListMultipartUploads(listMultipartUploadsRequest);
                //foreach (MultipartUploadSummary multipartUpload in listMultipartUploadsResponse.Uploads)
                //{
                //    NLogUtil.fileLogger.Debug("Key: " + multipartUpload.Key + " UploadId: " + multipartUpload.UploadId);
                //}

                // 3.分块上传，首先设置每块为 5Mb
                long partSize = 1024 * 1024 * 5L;
                //FileInfo partFile = new FileInfo(filename);
                // 计算分块数目
                int partCount = (int)(partFile.Length / partSize);
                if (partFile.Length % partSize != 0)
                {
                    partCount++;
                }
                // 新建一个List保存每个分块上传后的ETag和PartNumber
                List<PartETag> partETags = new List<PartETag>();
                for (int i = 0; i < partCount; i++)
                {
                    // 获取文件流
                    Stream stream = partFile.OpenRead();
                    // 跳到每个分块的开头
                    long skipBytes = partSize * i;
                    stream.Seek(skipBytes, SeekOrigin.Begin);
                    // 计算每个分块的大小
                    long size = Math.Min(partSize, partFile.Length - skipBytes);
                    // 创建UploadPartRequest，上传分块
                    UploadPartRequest uploadPartRequest = new UploadPartRequest();
                    uploadPartRequest.BucketName = bucketName;
                    uploadPartRequest.Key = objectKey;
                    uploadPartRequest.UploadId = initiateMultipartUploadResponse.UploadId;
                    uploadPartRequest.InputStream = stream;
                    uploadPartRequest.PartSize = size;
                    uploadPartRequest.PartNumber = i + 1;
                    UploadPartResponse uploadPartResponse = client.UploadPart(uploadPartRequest);
                    // 将返回的PartETag保存到List中。
                    partETags.Add(new PartETag()
                    {
                        ETag = uploadPartResponse.ETag,
                        PartNumber = uploadPartResponse.PartNumber
                    });
                    // 关闭文件
                    stream.Close();
                }

                // 4. 获取UploadId的所有Upload Part
                ListPartsRequest listPartsRequest = new ListPartsRequest()
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    UploadId = initiateMultipartUploadResponse.UploadId,
                };
                // 获取上传的所有Part信息
                ListPartsResponse listPartsResponse = client.ListParts(listPartsRequest);
                // 遍历所有Part
                foreach (PartSummary part in listPartsResponse.Parts)
                {
                    Logger.Debug("PartNumber: " + part.PartNumber + " ETag: " + part.ETag);
                }

                // 5. 完成分块上传
                CompleteMultipartUploadRequest completeMultipartUploadRequest =
                    new CompleteMultipartUploadRequest()
                    {
                        BucketName = bucketName,
                        Key = objectKey,
                        UploadId = initiateMultipartUploadResponse.UploadId,
                        PartETags = partETags
                    };
                CompleteMultipartUploadResponse completeMultipartUploadResponse =
                    client.CompleteMultipartUpload(completeMultipartUploadRequest);
                Logger.Debug(completeMultipartUploadResponse.ETag);
                return GeneratePresignedUrl(client, "xiaoxiaotong", partFile.Name, -1);

            }
            catch (BceServiceException ex)
            {
                Logger.Error($"同步至bos失败，{ex.Message},{ex.StackTrace},{ex.ErrorMessage},{ex.Data}");
                throw;
            }
        }

        /// <summary>
        /// 删除bos上的资源
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName"></param>
        public static bool DeleteFileFromBos(string bucketName, string objectName)
        {
            try
            {
                BosClient client = GenerateBosClient();
                // 删除Object
                client.DeleteObject(bucketName, objectName);
            }
            catch (Exception ex)
            {
                //Common.LogHelper.WriteLog("deleteBosObjectError:fileName" + objectName + ",error:" + ex.Message);
                Logger.Error("deleteBosObjectError:fileName" + objectName + ",error:" + ex.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 初始化bosclient
        /// </summary>
        /// <returns></returns>
        public static BosClient GenerateBosClient()
        {
            string accessKeyId = ConfigurationHelper.GetSectionValue("BaiduBos_Key"); // 用户的Access Key ID
            string secretAccessKey = ConfigurationHelper.GetSectionValue("BaiduBos_Secret"); ; // 用户的Secret Access Key
            string endpoint = ConfigurationHelper.GetSectionValue("BaiduBos_Endpoint"); // 指定BOS服务域名

            // 初始化一个BosClient
            BceClientConfiguration config = new BceClientConfiguration();
            config.Credentials = new DefaultBceCredentials(accessKeyId, secretAccessKey);
            config.Endpoint = endpoint;

            return new BosClient(config);
        }
        #endregion
    }

    /// <summary>
    /// token模型
    /// </summary>
    public class BdAIAuthModel
    {
        public string refresh_token { get; set; }

        public string expires_in { get; set; }

        public string scope { get; set; }

        public string session_key { get; set; }

        public string access_token { get; set; }

        public string session_secret { get; set; }
    }

    /// <summary>
}
