using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Report.Activities;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Report.Controllers
{
    [Area("Report")]
    public class ActivityController : Controller
    {
        private readonly IActivityRepo _activityRepo;
        
        private readonly IResponseHelper _resp;

        public ActivityController(IActivityRepo activityRepo, IResponseHelper resp)
        {
            _activityRepo = activityRepo;
            _resp = resp;
        }

        [RouteMark("活动报名")]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "rd" })]
        public async Task<IActionResult> GetActivityDrops()
        {
            return Json(_resp.success((
                await _activityRepo.getListAsync(u => u.IsDeleted == 0)
                ).Select(u => new { value = u.Id, text = u.Title })
             ));
        }

        [ResponseCache(Duration = 10)]
        public IActionResult GetActivities(PageDto dto)
        {
            long total = 0;
            return Json(_resp.success(new { items = _activityRepo.getList(dto, out total), total }));
        }

        [RouteMark("创建活动")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActivityDto dto)
        {
            return Json(_resp.success(await _activityRepo.addItemAsync(dto.Adapt<Activity>())));
        }

        /// <summary>
        /// 编辑活动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RouteMark("编辑活动")]
        public async Task<IActionResult> Edit(long id)
        {
            if (!await _activityRepo.getAnyAsync(u => u.Id == id))
                return Redirect("/error?msg=活动不存在");
            var activity = await _activityRepo.getOneAsync(u => u.Id == id);
            return View(activity.Adapt<ActivityDto>());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ActivityDto dto)
        {
            //var activity = await _activityRepo.getOneAsync(u => u.Id == dto.Id);
            var activity = dto.Adapt<Activity>();
            activity.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _activityRepo.updateItemAsync(activity)));
        }

        [RouteMark("移除活动")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove([FromServices]IExaminationRepo examRepo, [FromServices]IActivityReportRepo activityReportRepo, long id)
        {
            if (await examRepo.getAnyAsync(u => u.AssociationId == id.ToString()) || await activityReportRepo.getAnyAsync(u=>u.ActivityId==id))
            {
                return Json(_resp.error("该活动已被占用（关联了考试，或者已有报名该活动的用户），无法删除"));
            }
            var activity = await _activityRepo.getOneAsync(u => u.Id == id);
            activity.IsDeleted = 1;
            activity.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _activityRepo.updateItemAsync(activity)));
        }

    }
}
