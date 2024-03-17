using Autofac;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    
    public class MenuController : Controller
    {
        //[Route("/Menu/")]
        private IResponseHelper _resp;
        private IMenuRepo _menuRepo;
        public MenuController(IResponseHelper responseHelper,IMenuRepo menuRepo)
        {
            _menuRepo = menuRepo;
            _resp = responseHelper;
        }

        public IActionResult Index()
        {

            return Content("123");
        }
    }
}
