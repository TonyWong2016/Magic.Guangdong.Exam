using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.CloudModels;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Assistant.Lib;
using Magic.Guangdong.Exam.Areas.AI.Functions;
using Magic.Guangdong.Exam.Areas.AI.Models;
using Magic.Guangdong.Exam.LLM.Plugins.Checking;
using Magic.Guangdong.Exam.LLM.Plugins.Simple;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities.Collections;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Hunyuan.V20230901;
using TencentCloud.Hunyuan.V20230901.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        private readonly ITest _test;
        private string adminId = "";
        private Credential _cred;
        private readonly Kernel _kernel;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _en;
        //private readonly Tools.SseMiddleware _sseMiddleware;
        public MagicController(IResponseHelper responseHelper,
            AiConfigFactory aiConfigFactory, 
            IRedisCachingProvider redisCachingProvider,
            IHttpContextAccessor contextAccessor, 
            ICapPublisher capPublisher,
            Kernel kernel,
            IServiceProvider serviceProvider,
            IWebHostEnvironment en,
            ITest test)
        {
            _resp = responseHelper;
            _aiConfigsHunyuan = aiConfigFactory.GetConfigByModel("hunyuan");
            _aiConfigsDeepSeek = aiConfigFactory.GetConfigByModel("deepseek");
            _redisCachingProvider = redisCachingProvider;
            _contextAccessor = contextAccessor;
            _capPublisher = capPublisher;
            _serviceProvider = serviceProvider;
            _kernel = kernel;
            _en = en;
            _test = test;
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

                if(await _redisCachingProvider.KeyExistsAsync("chat_" + chatModel.admin))
                {
                    return Json(_resp.error("当前用户正在其他终端进行AI对话，请使用其他账号进行测试"));
                }
                await _redisCachingProvider.StringSetAsync("chat_" + chatModel.admin, JsonHelper.JsonSerialize(chatModel), TimeSpan.FromMinutes(3));

                if (chatModel.messages.Length > 500)
                {
                    await _redisCachingProvider.StringSetAsync("msglog_" + chatModel.admin, chatModel.messages, TimeSpan.FromSeconds(300));

                    chatModel.messages = "";
                }
                if (chatModel.model.Contains("deepseek"))
                {
                    await _capPublisher.PublishAsync(CapConsts.PREFIX + "GetMagicDeepSeekResponse2", chatModel);
                }
                else
                {
                    await _capPublisher.PublishAsync(CapConsts.PREFIX + "GetMagicHunyuanResponse", chatModel);
                }
                return Json(_resp.success(0, "ok"));
            }
            catch (Exception e)
            {
                Assistant.Logger.Error(e);
                return Json(_resp.error("获取响应失败，" + e.Message));
            }
        }

        /// <summary>
        /// 通过对话的方式完成内容的审核
        /// </summary>
        /// <returns></returns>
        //[HttpPost,ValidateAntiForgeryToken]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CheckingChat(ChatForChecking chatModel)
        {
            if (string.IsNullOrWhiteSpace(chatModel.prompt))
                return Json(_resp.error("无输入"));
            _kernel.Plugins.AddFromType<GetReportInfo>(nameof(GetReportInfo), _serviceProvider);
            _kernel.Plugins.AddFromType<Notice>(nameof(Notice), _serviceProvider);
            // 获取聊天完成服务
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            // 启用自动函数调用
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                //ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            //PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
            ChatHistory history = [];
            history.AddSystemMessage($"你是一个材料审查员，需要用到2个参数，一个从数据库读取的，完整的，结构化的申报信息参数，包括赛队编号，赛项，成员，学校，指导教师，邮箱等，另一个是通过文档识别接口识别出来的材料信息，里面逐行输出了提取的材料信息和置信度，" +
                $"这个材料是用户提交的附件信息，里面包含了全部或者部分第一个参数里的信息。" +
                $"你的工作如下：" +
                $"1.从对话信息中提取赛队编号信息，并调用本地插件进行检索，注意，赛队编号格式是2个连续大写字母加8为数字，如：AA24091234，其中第一位大写字母只能是A，B，C，D，前两位数字代表年份，如24代表2024年，23代表2023年；如格式错误则直接返回异常输入，后续工作不必再继续执行；" +
                $"2.如果步骤1返回了结果，那么整个结果将作为第一个输入参数，而其中有一个FileName属性，表示了文档材料的保存路径，使用该属性值再次调用合适的本地插件，识别文档信息，作为第二个输入参数；" +
                $"3.如果步骤2执行正常，将2个参数进行比对，如果你认为第二个参数归属于第一个参数的概率很高，也就是该材料提交的信息是正确的，那么通过第一个参数中的Email属性，调用合适的本地插件发送通知邮件，告知他材料审核通过；反之,那就输出“我无法确认该材料真实性，请联系人工审查员处理”" +
                $"注意，每个指令只执行1次，1次不成功就终止，不要重试；"
               );
            if (chatModel.messages!=null && chatModel.messages.Length != 0)
            {
                foreach(var item in chatModel.messages)
                {
                    AuthorRole role = AuthorRole.User;
                    if (item.role.Equals("assistant",StringComparison.OrdinalIgnoreCase))
                        role = AuthorRole.Assistant;
                    else if (item.role.Equals("tool", StringComparison.OrdinalIgnoreCase))
                        role = AuthorRole.Tool;
                    history.AddMessage(role, item.message);
                }
            }
            history.AddUserMessage(chatModel.prompt);
            var chatResult = await chatCompletionService.GetChatMessageContentAsync(
                history,
                 executionSettings: openAIPromptExecutionSettings,
                _kernel);

            Console.Write($"\nAssistant : {chatResult}\n");

            return Json(chatResult);
        }
        

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearChat(string admin)
        {
            await _redisCachingProvider.KeyDelAsync("TopServalCacheId" + admin);
            await _redisCachingProvider.KeyDelAsync("AfterServalCacheId" + admin);
            await _redisCachingProvider.KeyDelAsync("msglog_" + admin);
            await _redisCachingProvider.KeyDelAsync("chat_" + admin);
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
                    {
                        await _redisCachingProvider.KeyDelAsync("chat_" + admin);
                        await Response.WriteAsync($"data:[DONE]\n\n");
                        await Response.Body.FlushAsync();
                        return;
                    }
                    var message = await _redisCachingProvider.LPopAsync<string>("AfterServalCacheId" + admin);
                    if (string.IsNullOrEmpty(message))
                        continue;
                    // 按照SSE协议格式发送数据到客户端
                    await Response.WriteAsync($"data:{message}\n\n");
                    await Response.Body.FlushAsync();

                    //await Task.Run(() =>
                    //{
                    //    ParseResponseCall(message);
                    //    //Task.Delay(1);
                    //});

                }

            }
            catch (Exception ex)
            {
                // 可以记录异常等处理
                Console.WriteLine(ex.Message);
            }
        }

        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "GetMagicHunyuanResponse")]
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
                    req.Model = chatModel.HunyuanModel;

                if (string.IsNullOrEmpty(chatModel.messages) && await _redisCachingProvider.KeyExistsAsync("msglog_" + chatModel.admin))
                {
                    chatModel.messages = await _redisCachingProvider.StringGetAsync("msglog_" + chatModel.admin);
                }

                List<Message> myMsgLog =
                [
                    new Message()
                    {
                        Role = "system",
                        Content = "You are a helpful assistant.注意以下回复内容尽量控制在500字以内，不要超过1000字"
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
        [CapSubscribe(CapConsts.PREFIX + "GetMagicDeepSeekResponse2")]
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
                        Logger.Warning(line);
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
                else
                {
                    var responseContent = await resp.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var toolCalls = jsonResponse.GetProperty("tool_calls"); // 假设API返回包含"tool_calls"字段
                    foreach (var toolCall in toolCalls.EnumerateArray())
                    {
                        var functionName = toolCall.GetProperty("function_name").GetString();
                        var rawParameters = toolCall.GetProperty("parameters").GetRawText();

                        switch (functionName)
                        {
                            case "getweather":
                                var funcAParams = JsonSerializer.Deserialize<WeatherInput>(rawParameters);
                                var resultFuncA = _test.GetWeather(funcAParams);
                                responseContent = responseContent.Replace("\"content\":\"\"", "\"content\":" + resultFuncA + "");
                                // 根据需要处理结果
                                break;
                            case "getuserrecord":
                                var funcBParams = JsonSerializer.Deserialize<UserRecordInput>(rawParameters);
                                var resultFuncB = await _test.GetUserRecord(funcBParams);
                                responseContent = responseContent.Replace("\"content\":\"\"", "\"content\":" + resultFuncB + "");
                                // 处理结果
                                break;
                            
                        }
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
                await _redisCachingProvider.KeyDelAsync("chat_" + chatModel.admin);
            }
            //return Json(_resp.success(await response.Content.ReadAsStringAsync()));

        }

        private string BuildDeepSeekChatParmm(ChatModel chatModel)
        {
            var ds = new DeepSeekRequestModel();
            ds.stream = chatModel.isStream;
            
            List<DeepSeekMessages> listMsg =
                [
                    new DeepSeekMessages()
                    {
                        role = "system",
                        content = "You are a helpful assistant.注意以下回复内容尽量控制在500字以内，如非必要，不要超过1000字"
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

            if (chatModel.tools!=null && chatModel.tools.Any(u => u.type != "chat"))
            {
                ds.tools = chatModel.tools.Adapt<DsTool[]>();
                ds.tool_choice = "auto";
                
            }
            return JsonHelper.JsonSerialize(ds);
        }

        public List<DsTool> BuildFunction(ChatModel chatModel)
        {
            List<DsTool> listTool = new List<DsTool>();
            foreach (var tool in chatModel.tools)
            {
                //var functionCall = new Assistant.CloudModels.DsToolFunction()
                //{
                //    name = tool.toolFunction.name,
                //    description = tool.toolFunction.description,
                //    parameter = 
                //    new
                //    {
                //        type = "object",
                //        properties = new Dictionary<string, object>
                //        {
                //            ["location"] = new { type = "string", description = "The city and state, e.g., San Francisco, CA" },

                //        },
                //        required = new[] { "location" }
                //    }
                //};
                listTool.Add(new DsTool()
                {
                    type = tool.type,
                    function = tool.toolFunction
                });
            }
            return listTool;
        }



        
    }
}
