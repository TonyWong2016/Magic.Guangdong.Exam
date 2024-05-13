using EasyCaching.Core;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Teacher.Areas.System.Controllers
{
    [Area("System")]
    public class PaperController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ITeacherExamAssignViewRepo _teacherExamAssignViewRepo;
        private readonly IRedisCachingProvider _redisCachingProvider;
        public PaperController(IResponseHelper resp, ITeacherExamAssignViewRepo teacherExamAssignViewRepo,IRedisCachingProvider redisCachingProvider)
        {
            _resp = resp;
            _teacherExamAssignViewRepo = teacherExamAssignViewRepo;
            _redisCachingProvider = redisCachingProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        //)
    }
}
