using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Report.Activities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IActivityRepo _activityRepo;
        private readonly IProvinceRepo _provinceRepo;
        private readonly ICityRepo _cityRepo;
        private readonly IDistrictRepo _districtRepo;
        private readonly IUnitInfoRepo _unitInfoRepo;
        private readonly IResponseHelper _resp;

        public ActivityController(IActivityRepo activityRepo, IResponseHelper resp, IProvinceRepo provinceRepo, ICityRepo cityRepo, IDistrictRepo districtRepo, IUnitInfoRepo unitInfoRepo)
        {
            _activityRepo = activityRepo;
            _resp = resp;
            _provinceRepo = provinceRepo;
            _cityRepo = cityRepo;
            _districtRepo = districtRepo;
            _unitInfoRepo = unitInfoRepo;
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

        /// <summary>
        /// 获取单个活动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 100,VaryByQueryKeys =new string[] {"id","rd"})]
        public async Task<IActionResult> GetActivity(long id)
        {
            if(await _activityRepo.getAnyAsync(u => u.Id == id))
            {
                var ret = await _activityRepo.getOneAsync(u => u.Id == id);
                return Json(_resp.success(ret.Adapt<ActivityDto>()));
            }
            return Json(_resp.error("活动不存在"));
        }

        [ResponseCache(Duration = 600, VaryByQueryKeys = new string[] { "cityId", "provinceId" })]
        public async Task<IActionResult> GetDistrictDrops(int cityId, int provinceId = 0)
        {
            return Json(_resp.success(await _districtRepo.GetDistrictDropsAsync(cityId, provinceId)));
        }

        [ResponseCache(Duration = 600, VaryByQueryKeys = new string[] { "provinceId" })]
        public async Task<IActionResult> GetCityDrops(int provinceId)
        {

            return Json(_resp.success(
                await _cityRepo.GetCitiyDropsAsync(provinceId)
                ));
        }

        [ResponseCache(Duration = 600, VaryByQueryKeys = new string[] { "rd" })]
        public async Task<IActionResult> GetProvinceDrops()
        {
            var items = await _provinceRepo.getListAsync(u => u.IsDeleted == 0);
            return Json(_resp.success(
                items.Select(u => new { value = u.Id, name = u.ProvinceName, text = u.ProvinceShortName })));
        }
    }
}
