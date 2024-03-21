using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dto.Permissions;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    public class PermissionController : Controller
    {
        private IPermissionRepo _permissionRepo;
        private IResponseHelper _resp;
        public PermissionController(IResponseHelper responseHelper,IPermissionRepo permissionRepo)
        {
            _resp = responseHelper;
            _permissionRepo = permissionRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

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
