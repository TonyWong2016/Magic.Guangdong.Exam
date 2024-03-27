using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Dtos.Roles;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("system")]
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

        [RouteMark("角色管理")]
        public IActionResult Index()
        {
            return View();
        }

        [RouteMark("获取角色列表")]
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "whereJsonStr", "pageindex", "pagesize","orderby", "isAsc", "rd" })]
        public IActionResult GetRoleList(PageDto dto)
        {
            long total = 0;
            return Json(_resp.success(
                 new { items = _roleRepo.getList(dto, out total), total }
                ));
        }

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration =100,VaryByQueryKeys = new string[] { "rd" })]
        public async Task<IActionResult> GetRoleDrops()
        {
            var roles = await _roleRepo.getListAsync(u => u.IsDeleted == 0);
            return Json(_resp.success(
               roles.Select(u => new
               {
                   value = u.Id,
                   name = u.Name
               })
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
        public async Task<IActionResult> Edit(long id)
        {
            if (await _roleRepo.getAnyAsync(u => u.Id == id))
            {
                var role = await _roleRepo.getOneAsync(u => u.Id == id);
                return View(role.Adapt<RoleDto>());
            }
            return Json(_resp.error("角色不存在"));
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "roleId","rd" })]
        public async Task<IActionResult> GetRolePermissions(long roleId)
        {
            return Json(_resp.success((
                await _rolePermissionRepo
                .getListAsync(u => u.RoleId == roleId && u.IsDeleted == 0))
                .Select(u => u.PremissionId))
                );
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleDto dto)
        {
            if (await _roleRepo.getAnyAsync(u => u.Name == dto.Name && u.Id != dto.Id))
            {
                return Json(_resp.error("角色已存在，请更换角色名称"));
            }
            
            return Json(_resp.success(
                await _roleRepo.UpdateRole(dto)
                ));
        }

        [RouteMark("删除角色")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(long id)
        {
           return Json(_resp.success(await _roleRepo.RemoveRole(id)));
        }


    }
}
