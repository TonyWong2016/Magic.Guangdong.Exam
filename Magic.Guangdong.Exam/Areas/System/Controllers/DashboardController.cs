using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    public class DashboardController : Controller
    {
        [RouteMark("数据看板")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
