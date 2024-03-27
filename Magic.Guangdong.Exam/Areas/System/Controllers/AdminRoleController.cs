using DotNetCore.CAP;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("system")]
    public class AdminRoleController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IAdminRoleRepo _adminRoleRepo;
        private readonly ICapPublisher _capPublisher;
        

        public AdminRoleController(IResponseHelper responseHelper, IAdminRoleRepo adminRoleRepo, ICapPublisher capPublisher)
        {
            _resp = responseHelper;
            _adminRoleRepo = adminRoleRepo;
            _capPublisher = capPublisher;
        }
        public IActionResult Index()
        {
            return View();
        }

        [RouteMark("角色授权")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Grant(Guid adminId, long[] roleIds)
        {
            if(await _adminRoleRepo.Grant(adminId,roleIds))
            {
                return Json(_resp.success(true,"操作成功"));
            }
            return Json(_resp.error("操作失败"));
        }


    }
}
