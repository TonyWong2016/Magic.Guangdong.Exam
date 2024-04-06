using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class HomeController : Controller
    {
        //[Route("/Home/Index")]
        public IActionResult Index()
        {
            return Content("hello");
        }

        
    }
}
