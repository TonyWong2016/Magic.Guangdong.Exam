using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Teacher.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Magic.Guangdong.DbServices.Dtos.System.Admins;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    /// <summary>
    /// 账号相关
    /// </summary>
    [Area("system")]
    public class AccountController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ITeacherRepo _teacherRepo;
        private readonly IJwtService _jwtService;
        private readonly IAdminRoleRepo _adminRoleRepo;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICapPublisher _capPublisher;
        private readonly IAdminLoginLogRepo _adminLoginLogRepo;
        private readonly IRoleRepo _roleRepo;
        public AccountController(IResponseHelper resp, ITeacherRepo teacherRepo, IJwtService jwtService, IAdminRoleRepo adminRoleRepo, IRedisCachingProvider redisCachingProvider, IWebHostEnvironment webHostEnvironment, ICapPublisher capPublisher, IAdminLoginLogRepo adminLoginLogRepo,IRoleRepo roleRepo)
        {
            _resp = resp;
            _teacherRepo = teacherRepo;
            _jwtService = jwtService;
            _adminRoleRepo = adminRoleRepo;
            _redisCachingProvider = redisCachingProvider;
            _webHostEnvironment = webHostEnvironment;
            _capPublisher = capPublisher;
            _adminLoginLogRepo = adminLoginLogRepo;
            _roleRepo = roleRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            if (!await _redisCachingProvider.KeyExistsAsync("TeacherCaptchaStr"))
            {
                await _redisCachingProvider.StringSetAsync("TeacherCaptchaStr", Utils.GenerateRandomCodePro(1000, 3), DateTime.Now.AddDays(1) - DateTime.Now);
            }
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account">教师编号/邮箱/手机</param>
        /// <param name="hashpwd">加密后的密码 md5({password}{captchaTsp})</param>
        /// <param name="captchaTsp">验证码和时间戳的拼接</param>
        /// <param name="remember">是否记住账号密码</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string account, string hashpwd, string captchaTsp, int remember = 0)
        {
            if (!await _teacherRepo.getAnyAsync(
                u =>
                u.TeachNo.Equals(account) ||
                u.Email.Equals(account) ||
                u.Mobile.Equals(account) &&
                u.IsDeleted == 0))
            {
                return Json(_resp.error("用户名不存在"));
            }
            var teacher = await _teacherRepo.getOneAsync(
                u =>
                u.TeachNo.Equals(account) ||
                u.Email.Equals(account) ||
                u.Mobile.Equals(account) &&
                u.IsDeleted == 0);

            string password = Security.Decrypt(teacher.Password, Encoding.UTF8.GetBytes(teacher.KeyId), Encoding.UTF8.GetBytes(teacher.KeySecret));
            string tt = Security.GenerateMD5Hash(password + captchaTsp);
            Console.WriteLine(tt);
            if (Security.GenerateMD5Hash(password + captchaTsp) != hashpwd)
            {
                //这里实际是密码错误，但返回的信息要模糊一下，提防坏人！
                return Json(_resp.error("用户名或密码错误"));
            }
            //var adminRole = await _adminRoleRepo.getOneAsync(u => u.AdminId == admin.Id);
            DateTime expires = remember == 1 ? DateTime.Now.AddDays(3) : DateTime.Now.AddHours(3);
            string jwt = _jwtService.Make(Utils.ToBase64Str(teacher.Id.ToString()), teacher.Name, expires);
            await _capPublisher.PublishAsync(CapConsts.PREFIX + "SubmitTeacherLoginLog", $"{teacher.Id}|{jwt}|{Utils.DateTimeToTimeStamp(expires)}");
            //await _capPublisher.PublishAsync(CapConsts.PREFIX + "CacheMyPermission", new AfterLoginDto() { adminId = admin.Id, exp = expires });

            return Json(_resp.success(
                new
                {
                    access_token = jwt
                }));

        }

        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "SubmitTeacherLoginLog")]
        public async Task SubmitTeacherLoginLog(string jwtIdExp)
        {
            Console.WriteLine($"{DateTime.Now}:消费事务---记录登录日志");
            string[] parts = jwtIdExp.Split('|');
            await _adminLoginLogRepo.InsertLoginLog(Guid.Parse(parts[0]), parts[1], parts[2]);
        }

        //[NonAction]
        //[CapSubscribe(CapConsts.PREFIX + "CacheMyPermission")]
        public async Task CacheMyPermission(AfterLoginDto dto)
        {
            await _redisCachingProvider.KeyDelAsync("GD.Exam.Permissions_" + dto.adminId.ToString());
            var myPermissions = await _adminRoleRepo.GetMyPermission(dto.adminId);
            var superRoles = await _roleRepo.getListAsync(u => u.IsDeleted == 0 && u.Type == 1);
            List<long> superRoleIds = new List<long>();
            if (superRoles.Any())
                superRoleIds = superRoles.Select(u => u.Id).ToList();
            if(superRoleIds.Any() && myPermissions.Where(u=>superRoleIds.Contains(u.RoleId)).Any())            
                await _redisCachingProvider.HSetAsync("GD.Exam.Permissions_" + dto.adminId.ToString(), "super", "super");
            
            //await _redisCachingProvider.ZAddAsync(adminId.ToString(), myPermissions, Utils.TimeStampToDateTime(exp) - DateTime.Now);
            foreach (var myPermission in myPermissions)
            {
                await _redisCachingProvider.HSetAsync("GD.Exam.Permissions_" + dto.adminId.ToString(), myPermission.router, JsonHelper.JsonSerialize(myPermission));
            }
            await _redisCachingProvider.KeyExpireAsync("GD.Exam.Permissions_" + dto.adminId.ToString(), Convert.ToInt32((dto.exp - DateTime.Now).TotalSeconds));
        }

        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CheckJwt(string jwt)
        {
            return Json(_resp.success(new { result = _jwtService.Validate(jwt) }));
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Logout()
        {
            //这里要加入token作废机制，定期自动删除
            return Redirect("/system/account/login?msg=logout");
        }


        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }


        /// <summary>
        /// 发送验证码
        /// TODO.. 这里后续要增加限流机制，避免被频繁请求
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        //[AllowAnonymous]
        public async Task<IActionResult> GenerateCode(string to, string username, int length = 4)
        {
            string code = Utils.GenerateRandomCode(length);
            //15+1分钟有效（冗余1分钟）
            await _redisCachingProvider.StringSetAsync(to, code, DateTime.Now.AddMinutes(16) - DateTime.Now);

            if (to.Contains("@"))
            {
                string htmlContent;
                string templateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "web", "emailcode.html");
                using (StreamReader reader = new StreamReader(templateFilePath))
                {
                    htmlContent = reader.ReadToEnd().Replace("**content**", code);

                }
                await EmailKitHelper.SendVerificationCodeEmailAsync("欢迎注册", htmlContent, to, username);
            }
            return Json(_resp.success("发送成功"));
        }

    }
}
