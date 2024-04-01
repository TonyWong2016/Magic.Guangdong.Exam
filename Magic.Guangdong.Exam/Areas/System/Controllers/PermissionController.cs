using EasyCaching.Core;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.System.Permissions;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    public class PermissionController : Controller
    {
        private IPermissionRepo _permissionRepo;
        private IResponseHelper _resp;
        private IRedisCachingProvider _redisCachingProvider;
        public PermissionController(IResponseHelper responseHelper,IPermissionRepo permissionRepo, IRedisCachingProvider redisCachingProvider)
        {
            _resp = responseHelper;
            _permissionRepo = permissionRepo;
            _redisCachingProvider = redisCachingProvider;
        }

        public IActionResult Index()
        {
            return View();
        }
        [RouteMark("获取所有权限")]
        [ResponseCache(Duration = 600, VaryByQueryKeys = new string[] { "rd" })]
        public async Task<IActionResult> GetPermissions()
        {
            return Json(_resp.success(
                (await _permissionRepo
                .getListAsync(u => u.IsDeleted == 0 && u.Status == 0))
                .Adapt<List<PermissionDto>>()));
        }

        
    }


}
