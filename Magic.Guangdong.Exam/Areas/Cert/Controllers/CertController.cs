using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Cert.Controllers
{
    public class CertController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
