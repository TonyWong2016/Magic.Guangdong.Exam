using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Teacher.Areas.System.Controllers
{
    [Area("System")]
    public class PaperController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
