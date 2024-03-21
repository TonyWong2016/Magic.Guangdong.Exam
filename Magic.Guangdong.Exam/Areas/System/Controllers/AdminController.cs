using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
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
        public AdminController(IResponseHelper responseHelper,IAdminRepo adminRepo,IJwtService jwtService,IAdminRoleRepo adminRoleRepo) 
        {

            _resp = responseHelper;
            _adminRepo = adminRepo;
            _jwtService = jwtService;
            _adminRoleRepo = adminRoleRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account">用户名/邮箱/手机</param>
        /// <param name="hashpwd">加密后的密码 md5({account}{captchaTsp})</param>
        /// <param name="captchaTsp">验证码和时间戳的拼接</param>
        /// <param name="remember">是否记住账号密码</param>
        /// <returns></returns>
        public async Task<IActionResult> Login(string account, string hashpwd, string captchaTsp, int remember)
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
            if (Security.GenerateMD5Hash(password + captchaCode) != hashpwd)
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
                    access_token = _jwtService.Make(adminRole.AdminId.ToString(), adminRole.RoleId.ToString(), remember == 1)
                }));

        }
        
        public IActionResult CheckJwt(string jwt)
        {
            return Json(_resp.success(new { result = _jwtService.Validate(jwt) }));
        }
    }
}
