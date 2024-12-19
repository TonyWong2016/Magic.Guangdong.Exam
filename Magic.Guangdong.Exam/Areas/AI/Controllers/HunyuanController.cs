using Microsoft.AspNetCore.Mvc;
using TencentCloud.Common;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Assistant.CloudModels;
using Magic.Guangdong.Assistant.Lib;
using TencentCloud.Common.Profile;
using TencentCloud.Hunyuan.V20230901.Models;
using TencentCloud.Hunyuan.V20230901;
using Newtonsoft.Json;
using Azure;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Org.BouncyCastle.Ocsp;
using Microsoft.Identity.Client;
using DotNetCore.CAP;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Exam.Areas.Cert.Controllers;

namespace Magic.Guangdong.Exam.Areas.AI.Controllers
{
    [Area("ai")]
    public class HunyuanController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly AiConfig _aiConfigs;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICapPublisher _capPublisher;

        private string adminId = "";
        private Credential _cred;
        //private readonly Tools.SseMiddleware _sseMiddleware;
        public HunyuanController(IResponseHelper responseHelper, AiConfigFactory aiConfigFactory,IRedisCachingProvider redisCachingProvider, IHttpContextAccessor contextAccessor,ICapPublisher capPublisher) 
        {
            _resp = responseHelper;
            _aiConfigs = aiConfigFactory.GetConfigByModel("hunyuan");
            _redisCachingProvider = redisCachingProvider;
            _contextAccessor = contextAccessor;
            _capPublisher = capPublisher;
            if (_contextAccessor.HttpContext != null && _contextAccessor.HttpContext.Request.Cookies.TryGetValue("userId", out string cookieValue))
            {
                adminId = Utils.FromBase64Str(cookieValue);
            }
            _cred = new Credential
            {
                SecretId = _aiConfigs.SecretId,
                SecretKey = _aiConfigs.SecretKey,
            };
        }
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet("simplechat")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> SimpleChat(string prompt,string model="")
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return Json(_resp.error("无输入"));
            
            try
            {
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "GetHunyuanResponse", $"{prompt}|{adminId}");


                return Json(_resp.success(0, "ok"));
            }
            catch (Exception e)
            {
                Assistant.Logger.Error(e);
                return Json(_resp.error("获取响应失败，" + e.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ComplexChat(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return Json(_resp.error("无输入"));

            try
            {
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "GetHunyuanResponse", $"{prompt}|{adminId}");


                return Json(_resp.success(0, "ok"));
            }
            catch (Exception e)
            {
                Assistant.Logger.Error(e);
                return Json(_resp.error("获取响应失败，" + e.Message));
            }
        }

        [HttpGet("airesp")]
        public async Task AiResponseSse()
        {
            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            if (HttpContext.Request.Protocol.StartsWith("HTTP/1.1"))
            {
                Response.Headers["Connection"] = "keep-alive";
            }
            try
            {
                while (true)
                {
                    //await Task.Delay(100);
                    // 从通道中读取消息（这里等待消息到来）
                    //var message = await _redisCachingProvider.StringGetAsync("certProgress");
                    if (!await _redisCachingProvider.KeyExistsAsync("cacheId" + adminId))
                        return;
                    var message = await _redisCachingProvider.LPopAsync<string>("cacheId" + adminId);
                    if (string.IsNullOrEmpty(message))
                        continue;
                    // 按照SSE协议格式发送数据到客户端
                    await Response.WriteAsync($"data:{message}\n\n");
                    await Response.Body.FlushAsync();

                }

            }
            catch (Exception ex)
            {
                // 可以记录异常等处理
                Console.WriteLine(ex.Message);
            }
        }

        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "GetHunyuanResponse")]
        public async Task GetHunyuanResponse(string promptAndAdminId)
        {
            try
            {
                string[] parts = promptAndAdminId.Split('|');
                string prompt = parts[0];
                adminId = parts[1];
                Assistant.Logger.Warning("开始请求混元接口");
                var commonParams = new HunyuanCommonParams();
                //var cred = new Credential
                //{
                //    SecretId = _aiConfigs.SecretId,
                //    SecretKey = _aiConfigs.SecretKey,
                //};
                // 实例化一个client选项，可选的，没有特殊需求可以跳过
                ClientProfile clientProfile = new ClientProfile();
                // 实例化一个http选项，可选的，没有特殊需求可以跳过
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = commonParams.Endpoint;
                clientProfile.HttpProfile = httpProfile;

                // 实例化要请求产品的client对象,clientProfile是可选的
                HunyuanClient client = new HunyuanClient(_cred, commonParams.Region, clientProfile);
                // 实例化一个请求对象,每个接口都会对应一个request对象
                ChatCompletionsRequest req = new ChatCompletionsRequest();
                req.Model = HunyuanModels.Lite;
                Message message1 = new Message();
                message1.Role = "user";
                message1.Content = prompt;
                req.Messages = new Message[] { message1 };
                req.Stream = true;
                ChatCompletionsResponse resp = await client.ChatCompletions(req);
                // 输出json格式的字符串回包
                if (resp.IsStream)
                {
                    // 流式响应
                    foreach (var e in resp)
                    {
                        Assistant.Logger.Debug(e.Data);
                        await _redisCachingProvider.RPushAsync("cacheId" + adminId, new List<string>() { e.Data });
                        //await _sseMiddleware.BroadcastMessage(e.Data);
                    }
                }
                else
                {
                    // 非流式响应
                    Assistant.Logger.Debug(JsonConvert.SerializeObject(resp));
                }
            }
            catch (Exception ex)
            {
               Assistant.Logger.Error(ex);
            }
            finally
            {
                await _redisCachingProvider.KeyExpireAsync("cacheId" + adminId, 600);
            }
        }

        private string BuildChatModel(string content, string model = HunyuanModels.Lite, bool stream=true) 
        {
            return "{\"Model\":\"" + model + "\",\"Messages\":[{\"Role\":\"user\",\"Content\":\"" + content + "\"}],\"Stream\":"+ stream + "}";
        }

    }
}
