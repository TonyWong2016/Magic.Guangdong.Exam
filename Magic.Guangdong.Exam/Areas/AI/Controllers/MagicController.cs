using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant.CloudModels;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Assistant.Lib;
using Magic.Guangdong.Assistant;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TencentCloud.Common.Profile;
using TencentCloud.Hunyuan.V20230901.Models;
using TencentCloud.Hunyuan.V20230901;
using TencentCloud.Common;
using Azure;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Magic.Guangdong.Exam.Areas.AI.Controllers
{
    [Area("ai")]
    public class MagicController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly AiConfig _aiConfigsHunyuan;
        private readonly AiConfig _aiConfigsDeepSeek;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICapPublisher _capPublisher;

        private string adminId = "";
        private Credential _cred;
        //private readonly Tools.SseMiddleware _sseMiddleware;
        public MagicController(IResponseHelper responseHelper, AiConfigFactory aiConfigFactory, IRedisCachingProvider redisCachingProvider, IHttpContextAccessor contextAccessor, ICapPublisher capPublisher)
        {
            _resp = responseHelper;
            _aiConfigsHunyuan = aiConfigFactory.GetConfigByModel("hunyuan");
            _aiConfigsDeepSeek = aiConfigFactory.GetConfigByModel("deepseek");
            _redisCachingProvider = redisCachingProvider;
            _contextAccessor = contextAccessor;
            _capPublisher = capPublisher;
            if (_contextAccessor.HttpContext != null && _contextAccessor.HttpContext.Request.Cookies.TryGetValue("userId", out string cookieValue))
            {
                adminId = Utils.FromBase64Str(cookieValue);
            }
            _cred = new Credential
            {
                SecretId = _aiConfigsHunyuan.SecretId,
                SecretKey = _aiConfigsHunyuan.SecretKey,
            };
        }
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet("simplechat")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SimpleChat(ChatModel chatModel)
        {
            if (string.IsNullOrWhiteSpace(chatModel.prompt))
                return Json(_resp.error("无输入"));

            try
            {
                if (string.IsNullOrEmpty(chatModel.admin))
                    chatModel.admin = adminId;

                if (chatModel.messages.Length > 500)
                {
                    await _redisCachingProvider.StringSetAsync("msglog_" + chatModel.admin, chatModel.messages, TimeSpan.FromSeconds(300));

                    chatModel.messages = "";
                }
                if (chatModel.model.Contains("deepseek"))
                {
                    await _capPublisher.PublishAsync(CapConsts.PREFIX + "GetMagicDeepSeekResponse1", chatModel);
                }
                else
                {
                    await _capPublisher.PublishAsync(CapConsts.PREFIX + "GetMagicHunyuanResponse1", chatModel);
                }
                return Json(_resp.success(0, "ok"));
            }
            catch (Exception e)
            {
                Assistant.Logger.Error(e);
                return Json(_resp.error("获取响应失败，" + e.Message));
            }
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearChat(string admin)
        {
            await _redisCachingProvider.KeyDelAsync("TopServalCacheId" + admin);
            await _redisCachingProvider.KeyDelAsync("AfterServalCacheId" + admin);
            await _redisCachingProvider.KeyDelAsync("msglog_" + admin);
            return Json(_resp.success(true));
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
                    if (await _redisCachingProvider.KeyExistsAsync("TopServalCacheId" + admin))
                    {
                        var topServalMsg = await _redisCachingProvider.LPopAsync<string>("TopServalCacheId" + admin);
                        await Response.WriteAsync($"data:{topServalMsg}\n\n");
                        await Response.Body.FlushAsync();
                        return;
                    }
                    //var message = await _redisCachingProvider.StringGetAsync("certProgress");
                    if (!await _redisCachingProvider.KeyExistsAsync("AfterServalCacheId" + admin))
                        return;
                    var message = await _redisCachingProvider.LPopAsync<string>("AfterServalCacheId" + admin);
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
        [CapSubscribe(CapConsts.PREFIX + "GetMagicHunyuanResponse1")]
        public async Task GetHunyuanResponse(ChatModel chatModel, [FromCap] CapHeader header)
        {
            try
            {
                await Task.Delay(new Random().Next(10, 100));
                if (await _redisCachingProvider.HExistsAsync(CapConsts.MsgIdCacheOaName, "hy-" + header["cap-msg-id"]))
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

                if (string.IsNullOrEmpty(chatModel.messages) && await _redisCachingProvider.KeyExistsAsync("msglog_" + chatModel.admin))
                {
                    chatModel.messages = await _redisCachingProvider.StringGetAsync("msglog_" + chatModel.admin);
                }

                List<Message> myMsgLog =
                [
                    new Message()
                    {
                        Role = "system",
                        Content = "注意以下回复内容尽量控制在500字以内，不要超过1000字"
                    },
                ];
                if (!string.IsNullOrEmpty(chatModel.messages))
                {
                    myMsgLog = JsonHelper.JsonDeserialize<List<Message>>(chatModel.messages);
                }

                myMsgLog.Add(new Message()
                {
                    Role = "user",
                    Content = $"{chatModel.prompt}"
                });
                req.Messages = myMsgLog.ToArray();

                Assistant.Logger.Debug(JsonHelper.JsonSerialize(req));

                req.Stream = true;
                ChatCompletionsResponse resp = await client.ChatCompletions(req);

                // 输出json格式的字符串回包
                if (resp.IsStream)
                {
                    // 流式响应
                    int cnt = 0;
                    string listKey = "TopServalCacheId" + chatModel.admin;
                    foreach (var e in resp)
                    {
                        //Assistant.Logger.Debug(e.Data);
                        if (cnt > 5)
                        {
                            listKey = "AfterServalCacheId" + chatModel.admin;
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
                await _redisCachingProvider.KeyExpireAsync("TopServalCacheId" + chatModel.admin, 100);
                await _redisCachingProvider.KeyExpireAsync("AfterServalCacheId" + chatModel.admin, 600);
                await _redisCachingProvider.KeyDelAsync("msglog_" + chatModel.admin);
            }
        }


        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "GetMagicDeepSeekResponse1")]
        public async Task GetDeepSeekResponse(ChatModel chatModel)
        {
            try
            {
                if (string.IsNullOrEmpty(chatModel.prompt))
                    return;
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _aiConfigsDeepSeek.BaseUrl);
                request.Headers.Add("Accept", "application/json");
                
                request.Headers.Add("Authorization", $"Bearer {_aiConfigsDeepSeek.ApiKey}");
                if (string.IsNullOrEmpty(chatModel.messages) && await _redisCachingProvider.KeyExistsAsync("msglog_" + chatModel.admin))
                {
                    chatModel.messages = await _redisCachingProvider.StringGetAsync("msglog_" + chatModel.admin);
                }
                //var content = new StringContent("{\n  \"messages\": [\n    {\n      \"content\": \"You are a helpful assistant\",\n      \"role\": \"system\"\n    },\n    {\n      \"content\": \"Hi\",\n      \"role\": \"user\"\n    }\n  ],\n  \"model\": \"deepseek-chat\",\n  \"frequency_penalty\": 0,\n  \"max_tokens\": 2048,\n  \"presence_penalty\": 0,\n  \"response_format\": {\n    \"type\": \"text\"\n  },\n  \"stop\": null,\n  \"stream\": false,\n  \"stream_options\": null,\n  \"temperature\": 1,\n  \"top_p\": 1,\n  \"tools\": null,\n  \"tool_choice\": \"none\",\n  \"logprobs\": false,\n  \"top_logprobs\": null\n}", null, "application/json");
                Logger.Debug(BuildDeepSeekChatParmm(chatModel));
                request.Content = new StringContent(BuildDeepSeekChatParmm(chatModel),Encoding.UTF8,"application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using var resp = await client.SendAsync(request);
                resp.EnsureSuccessStatusCode();
                if (chatModel.isStream)
                {
                    // 读取流式响应
                    using var stream = await resp.Content.ReadAsStreamAsync();
                    using var reader = new StreamReader(stream);
                    int cnt = 0;
                    string listKey = "TopServalCacheId" + chatModel.admin;
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        
                        if (string.IsNullOrEmpty(line))
                            continue;
                       // Assistant.Logger.Debug(line);
                        if (cnt > 5)
                        {
                            listKey = "AfterServalCacheId" + chatModel.admin;
                        }
                        if (line.StartsWith("data:"))
                            line = line.Substring(5).Trim();
                        if (string.IsNullOrEmpty(line))
                            continue;
                        if (line.Contains("DONE"))
                            break;
                        await _redisCachingProvider.RPushAsync(listKey, new List<string>() { line });

                        cnt++;
                    }
                }
            }
            catch (Exception ex)
            {
                Assistant.Logger.Error(ex);
            }
            finally
            {
                await _redisCachingProvider.KeyExpireAsync("TopServalCacheId" + chatModel.admin, 100);
                await _redisCachingProvider.KeyExpireAsync("AfterServalCacheId" + chatModel.admin, 600);
                await _redisCachingProvider.KeyDelAsync("msglog_" + chatModel.admin);
            }
            //return Json(_resp.success(await response.Content.ReadAsStringAsync()));

        }



        private string BuildDeepSeekChatParmm(ChatModel chatModel)
        {
            var ds = new DeepSeekRequestModel();
            
            List<DeepSeekMessages> listMsg =
                [
                    new DeepSeekMessages()
                    {
                        role = "system",
                        content = "注意以下回复内容尽量控制在500字以内，如非必要，不要超过1000字"
                    },
                ];
            if (!string.IsNullOrEmpty(chatModel.messages))
            {
                listMsg.AddRange(JsonHelper.JsonDeserialize<List<DeepSeekMessages>>(chatModel.messages));
            }
            listMsg.Add(new DeepSeekMessages()
            {
                role = "user",
                content = chatModel.prompt
            });
            ds.messages = listMsg.ToArray();

            return JsonHelper.JsonSerialize(ds);
        }
    }
}
