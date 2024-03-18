using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    public class SeedController : Controller
    {
        private IMenuRepo _menuRepo;
        private IResponseHelper _resp;
        public SeedController(IResponseHelper responseHelper, IMenuRepo menuRepo)
        {
            // _menuRepo = menuRepo;
            _resp = responseHelper;
            _menuRepo = menuRepo;
        }
        public IActionResult Index()
        {
            // _menuRepo.getList();

            return View();
        }

        public async Task<IActionResult> InitTopMenuData()
        {
            if (await _menuRepo.getAnyAsync(u => u.IsDeleted == 0 && u.ParentId==0))
            {
                return Json(_resp.success("success", "无需插入"));
            }
            var topMenu = new List<DbServices.Entities.Menu>() {
                new DbServices.Entities.Menu()
                {
                    Name="考试管理",
                    Router="",
                    ParentId = 0,
                    Depth=0,
                    IsLeef = 0,
                    Description ="考试管理有关操作"
                },
                new DbServices.Entities.Menu()
                {
                    Name="报名管理",
                    Router="",
                    ParentId = 0,
                    Depth=0,
                    IsLeef = 0,
                    Description ="报名管理有关操作"
                },
                new DbServices.Entities.Menu()
                {
                    Name="账单管理",
                    Router="",
                    ParentId = 0,
                    Depth=0,
                    IsLeef = 0,
                    Description = "账单管理有关操作"
                },
                new DbServices.Entities.Menu()
                {
                    Name="发票管理",
                    Router="",
                    ParentId = 0,
                    Depth=0,
                    IsLeef = 0,
                    Description ="发票管理有关操作"
                },
                new DbServices.Entities.Menu()
                {
                    Name="系统管理",
                    Router="",
                    ParentId = 0,
                    Depth=0,
                    IsLeef = 0,
                    Description ="系统管理有关操作"
                },
            };
            await _menuRepo.addItemsBulkAsync(topMenu);
            return Json(_resp.success("success", "种子数据插入完成"));
        }

        public async Task<IActionResult> InitSubMenuData()
        {
            if (await _menuRepo.getAnyAsync(u => u.IsDeleted == 0 && u.ParentId!=0))
            {
                return Json(_resp.success("success", "无需插入"));
            }
            var tops = await _menuRepo.getListAsync(u => u.ParentId == 0);
            var subMenus = new List<DbServices.Entities.Menu>();
           
            foreach (var item in tops)
            {
                int cnt = 0;
                while (cnt <3)
                {
                    string name = $"{item.Name}-子菜单{cnt + 1}";
                    subMenus.Add(new DbServices.Entities.Menu()
                    {
                        Name = name,
                        Router = "",
                        ParentId = item.Id,
                        Depth = item.Depth + 1,
                        IsLeef = 0,
                        Description = $"{name}有关操作"
                    });
                    cnt++;
                }
                
            }
            
            await _menuRepo.addItemsBulkAsync(subMenus);
            return Json(_resp.success("success", "种子数据插入完成"));
        }
    }
}
