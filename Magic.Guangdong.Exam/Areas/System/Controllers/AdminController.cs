using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Dto.Admin;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
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
        /// 登录
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
            if (admin.Status != 0)
            {
                return Json(_resp.error("账户异常，请联系平台管理人员"));
            }
            //var adminRole = await _adminRoleRepo.getOneAsync(u => u.AdminId == admin.Id);
            DateTime expires = remember == 1 ? DateTime.Now.AddDays(3) : DateTime.Now.AddHours(3);
            string jwt = _jwtService.Make(Utils.ToBase64Str(admin.Id.ToString()), admin.Name, expires);
            await _capPublisher.PublishAsync(CapConsts.PREFIX + "SubmitLoginLog", $"{admin.Id}|{jwt}|{Utils.DateTimeToTimeStamp(expires)}");
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
            //await _redisCachingProvider.StringSetAsync(parts[0], parts[2], Utils.TimeStampToDateTime(Convert.ToInt64(parts[2])) - DateTime.Now);
            await _adminLoginLogRepo.InsertLoginLog(Guid.Parse(parts[0]), parts[1], parts[2]);
            
            //await _userAnswerRecordRepo.SubmitMyPaper(dto);
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
            //这里要加入token作废机制，定期自动删除
            //Response.Cookies.Delete("examToken");
            //Response.Cookies.Delete("userId");
            return Redirect("/system/admin/login?msg=logout");
        }

        [AllowAnonymous]
        [RouteMark("注册管理员")]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost,ValidateAntiForgeryToken]        
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            string VerificationCode="";
            if (await _redisCachingProvider.KeyExistsAsync(dto.Email))
            {
                VerificationCode = await _redisCachingProvider.StringGetAsync(dto.Email);
            }
            else if(await _redisCachingProvider.KeyExistsAsync(dto.Mobile))
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
            admin.KeySecret = keySecret;
            admin.NickName = dto.NickName;
            return Json(_resp.success(await _adminRepo.addItemAsync(admin), "注册成功"));
        }
        
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public  async Task<IActionResult> GenerateCode(string to,string username,int length=4)
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
