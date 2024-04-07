using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    public class UnitInfoController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IUnitInfoRepo _unitInfoRepo;
        private readonly IReportInfoRepo _reportInfoRepo;
        public UnitInfoController(IResponseHelper resp, IUnitInfoRepo unitInfoRepo, IReportInfoRepo reportInfoRepo)
        {
            _resp = resp;
            _unitInfoRepo = unitInfoRepo;
            _reportInfoRepo = reportInfoRepo;
        }

        [RouteMark("单位库管理")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetUnitInfos(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _unitInfoRepo.GetUnitInfos(dto, out total), total }));

        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "keyword","districtId", "cityId", "provinceId" })]
        public async Task<IActionResult> GetUnitInfoDrops(string keyword,int districtId=0, int cityId = 0, int provinceId = 0)
        {
            return Json(_resp.success(await _unitInfoRepo.GetUnitDropsAsync(keyword, districtId, cityId, provinceId)));
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "id" })]
        public async Task<IActionResult> GetUnitInfoView(int Id)
        {
            return Json(_resp.success(await _unitInfoRepo.GetUnitInfoView(Id)));
        }

        [RouteMark("新增单位")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnitInfo unitInfo)
        {
            if(await _unitInfoRepo.getAnyAsync(u=>u.OrganizationCode==unitInfo.OrganizationCode || 
            (u.UnitCaption==unitInfo.UnitCaption &&
            u.ProvinceId==unitInfo.ProvinceId &&
            u.CityId==unitInfo.CityId &&
            u.DistrictId == unitInfo.DistrictId)))
            {
                return Json(_resp.error("单位已存在"));
            }

            return Json(_resp.success(await _unitInfoRepo.addItemAsync(unitInfo)));
        }

        [RouteMark("编辑单位")]
        public async Task<IActionResult> Edit(long id)
        {
            return View(await _unitInfoRepo.getOneAsync(u=>u.Id==id));
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UnitInfoView unitInfo)
        {
            if (await _unitInfoRepo.getAnyAsync(u => u.Id != unitInfo.Id &&
            u.OrganizationCode == unitInfo.OrganizationCode ||
           (u.UnitCaption == unitInfo.UnitCaption &&
           u.ProvinceId == unitInfo.ProvinceId &&
           u.CityId == unitInfo.CityId &&
           u.DistrictId == unitInfo.DistrictId)
           ))
            {
                return Json(_resp.error("单位已存在"));
            }
            var model = unitInfo.Adapt<UnitInfo>();
            model.UpdatedAt = DateTime.Now;

            return Json(_unitInfoRepo.updateItemAsync(model));
        }

        [RouteMark("删除单位")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(long id)
        {
            if(await _reportInfoRepo.getAnyAsync(u => u.UnitId == id))
            {
                return Json(_resp.error("单位已被使用，不能删除"));
            }

            if(!await _unitInfoRepo.getAnyAsync(u => u.Id == id))
            {
                return Json(_resp.error("单位不存在"));
            }

            var model = await _unitInfoRepo.getOneAsync(u => u.Id == id);
            model.UpdatedAt = DateTime.Now;
            model.IsDeleted = 1;
            return Json(_resp.success(await _unitInfoRepo.updateItemAsync(model)));
        }
    }
}
