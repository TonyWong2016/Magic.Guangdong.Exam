using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.System.Admins;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    public class AdminController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IAdminRepo _adminRepo;
        private readonly IJwtService _jwtService;
        private readonly IAdminRoleRepo _adminRoleRepo;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICapPublisher _capPublisher;
        private readonly IAdminLoginLogRepo _adminLoginLogRepo;
        /// <summary>
        /// 管理员相关
        /// </summary>
        /// <param name="responseHelper"></param>
        /// <param name="adminRepo"></param>
        /// <param name="jwtService"></param>
        /// <param name="adminRoleRepo"></param>
        /// <param name="redisCachingProvider"></param>
        /// <param name="webHostEnvironment"></param>
        /// <param name="capPublisher"></param>
        public AdminController(IResponseHelper responseHelper,IAdminRepo adminRepo,IJwtService jwtService,IAdminRoleRepo adminRoleRepo,IRedisCachingProvider redisCachingProvider, IWebHostEnvironment webHostEnvironment,ICapPublisher capPublisher, IAdminLoginLogRepo adminLoginLogRepo) 
        {
            _resp = responseHelper;
            _adminRepo = adminRepo;
            _jwtService = jwtService;
            _adminRoleRepo = adminRoleRepo;
            _redisCachingProvider = redisCachingProvider;
            _webHostEnvironment = webHostEnvironment;
            _capPublisher = capPublisher;
            _adminLoginLogRepo = adminLoginLogRepo;
        }
        
        
        [RouteMark("账号管理")]
        public IActionResult Index()
        {
            return View();
        }


        [RouteMark("获取管理员账号列表")]
        [HttpPost]
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "whereJsonStr", "pageindex", "pagesize", "orderby", "isAsc", "rd" })]
        public IActionResult GetAdminList(AdminListPageDto dto)
        {
            long total;
            return Json(_resp.success(
                new { items = _adminRepo.GetAdminList(dto,  out total), total }
                ));
        }

        [RouteMark("创建账号")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 创建账号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminDto dto)
        {
            if (await _adminRepo.CreateAdmin(dto))
                return Json(_resp.success(true, "添加成功"));
            return Json(_resp.error("添加失败，请尝试更换用户名，邮箱和电话"));
        }

        [RouteMark("修改用户信息")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var admin = await _adminRepo.getOneAsync(u => u.Id == id);
            admin.Password = "";//密码默认不返回
            var ars = await _adminRoleRepo.getListAsync(u=>u.AdminId==id);
            var ret = admin.Adapt<AdminDto>();
            ret.RoleIds = ars.Select(u => u.RoleId).ToArray();
            return View(ret);
        }


        /// <summary>
        /// 修改管理员信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminDto dto)
        {
            if (await _adminRepo.UpdateAdmin(dto))
                return Json(_resp.success(true, "修改成功"));
            return Json(_resp.error("修改失败"));

        }


        [RouteMark("重置密码")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(Guid adminId,string newPwd)
        {
            if (string.IsNullOrWhiteSpace(newPwd))
                newPwd = Utils.GenerateRandomCodePro(8, 2);
            var admin = await _adminRepo.getOneAsync(u => u.Id == adminId);
            string keyId = Utils.GenerateRandomCodePro(16);
            string keySecret = Utils.GenerateRandomCodePro(16);
           
            admin.KeyId = keyId;
            admin.KeySecret = keySecret;
            admin.Password = Security.Encrypt(newPwd, Encoding.UTF8.GetBytes(keyId), Encoding.UTF8.GetBytes(keySecret));
            return Json(_resp.success(await _adminRepo.updateItemAsync(admin), $"重置成功，请牢记此密码，该密码【{newPwd}】只显示这一次"));
        }

        /// <summary>
        /// 移除账号
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        [RouteMark("删除账号")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid id)
        {
            var admin = await _adminRepo.getOneAsync(u => u.Id == id);
            admin.IsDeleted = 1;
            admin.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _adminRepo.updateItemAsync(admin), "删除成功"));
        }
    }
}
