using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    
    public class MenuController : Controller
    {
        //[Route("/Menu/")]

        [AllowAnonymous]
        public IActionResult Index()
        {

            return Content("123");
        }
    }
}
