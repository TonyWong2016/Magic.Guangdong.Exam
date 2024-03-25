using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dto.Admin;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
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
        public AdminController(IResponseHelper responseHelper,IAdminRepo adminRepo,IJwtService jwtService,IAdminRoleRepo adminRoleRepo,IRedisCachingProvider redisCachingProvider) 
        {

            _resp = responseHelper;
            _adminRepo = adminRepo;
            _jwtService = jwtService;
            _adminRoleRepo = adminRoleRepo;
            _redisCachingProvider = redisCachingProvider;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            if(!await _redisCachingProvider.KeyExistsAsync("CaptchaStr"))
            {
                await _redisCachingProvider.StringSetAsync("CaptchaStr", Utils.GenerateRandomCodePro(1000, 3), DateTime.Now.AddDays(1) - DateTime.Now);
            }

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account">用户名/邮箱/手机</param>
        /// <param name="hashpwd">加密后的密码 md5({password}{captchaTsp})</param>
        /// <param name="captchaTsp">验证码和时间戳的拼接</param>
        /// <param name="remember">是否记住账号密码</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string account, string hashpwd, string captchaTsp, int remember=0)
        {
            if (!await _adminRepo.getAnyAsync(
                u =>
                u.Name.Equals(account) ||
                u.Email.Equals(account) ||
                u.Name.Equals(account) &&
                u.IsDeleted == 0))
            {
                return Json(_resp.error("用户名不存在"));
            }
            var admin = await _adminRepo.getOneAsync(
                u =>
                u.Name.Equals(account) ||
                u.Email.Equals(account) ||
                u.Name.Equals(account) &&
                u.IsDeleted == 0);

            string password = Security.Decrypt(admin.Password, Encoding.UTF8.GetBytes(admin.KeyId), Encoding.UTF8.GetBytes(admin.KeySecret));
            if (Security.GenerateMD5Hash(password + captchaTsp) != hashpwd)
            {
                //这里实际是密码错误，但返回的信息要模糊一下，提防坏人！
                return Json(_resp.error("用户名或密码错误"));
            }
            if (!await _adminRoleRepo.getAnyAsync(u => u.AdminId == admin.Id))
            {
                //没有授权，一般取消这个账户的所有角色就可以达到禁用的效果了。
                return Json(_resp.error("账户异常"));
            }
            var adminRole = await _adminRoleRepo.getOneAsync(u => u.AdminId == admin.Id);
            
            return Json(_resp.success(
                new
                {
                    access_token = _jwtService.Make(admin.Name, adminRole.RoleId.ToString(), remember == 1)
                }));

        }

        [AllowAnonymous]
        [HttpPost,ValidateAntiForgeryToken]
        public IActionResult CheckJwt(string jwt)
        {
            return Json(_resp.success(new { result = _jwtService.Validate(jwt) }));
        }

        [AllowAnonymous]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("examToken");
            Response.Cookies.Delete("userId");
            return Redirect("/system/admin/login?msg=logout");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {

            string keySecret = Utils.GenerateRandomCodePro(16);
            string keyId = Utils.GenerateRandomCodePro(16);
            string password = Security.Encrypt(dto.Password, Encoding.UTF8.GetBytes(keyId), Encoding.UTF8.GetBytes(keySecret));
            var admin = new Admin();
            admin.Name = string.IsNullOrEmpty(dto.Name) ? Utils.GenerateRandomCodePro(6) : dto.Name;
            admin.Email = dto.Email;
            admin.Mobile = dto.Mobile;
            admin.Password = password;
            admin.KeyId = keyId;
            admin.KeySecret = keySecret;
        }
    }

}
