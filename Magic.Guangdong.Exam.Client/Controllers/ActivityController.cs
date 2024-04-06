using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IActivityRepo _activityRepo;

        private readonly IResponseHelper _resp;

        public ActivityController(IActivityRepo activityRepo, IResponseHelper resp)
        {
            _activityRepo = activityRepo;
            _resp = resp;
        }
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 100,VaryByQueryKeys = new string[] { "whereJsonStr","pageIndex","pageSize","orderby","isAsc" })]
        public IActionResult GetActivities(PageDto dto)
        {
            long total = 0;
            return Json(_resp.success(new { items = _activityRepo.getList(dto, out total), total }));
        }
    }
}
