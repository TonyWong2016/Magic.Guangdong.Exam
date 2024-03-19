using Autofac;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dto.Menus;
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


        public MenuController(IResponseHelper responseHelper,IMenuRepo menuRepo)
        {
            _menuRepo = menuRepo;
            _resp = responseHelper;
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
        [ResponseCache(Duration = 100,VaryByQueryKeys = new string[] { "menuId" } )]
        [RouteMark("获取菜单")]
        public async Task<IActionResult> GetMenus(long? menuId)
        {
            //var list = await _menuRepo.getListAsync(u => u.IsDeleted == 0);
            //var ret = list.Adapt<List<MenuDto>>();
            if (menuId == null)
            {
                return Json(_resp.success((await _menuRepo.getListAsync(u => u.IsDeleted == 0)).Adapt<List<MenuDto>>()));
            }
            return Json(_resp.success((await _menuRepo.getListAsync(u => u.ParentId == (long)menuId)).Adapt<List<MenuDto>>()));
        }

        [RouteMark("创建菜单")]
        public IActionResult Create()
        {
            ViewData["title"] = "创建栏目";
            return View();
        }

        [ResponseCache(Duration = 600)]
        public IActionResult GetMarkedRoutes()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(u => u.Namespace != null).Where(u => u.Namespace.StartsWith("Magic.Guangdong.Exam.") && u.Name.EndsWith("Controller"));
            foreach (var type in types)
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    foreach (Attribute attribute in methodInfo.GetCustomAttributes(false))
                    {
                        if (attribute is RouteMark routeMark)
                        {
                            Console.WriteLine(type.Name);
                            Console.WriteLine(methodInfo.Name);
                            Console.WriteLine(routeMark.Module);
                        }
                    }
                }
            }
            return Content("123");
        }

    }
}
