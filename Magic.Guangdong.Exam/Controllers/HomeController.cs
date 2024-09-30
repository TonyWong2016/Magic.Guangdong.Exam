using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Controllers
{
    [RouteMark("测试")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [RouteMark("测试1")]
        public IActionResult Index()
        {           
            return View();
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
}
