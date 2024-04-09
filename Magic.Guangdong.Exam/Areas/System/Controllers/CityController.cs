using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("system")]
    public class CityController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IProvinceRepo _provinceRepo;
        private readonly ICityRepo _cityRepo;
        public CityController(IResponseHelper resp, IProvinceRepo provinceRepo, ICityRepo cityRepo)
        {
            _resp = resp;
            _provinceRepo = provinceRepo;
            _cityRepo = cityRepo;
        }

        [RouteMark("城市管理")]
        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        public IActionResult GetCities(PageDto dto)
        {
            long total;
            return Json(_resp.success(new {items = _cityRepo.GetCities(dto, out total) ,total}));
        }


        [ResponseCache(Duration =600,VaryByQueryKeys =new string[] { "provinceId" })]
        public async Task<IActionResult> GetCityDrops(int provinceId)
        {
            //Expression<Func<City, bool>> where = null;
            //where = where.And(u => u.IsDeleted == 0);

            //if (provinceId > 0)
            //{
            //    where = where.And(u => u.ProvinceId == provinceId);
            //}
            //var items = await _cityRepo.getListAsync(where);
            //return Json(_resp.success(
            //    items.Select(u => new { value = u.Id, name = u.CityName, u.ProvinceId })
            //    ));
            return Json(_resp.success(
                await _cityRepo.GetCitiyDropsAsync(provinceId)
                ));
        }

        [RouteMark("新增城市")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(City city)
        {
            
            if (city.ProvinceId == 0)
            {
                return Json(_resp.error("请选择省份"));
            }
            if(await _cityRepo.getAnyAsync(u=>u.CityName==city.CityName && u.ProvinceId==city.ProvinceId))
            {
                return Json(_resp.error("城市已存在"));
            }
            return Json(_resp.success(await _cityRepo.addItemAsync(city)));
        }

        [RouteMark("编辑城市")]
        public async Task<IActionResult> Edit(int id)
        {
            return View(await _cityRepo.getOneAsync(u=>u.Id==id));
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(City city)
        {
            
            if(await _cityRepo.getAnyAsync(u=>u.Id!=city.Id && u.CityName==city.CityName && u.ProvinceId==city.ProvinceId))
            {
                return Json(_resp.error("城市已存在"));
            }
            city.UpdatedAt=DateTime.Now;
            return Json(_resp.success(await _cityRepo.updateItemAsync(city)));
        }

        [RouteMark("删除城市")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove([FromServices] IDistrictRepo districtRepo, int Id)
        {
            if (!await _cityRepo.getAnyAsync(u => u.Id == Id))
            {
                return Json(_resp.error("城市不存在"));
            }
            if (await districtRepo.getAnyAsync(u => u.CityId == Id))
            {
                return Json(_resp.error("城市下存在区/县，不能删除"));
            }
            var city = await _cityRepo.getOneAsync(u => u.Id == Id);
            city.IsDeleted = 1;
            city.UpdatedAt= DateTime.Now;
            return Json(_resp.success(await _cityRepo.updateItemAsync(city)));
        }
    }
}
