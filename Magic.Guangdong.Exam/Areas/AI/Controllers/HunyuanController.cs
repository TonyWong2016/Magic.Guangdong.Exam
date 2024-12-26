using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.CloudModels;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Assistant.Lib;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Hunyuan.V20230901;
using TencentCloud.Hunyuan.V20230901.Models;

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
        public async Task<IActionResult> SimpleChat(ChatModel chatModel)
        {
            if (string.IsNullOrWhiteSpace(chatModel.prompt))
                return Json(_resp.error("无输入"));
            
            try
            {
                if(string.IsNullOrEmpty(chatModel.admin))
                    chatModel.admin = adminId;
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "GetHunyuanResponse", chatModel);


                return Json(_resp.success(0, "ok"));
            }
            catch (Exception e)
            {
                Assistant.Logger.Error(e);
                return Json(_resp.error("获取响应失败，" + e.Message));
            }
        }

        
        [HttpGet("airesp")]
        public async Task AiResponseSse(string admin)
        {
            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            if (HttpContext.Request.Protocol.StartsWith("HTTP/1.1"))
            {
                Response.Headers["Connection"] = "keep-alive";
            }
            try
            {
                if (string.IsNullOrEmpty(admin))
                    admin = adminId;
                while (true)
                {
                    await Task.Delay(20);
                    // 从通道中读取消息（这里等待消息到来）
                    if (await _redisCachingProvider.KeyExistsAsync("TopThreeCacheId" + admin))
                    {
                        var topThreeMsg = await _redisCachingProvider.LPopAsync<string>("TopThreeCacheId" + admin);
                        await Response.WriteAsync($"data:{topThreeMsg}\n\n");
                        await Response.Body.FlushAsync();
                        return;   
                    }
                    //var message = await _redisCachingProvider.StringGetAsync("certProgress");
                    if (!await _redisCachingProvider.KeyExistsAsync("AfterThreeCacheId" + admin))
                        return;
                    var message = await _redisCachingProvider.LPopAsync<string>("AfterThreeCacheId" + admin);
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
        public async Task GetHunyuanResponse(ChatModel chatModel, [FromCap] CapHeader header)
        {
            try
            {
                await Task.Delay(new Random().Next(10, 100));
                if(await _redisCachingProvider.HExistsAsync(CapConsts.MsgIdCacheOaName,"hy-" + header["cap-msg-id"]))
                {
                    Assistant.Logger.Warning("以分配到其他终端");
                    return;
                }
                await _redisCachingProvider.HSetAsync(CapConsts.MsgIdCacheOaName, "hy-" + header["cap-msg-id"], header["cap-exec-instance-id"]);
                Assistant.Logger.Warning("开始请求混元接口");

                var commonParams = new HunyuanCommonParams();
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
                if (!string.IsNullOrWhiteSpace(chatModel.model))
                    req.Model = chatModel.finalModel;
                Message currMsg = new Message();
                currMsg.Role = "user";
                currMsg.Content = $"{chatModel.prompt},注意回复内容尽量控制在500字以内，不要超过1000字";
                if (chatModel.chatType)
                {
                    List<Message> myMsgLog = new List<Message>();
                    int x = 0;

                    if (await _redisCachingProvider.KeyExistsAsync($"{chatModel.admin}_msgLog"))
                    {
                        var messagesHash = await _redisCachingProvider.HGetAllAsync($"{chatModel.admin}_msgLog");
                        if (messagesHash.Count > 20)
                            await _redisCachingProvider.KeyDelAsync($"{chatModel.admin}_msgLog");
                        foreach (var item in messagesHash.OrderBy(u => u.Key))
                        {
                            myMsgLog.Add(JsonHelper.JsonDeserialize<Message>(item.Value));
                        }
                        x = messagesHash.Count + 1;
                    }

                    await _redisCachingProvider.HSetAsync($"{chatModel.admin}_msgLog", chatModel.admin + x, JsonHelper.JsonSerialize(currMsg));

                    myMsgLog.Add(currMsg);
                    //myMsgLog.Reverse();
                    //req.Messages = [message1];
                    req.Messages = myMsgLog.ToArray();
                }
                else
                    req.Messages = [currMsg];
                req.Stream = true;
                ChatCompletionsResponse resp = await client.ChatCompletions(req);
                

                // 输出json格式的字符串回包
                if (resp.IsStream)
                {
                    // 流式响应
                    int cnt = 0;
                    string listKey = "TopThreeCacheId" + chatModel.admin;
                    foreach (var e in resp)
                    {
                        Assistant.Logger.Debug(e.Data);                        
                        if (cnt > 2)
                        {
                            listKey = "AfterThreeCacheId" + chatModel.admin;
                        }
                        await _redisCachingProvider.RPushAsync(listKey, new List<string>() { e.Data });

                        cnt++;
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
                await _redisCachingProvider.KeyExpireAsync("TopThreeCacheId" + chatModel.admin, 100);
                await _redisCachingProvider.KeyExpireAsync("AfterThreeCacheId" + chatModel.admin, 600);
                await _redisCachingProvider.KeyExpireAsync($"{chatModel.admin}_msgLog",300);
            }
        }

        private string BuildChatModel(string content, string model = HunyuanModels.Lite, bool stream=true) 
        {
            return "{\"Model\":\"" + model + "\",\"Messages\":[{\"Role\":\"user\",\"Content\":\"" + content + "\"}],\"Stream\":"+ stream + "}";
        }

    }

    public class ChatModel
    {
        public string prompt {  get; set; }

        public string admin {  get; set; }

        public string model { get; set; }

        public bool chatType { get; set; } = true;

        public string initiator {  get; set; }

        public string finalModel
        {
            get
            {
                return "hunyuan-" + model;
            }
        }
    }
}
