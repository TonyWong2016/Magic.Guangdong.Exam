using Autofac;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Dto.Menus;
using Magic.Guangdong.DbServices.Dto.Permissions;
using Magic.Guangdong.DbServices.Dto.Routers;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]

    public class MenuController : Controller
    {
        //[Route("/Menu/")]
        private IResponseHelper _resp;
        private IMenuRepo _menuRepo;
       

        /// <summary>
        /// 栏目菜单相关
        /// </summary>
        /// <param name="responseHelper"></param>
        /// <param name="menuRepo"></param>
        /// <param name="permissionRepo"></param>
        public MenuController(IResponseHelper responseHelper, IMenuRepo menuRepo)
        {
            _menuRepo = menuRepo;
            _resp = responseHelper;
        }
        [RouteMark("栏目管理")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取菜单列表，如果menuId为null，则返回所有菜单，否则返回父菜单下的子菜单列表。
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "parentId" })]
        public async Task<IActionResult> GetMenus(long? parentId)
        {
            if (parentId == null)
            {
                return Json(_resp.success((await _menuRepo.getListAsync(u => u.IsDeleted == 0 && u.Status == 1)).Adapt<List<MenuDto>>()));
            }
            return Json(_resp.success((await _menuRepo.getListAsync(u => u.IsDeleted==0 && u.Status==1 && u.ParentId == (long)parentId)).Adapt<List<MenuDto>>()));
        }

        [HttpGet]
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "whereJsonStr", "pageindex", "pagesize", "rd" })]
        public IActionResult GetMenuPages(PageDto dto)
        {
            long total = 0;

            return Json(_resp.success(new { items = _menuRepo.getList(dto, out total).Adapt<List<MenuDto>>(), total }));
        }

        [RouteMark("创建栏目")]
        public IActionResult Create()
        {
            ViewData["title"] = "创建栏目";
            return View();
        }

        /// <summary>
        /// 创建栏目。
        /// </summary>
        /// <param name="menuDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[RouteMark("创建栏目")]
        public async Task<IActionResult> Create(MenuDto menuDto)
        {
            var menu = menuDto.Adapt<Menu>();
            //先临时指定一个
            menu.CreatorId = Guid.Parse("49D90000-4C24-00FF-D9D4-08DC47CA938C");
            if(await _menuRepo.getAnyAsync(u=>u.Name==menu.Name && u.IsDeleted==0))
            {
                return Json(_resp.error("栏目名称已存在"));
            }
            return Json(_resp.success(await _menuRepo.addItemAsync(menu)));
        }

        /// <summary>
        /// 编辑栏目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RouteMark("编辑栏目")]
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["title"] = "编辑栏目";
            if (await _menuRepo.getAnyAsync(u => u.Id == id))
            {
                return View((await _menuRepo.getOneAsync(u => u.Id == id)).Adapt<MenuDto>());
            }
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// 编辑栏目
        /// </summary>
        /// <param name="menuDto"></param>
        /// <returns></returns>
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MenuDto menuDto)
        {
            if(await _menuRepo
                .getAnyAsync(u=>u.Name==menuDto.Name && u.IsDeleted==0 && u.Id!=menuDto.Id)
                )
            {
                return Json(_resp.error("栏目名称已存在"));
            }
            var menu = menuDto.Adapt<Menu>();
            menu.UpdatedAt = DateTime.Now;

            return Json(_resp.success(await _menuRepo.updateItemAsync(menu)));
        }

        /// <summary>
        /// 移除栏目（软删除）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(long id)
        {
            var item = await _menuRepo.getOneAsync(u => u.Id == id);
            item.UpdatedAt=DateTime.Now;
            item.IsDeleted = 1;
            return Json(_resp.success(await _menuRepo.updateItemAsync(item)));
        }

    }
}
