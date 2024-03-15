using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("123");
        }
    }
}
