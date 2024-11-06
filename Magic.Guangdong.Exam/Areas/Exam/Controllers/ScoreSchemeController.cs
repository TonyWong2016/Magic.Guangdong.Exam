using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Exam.Controllers
{
    [Area("Exam")]
    public class ScoreSchemeController : Controller
    {
        private IScoreSchemeRepo _scoreSchemeRepo;
        private IResponseHelper _resp;

        public ScoreSchemeController(IScoreSchemeRepo scoreSchemeRepo, IResponseHelper resp)
        {
            _resp = resp;
            _scoreSchemeRepo = scoreSchemeRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetList(PageDto pageDto) 
        {
            long total;
            return Json(_resp.success(new { items = _scoreSchemeRepo.getList(pageDto, out total), total }));
        }

        
    }
}
