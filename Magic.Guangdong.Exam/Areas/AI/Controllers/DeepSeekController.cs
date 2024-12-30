using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.CloudModels;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Assistant.Lib;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.AI.Controllers
{
    public class DeepSeekController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly AiConfig _aiConfigs;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICapPublisher _capPublisher;
        public DeepSeekController(IResponseHelper responseHelper, AiConfigFactory aiConfigFactory,IRedisCachingProvider redisCachingProvider, IHttpContextAccessor contextAccessor,ICapPublisher capPublisher)
        {
            _resp = responseHelper;
            _aiConfigs = aiConfigFactory.GetConfigByModel("deepseek");
            _redisCachingProvider = redisCachingProvider;
            _contextAccessor = contextAccessor;
            _capPublisher = capPublisher;
        }
        private string adminId = "";

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SimpleChat(ChatModel chatModel)
        {
            if(string.IsNullOrEmpty(chatModel.prompt))
                return Json(_resp.error("无有效输入"));
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, _aiConfigs.BaseUrl);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {_aiConfigs.ApiKey}");
            //var content = new StringContent("{\n  \"messages\": [\n    {\n      \"content\": \"You are a helpful assistant\",\n      \"role\": \"system\"\n    },\n    {\n      \"content\": \"Hi\",\n      \"role\": \"user\"\n    }\n  ],\n  \"model\": \"deepseek-chat\",\n  \"frequency_penalty\": 0,\n  \"max_tokens\": 2048,\n  \"presence_penalty\": 0,\n  \"response_format\": {\n    \"type\": \"text\"\n  },\n  \"stop\": null,\n  \"stream\": false,\n  \"stream_options\": null,\n  \"temperature\": 1,\n  \"top_p\": 1,\n  \"tools\": null,\n  \"tool_choice\": \"none\",\n  \"logprobs\": false,\n  \"top_logprobs\": null\n}", null, "application/json");

            request.Content = new StringContent(BuildChatParmm(chatModel));
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return Json(_resp.success(await response.Content.ReadAsStringAsync()));

        }

        private string BuildChatParmm(ChatModel chatModel)
        {
            var ds = new DeepSeekRequestModel();
            ds.model = _aiConfigs.Model;
            List<DeepSeekMessages> listMsg = new List<DeepSeekMessages>();
            if (!string.IsNullOrEmpty(chatModel.messages))
            {
                listMsg = JsonHelper.JsonDeserialize<List<DeepSeekMessages>>(chatModel.messages);
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
