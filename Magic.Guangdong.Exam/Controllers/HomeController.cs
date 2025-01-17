using DotNetCore.CAP;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Exam.Configs.Plugins.SimplePlugins;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;

namespace Magic.Guangdong.Exam.Controllers
{
    [RouteMark("测试")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICapPublisher _capPublisher;
        private readonly Kernel _kernel;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _en;
        public HomeController(ILogger<HomeController> logger,ICapPublisher capPublisher,Kernel kernel, IWebHostEnvironment en, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _capPublisher = capPublisher;            
            _kernel = kernel.Clone();
            _serviceProvider = serviceProvider;
            _en = en;
        }
        [RouteMark("测试1")]
        public async Task<IActionResult> Index()
        {
            //var test = await _recordRepo.getAnyAsync(u => u.IsDeleted == 0);
            //int i = 0;
            //while ( i < 3){
            //    Assistant.Logger.Warning($"生产消息：" + DateTime.Now + i.ToString());
            //    _capPublisher.Publish(CapConsts.PREFIX + "TEST", $"第{(i+1).ToString()}条，{DateTime.Now}");
            //    i++;
            //}

            //_kernel.Plugins.AddFromType<TimeInformationPlugin>();
            _kernel.Plugins.AddFromType<RecordSearchPlugin>("RecordSearch", _serviceProvider);
            // 获取聊天完成服务
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            
            // 启用自动函数调用
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {  
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                
            };

            PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
            ChatHistory chatHistory = []; 
            string? input = null;
            string testPath = Path.Combine($"{_en.WebRootPath}", "upfile", "202411", "3", "test.jpg");
            byte[] bytes = System.IO.File.ReadAllBytes(testPath);

            //chatHistory = new ChatHistory("Your job is describing images.");
            //chatHistory.AddUserMessage([
            //    new TextContent("这是什么图?"),
            //    new ImageContent(bytes, "image/jpeg"),
            //]);
            chatHistory.AddUserMessage("Please get the record which id is 617235768115590");
            var chatResult = await chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                openAIPromptExecutionSettings,
                _kernel);
            //var chatResult = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
            //await chatHistory.AddStreamingMessageAsync(chatResult);
            Console.Write($"\nAssistant : {chatResult}\n");
            //_kernel.InvokeAsync()
            //Console.WriteLine(await _kernel.InvokePromptAsync("Search the record by name.", new(settings)));


            return View();
        }

        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "TEST")]
        public async Task TEST1(object t)
        {
            
            await Task.Delay(3000);
            Assistant.Logger.Warning($"group1消费消息：" + t.ToString());

        }



        [RouteMark("测试2")]
        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 600)]
        public IActionResult GetMarkedRoutes()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(u=>u.Namespace!=null).Where(u => u.Namespace.StartsWith("Magic.Guangdong.Exam.") && u.Name.EndsWith("Controller"));
            foreach (var type in types)
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    foreach (Attribute attribute in methodInfo.GetCustomAttributes(false))
                    {
                        if (attribute is RouteMark routeMark)
                        {
                            Console.WriteLine(type.Name);
                            Console.WriteLine(methodInfo.Name);
                            Console.WriteLine(routeMark.Module);
                        }
                    }
                }
            }
            return Content("123");
        }

        [AllowAnonymous]
        public IActionResult AdminLogin(string msg)
        {
            return Redirect("/system/account/login?msg="+msg);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetTemporaryToken([FromServices] IJwtService _jwtService)
        {
            try
            {
                DateTime expires = DateTime.Now.AddMinutes(10);
                string jwt = _jwtService.Make(Utils.ToBase64Str("49D90000-4C24-00FF-D9D4-08DC47CA938C"), "sa", expires);
                return Content(jwt);
            }
            catch
            {
                throw;
            }      
        }

        public IActionResult TestMaskData(string text)
        {
            var maskData = new MaskDataDto()
            {
                text = text,
                keyId = Utils.GenerateRandomCodePro(16),
                keySecret = Utils.GenerateRandomCodePro(16),
                maskDataType = MaskDataType.Other
            };
            Console.WriteLine($"TestMaskData: {JsonHelper.JsonSerialize(maskData)}");
            return Json(maskData);
        }

        
    }

    public class TimeInformationPlugin
    {
        [KernelFunction, Description("UTC格式的当前时间。")]
        public string GetCurrentUtcTime() => DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
    }

}
