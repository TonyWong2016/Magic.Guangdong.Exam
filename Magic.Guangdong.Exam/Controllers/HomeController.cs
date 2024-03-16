using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

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

        public IActionResult Index()
        {           
            return View();
        }

        [RouteMark("测试1")]
        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 600)]
        public IActionResult GetMarkedRoutes()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(u => u.Namespace == "Magic.Guangdong.Exam.Controllers" && u.Name.EndsWith("Controller"));
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
                        }
                    }
                }
            }
            return View();
        }
        
    }
}
