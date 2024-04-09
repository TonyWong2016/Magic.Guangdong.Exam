using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    public class ProvinceController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IProvinceRepo _provinceRepo;
        public ProvinceController(IResponseHelper resp, IProvinceRepo provinceRepo)
        {
            _resp = resp;
            _provinceRepo = provinceRepo;
        }

        [RouteMark("省份管理")]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 100,VaryByQueryKeys = new string[] { "whereJsonStr","pageIndex","pageSize","orderby","isAsc","rd" })]
        public  IActionResult GetProvinces(PageDto dto)
        {
            long total;
            return Json(_resp.success(new {items= _provinceRepo.getList(dto, out total) ,total}));
        }

        //[AllowAnonymous]
        [ResponseCache(Duration = 600,VaryByQueryKeys = new string[] { "rd" })]
        public async Task<IActionResult> GetProvinceDrops()
        {
            var items = await _provinceRepo.getListAsync(u => u.IsDeleted==0);
            return Json(_resp.success(
                items.Select(u => new { value = u.Id, name = u.ProvinceName,text=u.ProvinceShortName })));
        }

        [RouteMark("添加省份")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Province province)
        {
            if(await _provinceRepo.getAnyAsync(u=>u.ProvinceName == province.ProvinceName ||
            u.ProvinceShortName==province.ProvinceShortName||
            u.ProvinceCode==province.ProvinceCode||
            u.Id==province.Id))
            {
                return Json(_resp.error("省份已存在"));
            }
            return Json(_resp.success(await _provinceRepo.addItemAsync(province)));
        }

        [RouteMark("编辑省份")]
        public async Task<IActionResult> Edit(int Id)
        {
            return View(await _provinceRepo.getOneAsync(u => u.Id == Id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Province province)
        {
            if (await _provinceRepo.getAnyAsync(u => u.Id != province.Id &&
                (u.ProvinceName == province.ProvinceName ||
                u.ProvinceShortName == province.ProvinceShortName ||
                u.ProvinceCode == province.ProvinceCode)
                ))
            {
                return Json(_resp.error("省份已存在"));
            }
            province.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _provinceRepo.updateItemAsync(province)));
        }

        [RouteMark("删除省份")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove([FromServices]ICityRepo cityRepo, int Id)
        {
            if(!await _provinceRepo.getAnyAsync(u=>u.Id==Id))
            {
                return Json(_resp.error("省份不存在"));
            }
            if (await cityRepo.getAnyAsync(u => u.ProvinceId == Id))
            {
                return Json(_resp.error("省份下存在城市，不能删除"));
            }
            var province = await _provinceRepo.getOneAsync(u => u.Id == Id);
            province.IsDeleted = 1;
            province.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _provinceRepo.updateItemAsync(province)));
        }
    }
}
