using Autofac;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dto.Menus;
using Magic.Guangdong.DbServices.Dto.Permissions;
using Magic.Guangdong.DbServices.Dto.Routers;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]

    public class MenuController : Controller
    {
        //[Route("/Menu/")]
        private IResponseHelper _resp;
        private IMenuRepo _menuRepo;
        private IPermissionRepo _permissionRepo;

        /// <summary>
        /// 栏目菜单相关
        /// </summary>
        /// <param name="responseHelper"></param>
        /// <param name="menuRepo"></param>
        /// <param name="permissionRepo"></param>
        public MenuController(IResponseHelper responseHelper, IMenuRepo menuRepo, IPermissionRepo permissionRepo)
        {
            _menuRepo = menuRepo;
            _resp = responseHelper;
            _permissionRepo = permissionRepo;
        }

        public IActionResult Index()
        {

            return Content("123");
        }

        /// <summary>
        /// 获取菜单列表，如果menuId为null，则返回所有菜单，否则返回父菜单下的子菜单列表。
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "menuId" })]
        public async Task<IActionResult> GetMenus(long? menuId)
        {
            if (menuId == null)
            {
                return Json(_resp.success((await _menuRepo.getListAsync(u => u.IsDeleted == 0)).Adapt<List<MenuDto>>()));
            }
            return Json(_resp.success((await _menuRepo.getListAsync(u => u.ParentId == (long)menuId)).Adapt<List<MenuDto>>()));
        }

        [RouteMark("创建栏目")]
        public IActionResult Create()
        {
            ViewData["title"] = "创建栏目";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuDto menu)
        {
            return null;
        }

        [ResponseCache(Duration = 600, VaryByQueryKeys = new string[] { "rd" })]
        public async Task<IActionResult> GetPermissions()
        {
            return Json(_resp.success(
                (await _permissionRepo
                .getListAsync(u => u.IsDeleted == 0))
                .Adapt<List<PermissionDto>>()));
        }


    }
}
