using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    public class DistrictController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IDistrictRepo _districtRepo;
        public DistrictController(IResponseHelper resp, IDistrictRepo districtRepo)
        {
            _resp = resp;
            _districtRepo = districtRepo;
        }

        [RouteMark("区县管理")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetDistricts(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _districtRepo.GetDictricts(dto, out total), total }));
        }

        [ResponseCache(Duration =100,VaryByQueryKeys = new string[] {"cityId","provinceId"})]
        public async Task<IActionResult> GetDistrictDrops(int cityId, int provinceId=0)
        {
            return Json(_resp.success(await _districtRepo.GetDistrictDropsAsync(cityId, provinceId)));
        }

        [ResponseCache(Duration = 100,VaryByQueryKeys = new string[] {"id"})]
        public async Task<IActionResult> GetDistrictView(int Id)
        {
            return Json(_resp.success(await _districtRepo.GetDistrictView(Id))); 
        }

        [RouteMark("新增区县")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(District district)
        {
            if (await _districtRepo.getAnyAsync(u => u.DistrictName == district.DistrictName && u.CityId == district.CityId))
            {
                return Json(_resp.error("区县已存在"));
            }
            district.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _districtRepo.addItemAsync(district)));
        }

        [RouteMark("编辑区县")]
        public async Task<IActionResult> Edit(int id)
        {
           
            return View(await _districtRepo.getOneAsync(u => u.Id == id));
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(District district)
        {
            if (await _districtRepo.getAnyAsync(u => u.Id!=district.Id && u.DistrictName == district.DistrictName && u.CityId == district.CityId))
            {
                return Json(_resp.error("区县已存在"));
            }
            district.UpdatedAt=DateTime.Now;
            return Json(_resp.success(await _districtRepo.updateItemAsync(district)));
        }

        [RouteMark("删除区县")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove([FromServices]IUnitInfoRepo unitInfoRepo, int id)
        {
            if(!await _districtRepo.getAnyAsync(u => u.Id == id))
            {
                return Json(_resp.error("区县不存在"));
            }
            if (await unitInfoRepo.getAnyAsync(u => u.DistrictId == id))
            {
                return Json(_resp.error("有单位库关联当前区县，不可删除"));
            }
            var district = await _districtRepo.getOneAsync(u => u.Id == id);
            district.IsDeleted = 1;
            district.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _districtRepo.updateItemAsync(district)));
        }
    }
}
