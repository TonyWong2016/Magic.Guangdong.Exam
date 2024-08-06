using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Magic.Guangdong.DbServices.Dtos.System.Admins;
using Essensoft.Paylink.Alipay.Domain;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    /// <summary>
    /// 账号相关
    /// </summary>
    [Area("system")]
    public class AccountController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IAdminRepo _adminRepo;
        private readonly IJwtService _jwtService;
        private readonly IAdminRoleRepo _adminRoleRepo;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICapPublisher _capPublisher;
        private readonly IAdminLoginLogRepo _adminLoginLogRepo;
        private readonly IRoleRepo _roleRepo;
        public AccountController(IResponseHelper resp, IAdminRepo adminRepo, IJwtService jwtService, IAdminRoleRepo adminRoleRepo, IRedisCachingProvider redisCachingProvider, IWebHostEnvironment webHostEnvironment, ICapPublisher capPublisher, IAdminLoginLogRepo adminLoginLogRepo,IRoleRepo roleRepo)
        {
            _resp = resp;
            _adminRepo = adminRepo;
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
            if (!await _redisCachingProvider.KeyExistsAsync("CaptchaStr"))
            {
                await _redisCachingProvider.StringSetAsync("CaptchaStr", Utils.GenerateRandomCodePro(1000, 3), DateTime.Now.AddDays(1) - DateTime.Now);
            }
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account">用户名/邮箱/手机</param>
        /// <param name="hashpwd">加密后的密码 md5({password}{captchaTsp})</param>
        /// <param name="captchaTsp">验证码和时间戳的拼接</param>
        /// <param name="remember">是否记住账号密码</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string account, string hashpwd, string captchaTsp, int remember = 0)
        {
            if (!await _adminRepo.getAnyAsync(
                u =>
                u.Name.Equals(account) ||
                u.Email.Equals(account) ||
                u.Mobile.Equals(account) &&
                u.IsDeleted == 0))
            {
                return Json(_resp.error("用户名不存在"));
            }
            var admin = await _adminRepo.getOneAsync(
                u =>
                u.Name.Equals(account) ||
                u.Email.Equals(account) ||
                u.Mobile.Equals(account) &&
                u.IsDeleted == 0);

            string password = Security.Decrypt(admin.Password, Encoding.UTF8.GetBytes(admin.KeyId), Encoding.UTF8.GetBytes(admin.KeySecret));
            string tt = Security.GenerateMD5Hash(password + captchaTsp);
            Console.WriteLine(tt);
            if (Security.GenerateMD5Hash(password + captchaTsp) != hashpwd)
            {
                //这里实际是密码错误，但返回的信息要模糊一下，提防坏人！
                return Json(_resp.error("用户名或密码错误"));
            }
            if (admin.Status != 0)
            {
                return Json(_resp.error("账户异常，请联系平台管理人员"));
            }
            //var adminRole = await _adminRoleRepo.getOneAsync(u => u.AdminId == admin.Id);
            DateTime expires = remember == 1 ? DateTime.Now.AddDays(3) : DateTime.Now.AddHours(3);
            await CacheMyPermission(new AfterLoginDto() { adminId = admin.Id, exp = expires });
            string jwt = _jwtService.Make(Utils.ToBase64Str(admin.Id.ToString()), admin.Name, expires);
            await _capPublisher.PublishAsync(CapConsts.PREFIX + "SubmitLoginLog", $"{admin.Id}|{jwt}|{Utils.DateTimeToTimeStamp(expires)}");
            //await _capPublisher.PublishAsync(CapConsts.PREFIX + "CacheMyPermission", new AfterLoginDto() { adminId = admin.Id, exp = expires });

            return Json(_resp.success(
                new
                {
                    access_token = jwt
                }));

        }

        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "SubmitLoginLog")]
        public async Task SubmitLoginLog(string jwtIdExp)
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
        [RouteMark("注册管理员")]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 注册(自主注册不可以授权，需要管理员后台授权)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _adminRepo.getAnyAsync(u => u.Name == dto.Name || u.Email == dto.Email || u.Mobile == dto.Mobile))
            {
                return Json(_resp.error("该用户名/邮箱/手机号已存在"));
            }
            string VerificationCode = "";
            if (await _redisCachingProvider.KeyExistsAsync(dto.Email))
            {
                VerificationCode = await _redisCachingProvider.StringGetAsync(dto.Email);
            }
            else if (await _redisCachingProvider.KeyExistsAsync(dto.Mobile))
            {
                VerificationCode = await _redisCachingProvider.StringGetAsync(dto.Mobile);
            }
            if (VerificationCode != dto.VerificationCode)
            {
                return Json(_resp.error("验证码错误"));
            }
            string keySecret = Utils.GenerateRandomCodePro(16);
            string keyId = Utils.GenerateRandomCodePro(16);
            string password = Security.Encrypt(dto.Password, Encoding.UTF8.GetBytes(keyId), Encoding.UTF8.GetBytes(keySecret));
            var admin = new Admin();
            admin.Name = string.IsNullOrEmpty(dto.Name) ? Utils.GenerateRandomCodePro(6) : dto.Name;
            admin.Email = dto.Email;
            admin.Mobile = dto.Mobile;
            admin.Password = password;
            admin.KeyId = keyId;
            admin.Description = dto.Description;
            admin.KeySecret = keySecret;
            admin.NickName = dto.NickName;
            return Json(_resp.success(await _adminRepo.addItemAsync(admin), "注册成功"));
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

        /// <summary>
        /// 为调用api获取token
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="sign"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetAccessTokenForApi(string keyId, string sign, long timestamp)
        {
            TimeSpan ts = DateTime.Now - Utils.TimeStampToDateTime(timestamp);
            if (ts.TotalSeconds > 300)
            {
                return Json(_resp.error("时间戳已过期"));
            }
            if (!await _adminRepo.getAnyAsync(
                u =>
                u.KeyId.Equals(keyId) &&
                u.IsDeleted == 0))
            {
                return Json(_resp.error("用户不存在"));
            }
            var admin = await _adminRepo.getOneAsync(
                u =>
                u.KeyId.Equals(keyId) &&
                u.IsDeleted == 0);

            
            string tt = Security.GenerateMD5Hash(admin.KeyId + admin.KeySecret + timestamp);
            if (tt == sign.ToLower())
            {
                DateTime expires = DateTime.Now.AddHours(6);
                await CacheMyPermission(new AfterLoginDto() { adminId = admin.Id, exp = expires });
                string jwt = _jwtService.Make(Utils.ToBase64Str(admin.Id.ToString()), admin.Name, expires);
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "SubmitLoginLog", $"{admin.Id}|{jwt}|{Utils.DateTimeToTimeStamp(expires)}");
                return Json(_resp.success(
                    new
                    {
                        access_token = jwt
                    }));
            }
            return Json(_resp.error("获取Token失败，请检查传入参数是否正确"));
        }

        [HttpPost]
        [WebApiModule]
        public async Task<IActionResult> ApiTest()
        {
            return Json(_resp.success(Guid.NewGuid()));
        }
    }
}
