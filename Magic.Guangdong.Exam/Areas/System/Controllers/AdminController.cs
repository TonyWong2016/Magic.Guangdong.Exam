using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Dtos.Admins;
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
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "whereJsonStr", "pageindex", "pagesize", "orderby", "isAsc", "rd" })]
        public IActionResult GetAdminList(PageDto dto, long[] roleIds=null)
        {
            long total;
            return Json(_resp.success(
                new { items = _adminRepo.GetAdminList(dto,roleIds, out total), total }
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
        [HttpPost,ValidateAntiForgeryToken]        
        public async Task<IActionResult> Create(AdminDto dto)
        {
            if (await _adminRepo.getAnyAsync(u => u.Name == dto.Name || u.Email == dto.Email || u.Mobile == dto.Mobile))
            {
                return Json(_resp.error("该用户名/邮箱/手机号已存在"));
            }
            string keySecret = Utils.GenerateRandomCodePro(16);
            string keyId = Utils.GenerateRandomCodePro(16);
            string password = Security.Encrypt(dto.Password, Encoding.UTF8.GetBytes(keyId), Encoding.UTF8.GetBytes(keySecret));
            
            //admin=dto.Adapt<Admin>();
            var admin = new Admin();
            admin.Id = dto.Id;
            admin.Name = string.IsNullOrEmpty(dto.Name) ? Utils.GenerateRandomCodePro(6) : dto.Name;
            admin.Email = dto.Email;
            admin.Mobile = dto.Mobile;
            admin.Password = password;
            admin.KeyId = keyId;
            admin.Description = dto.Description;
            admin.KeySecret = keySecret;
            admin.NickName = dto.NickName;
            return Json(_resp.success(await _adminRepo.addItemAsync(admin), "添加成功"));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var admin = await _adminRepo.getOneAsync(u => u.Id == id);
            admin.Password = "";//密码默认不返回
            return View(admin.Adapt<AdminDto>());
        }

        /// <summary>
        /// 修改管理员信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(AdminDto dto)
        {
            var admin = await _adminRepo.getOneAsync(u => u.Id == dto.Id);
            if (dto.Name != admin.Name)
            {
                //实际不会进入到这里，因为页面不会放开该字段的修改，加上只是避免一些极端情况。
                return Json(_resp.error("用户名创建后不可以修改"));
            }
            //name作为固定值，创建后就不可以修改了！
            //admin.Name = dto.Name;
            admin.Email = dto.Email;    
            admin.Mobile = dto.Mobile;
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                admin.Password= Security.Encrypt(dto.Password, Encoding.UTF8.GetBytes(admin.KeyId), Encoding.UTF8.GetBytes(admin.KeySecret));
            }
            admin.Status = dto.Status;
            admin.Description= dto.Description;
            admin.UpdatedAt = DateTime.Now;
            admin.NickName = dto.NickName;
            return Json(_resp.success(await _adminRepo.updateItemAsync(admin), "修改成功"));
        }

        /// <summary>
        /// 移除账号
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Remove(Guid adminId)
        {
            var admin = await _adminRepo.getOneAsync(u => u.Id == adminId);
            admin.IsDeleted = 1;
            admin.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _adminRepo.updateItemAsync(admin), "删除成功"));
        }
    }
}
