using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Dto.Role;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using static FreeSql.Internal.GlobalFilter;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    public class RoleController : Controller
    {
        public readonly IResponseHelper _resp;
        public readonly IRoleRepo _roleRepo;
        public readonly IRolePermissionRepo _rolePermissionRepo;

        public RoleController(IResponseHelper responseHelper, IRoleRepo roleRepo, IRolePermissionRepo rolePermissionRepo)
        {
            _resp = responseHelper;
            _roleRepo = roleRepo;
            _rolePermissionRepo = rolePermissionRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        [RouteMark("获取角色列表")]
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "whereJsonStr", "pageindex", "pagesize", "rd" })]
        public IActionResult GetList(PageDto dto)
        {
            long total = 0;
            return Json(_resp.success(
                 new { items = _roleRepo.getList(dto, out total), total }
                ));
        }

        [RouteMark("创建角色")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleDto dto)
        {
            if(await _roleRepo.getAnyAsync(u=>u.Name == dto.Name))
            {
                return Json(_resp.error("角色已存在，请更换角色名称"));
            }
            return Json(_resp.success(
                await _roleRepo.CreateRole(dto)
                ));
        }

        [RouteMark("编辑角色")]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleDto dto)
        {
            if(await _roleRepo.getAnyAsync(u=>u.Name==dto.Name && u.Id != dto.Id))
            {
                return Json(_resp.error("角色已存在，请更换角色名称"));
            }
            
            return Json(_resp.success(
                await _roleRepo.updateItemAsync(dto.Adapt<Role>())
                ));
        }
    }
}
