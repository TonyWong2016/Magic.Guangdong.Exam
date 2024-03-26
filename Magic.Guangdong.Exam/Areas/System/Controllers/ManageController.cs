using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    public class ManageController : Controller
    {
        
        public ManageController() { }

        [RouteMark("管理界面")]
        public IActionResult Index()
        {
            return View();
        }

         
    }
}
