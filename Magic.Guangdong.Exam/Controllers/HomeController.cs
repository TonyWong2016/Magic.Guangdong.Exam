using DotNetCore.CAP;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Magic.Guangdong.Exam.LLM.Plugins.Simple;
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
        private readonly IBaiduOcrHelper _baiduOcrHelper;
        private readonly IAdminRepo _adminRepo;
        public HomeController(ILogger<HomeController> logger, IBaiduOcrHelper baiduOcrHelper,ICapPublisher capPublisher,Kernel kernel, IWebHostEnvironment en, IServiceProvider serviceProvider,IAdminRepo adminRepo)
        {
            _logger = logger;
            _baiduOcrHelper = baiduOcrHelper;
            _capPublisher = capPublisher;            
            _kernel = kernel.Clone();
            _serviceProvider = serviceProvider;
            _en = en;
            _adminRepo = adminRepo;
        }

        public async Task<IActionResult> Index()
        {
            //这一步后续从数据看读取
            var obj = new ApplyModel()
            {
                ProjectType = "航天科技创新赛",
                Area = "浙江 杭州市",
                TeamNumber = "BA24030035",
                ApplyMembers = [new ApplyMember {
                    Group="初中三年级",
                    Name="宋宇宁",
                    IdCard="330103200811111044",
                    School = "杭州外国语学校",
                    Role = "参赛选手"
                },new ApplyMember {
                    Group="初中三年级",
                    Name="曾昱希",
                    IdCard="330106200810124029",
                    School = "杭州外国语学校",
                    Role = "参赛选手"
                },new ApplyMember {
                    Group="初中三年级",
                    Name="范欣航",
                    IdCard="330102200901201223",
                    School = "杭州外国语学校",
                    Role = "参赛选手"
                },new ApplyMember {
                    Group="",
                    Name="刘勇",
                    IdCard="421004198111133035",
                    School = "杭州外国语学校",
                    Role = "指导老师"
                }],
                Schools = "杭州外国语学校",
                MemberSign = "宋宇宁，曾昱希，范欣航",
                FileName = ""
            };
            string pdfPath = Path.Combine($"{_en.WebRootPath}", "upfile", "test", "申报表2.pdf");
            var result = await _baiduOcrHelper.DocumentRecognition(pdfPath);

            // 获取聊天完成服务
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            // 启用自动函数调用
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,

            };

            PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
            ChatHistory chatHistory = [];
            chatHistory.AddSystemMessage($"你是一个材料审核人员，我传递给你两个比对文件，第一个是我从数据库里取出来的，结构化的申报信息;" +
                $"第二个是我通过文档识别提取到的用户提交的申报材料的内容，是根据材料表格逐行输出，并把识别到的文字放到了words属性，并且给与了置信度参数probability；" +
                $"请你自己解析第二个参数的内容，并将其和第一个参数做比对，判定该材料是否为第一个参数里的赛队为同一支，参赛人员是否相符");
            chatHistory.AddUserMessage($"第一个参数：{JsonHelper.JsonSerialize(obj)} ; 第二个参数：{JsonHelper.JsonSerialize(result)}");
            var chatResult = await chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                openAIPromptExecutionSettings,
                _kernel);
            
            Console.Write($"\nAssistant : {chatResult}\n");

            return Json(chatResult);
        }

        [RouteMark("测试1")]
        public async Task<IActionResult> TestSK()
        {
            //var test = await _recordRepo.getAnyAsync(u => u.IsDeleted == 0);
            //int i = 0;
            //while ( i < 3){
            //    Assistant.Logger.Warning($"生产消息：" + DateTime.Now + i.ToString());
            //    _capPublisher.Publish(CapConsts.PREFIX + "TEST", $"第{(i+1).ToString()}条，{DateTime.Now}");
            //    i++;
            //}

            //_kernel.Plugins.AddFromType<TimeInformationPlugin>();
            _kernel.Plugins.AddFromType<RecordSearch>("RecordSearch", _serviceProvider);
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
        public async Task<IActionResult> GetTemporaryToken([FromServices] IJwtService _jwtService,string uname)
        {
            try
            {
                if (string.IsNullOrEmpty(uname))
                    uname = "sa";
                var admin = await _adminRepo.getOneAsync(u => u.Name.Equals(uname));
                if (admin == null)
                    return Content("error");
                DateTime expires = DateTime.Now.AddMinutes(10);
                string jwt = _jwtService.Make(Utils.ToBase64Str(admin.Id.ToString()), uname, expires);
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

    public class ApplyModel
    {
        public string ProjectType { get; set; }

        public string Area { get; set; }

        public string TeamNumber { get; set; }

        public ApplyMember[] ApplyMembers { get; set; }

        public string FileName { get; set; }

        public string MemberSign { get; set; }

        public string Schools { get; set; }

        public string Email { get; set; } = "wtlemon@126.com";
    }

    public class ApplyMember
    {
        public string Name { get; set; }

        public string IdCard { get; set; }

        public string School { get; set; }
        
        public string Group { get; set; }
    
        public string Role { get; set; }
    }
}
