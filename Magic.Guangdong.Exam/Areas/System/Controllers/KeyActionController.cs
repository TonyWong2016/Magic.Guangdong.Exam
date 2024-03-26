using DotNetCore.CAP;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    public class KeyActionController : Controller
    {
        private readonly IKeyActionRepo _keyActionRepo;
        public KeyActionController(IKeyActionRepo keyActionRepo)
        {
            _keyActionRepo = keyActionRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "AddKeyAction")]
        public async Task AddKeyAction(KeyAction keyAction)
        {
            await _keyActionRepo.addItemAsync(keyAction);
        }
    }
}
