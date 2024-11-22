using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class ExposedApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }



    }
}
