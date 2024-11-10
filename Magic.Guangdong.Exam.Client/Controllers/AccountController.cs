//using Authing.ApiClient.Auth;
//using Authing.ApiClient.Auth.Types;
using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Account;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Passport.DbServices.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //private readonly AuthenticationClient _authenticationClient;
        private readonly IConfiguration _configuration;
        private readonly IUserBaseRepo _userBaseRepo;
        private readonly ICapPublisher _capPublisher;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IResponseHelper _resp;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserCenterRepo _userCenterRepo;
       
        public AccountController(IResponseHelper resp,
            ICapPublisher capPublisher,
            IUserBaseRepo userBaseRepo, 
            IRedisCachingProvider redisCachingProvider,
            IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor,
            IUserCenterRepo userCenterRepo)
        {
            _resp = resp;
            _userBaseRepo = userBaseRepo;
            _capPublisher = capPublisher;
           // _authenticationClient = authenticationClient;
            _redisCachingProvider = redisCachingProvider;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _userCenterRepo = userCenterRepo;
        }



        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> ExternalLoginCallback()
        {
            // Handle the external login callback.
            // For example, you can retrieve the user's information from the ClaimsPrincipal.
            var result = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
            if (result?.Principal != null)
            {
                // Access the user's claims here.
                var userName = result.Principal.Identity.Name;
                var userEmail = result.Principal.FindFirstValue(ClaimTypes.Email);

                // You can now use this information for further processing.
                // For example, create a session or store the data in a database.
            }

            // Redirect back to the application's home page or another location.
            return RedirectToPage("me");
        }

        public async Task<IActionResult> Logout(string idToken, string redirectUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme);
            string url = $"{ConfigurationHelper.GetSectionValue("authHost")}/connect/endsession?id_token_hint={idToken}&post_logout_redirect_uri={redirectUrl}&state={Guid.NewGuid().ToString("N")}&x-client-SKU=ID_NETSTANDARD2_0&x-client-ver=5.5.0.0";
            HttpContext.Response.Cookies.Delete("accountId");
            HttpContext.Response.Cookies.Delete("accountName");
            HttpContext.Response.Cookies.Delete("idToken");
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            HttpContext.Response.Cookies.Delete("clientsign");
            Console.WriteLine(url);
            return Redirect(url);
            //return Content("已退出");
        }

        //public async Task<IActionResult> AuthingCallBack([FromQuery] string code)
        //{
        //    // 无效 Code 处理
        //    if (code == null)
        //    {
        //        //throw new BadRequestException("code 无效");
        //        return Content("code 无效");
        //    }
        //    CodeToTokenRes tokenInfo;
        //    try
        //    {
        //        // 错误的 Code 可能会导致换取 Token 失败，出现异常大部分都是 Code 错误的原因
        //        tokenInfo = await _authenticationClient.GetAccessTokenByCode(code);

        //    }
        //    catch (Exception ex)
        //    {
        //        // 抛出错误处理，传入 Code 有问题
        //        Assistant.Logger.Error("code 无效"+ex.Message);
        //        return Content("code 无效");
        //    }
        //    var token = tokenInfo.AccessToken;
        //    Assistant.Logger.Warning(token);
        //    var accountId = JwtService.DecodeJwtString(token);
        //    if (accountId == "expired")
        //    {
        //        return Json(_resp.error("登录超时"));
        //    }
        //    //_capPublisher.Publish("SyncAccountInfo", accountId);
        //    await _capPublisher.PublishAsync(CapConsts.PREFIX + "SyncAccountInfo", accountId);



        //    _httpContextAccessor.HttpContext.Response.Cookies.Append("authToken", Security.GenerateMD5Hash(token),
        //        new CookieOptions()
        //        {
        //            Expires = DateTime.Now.AddHours(5),
        //            HttpOnly = true,
        //            SameSite=SameSiteMode.Lax
        //        });

        //    _httpContextAccessor.HttpContext.Response.Cookies.Append("accessToken", token,
        //        new CookieOptions()
        //        {
        //            Expires = DateTime.Now.AddHours(5),
        //            SameSite = SameSiteMode.Lax
        //        });
        //    _httpContextAccessor.HttpContext.Response.Cookies.Append("accountId", accountId,
        //        new CookieOptions()
        //        {
        //            Expires = DateTime.Now.AddHours(5),
        //            SameSite = SameSiteMode.Lax                    
        //        });
        //    return Redirect("/Account/me");

        //}
        //public IActionResult Login()
        //{
        //    // 配置 OIDC 相关信息
        //    var oauthOption = new OidcOption
        //    {
        //        AppId = _configuration["Authing.Config:AppId"],
        //        RedirectUri = _configuration["Authing.Config:RedirectUri"]
        //    };
        //    // 生成对应的 loginUrl
        //    var loginUri = _authenticationClient.BuildAuthorizeUrl(oauthOption);
        //    return Redirect(loginUri);
        //}

        //public async Task<IActionResult> Logout(string accountId)
        //{

        //    if (await _redisCachingProvider.HExistsAsync("accountToken", accountId))
        //    {
        //        string token = await _redisCachingProvider.HGetAsync("accountToken", accountId);

        //        await _authenticationClient.RevokeToken(token);

        //        var url = _authenticationClient.BuildLogoutUrl(new LogoutParams
        //        {
        //            Expert = true,
        //            IdToken = token.Trim('"'),
        //            // 跳转 url 可以自定义，当用户登出成功时将跳转到这个地址，此处默认为 "http://localhost:5000"
        //            RedirectUri = "https://localhost:7296",
        //        });
        //        return Redirect(url);
        //    }
        //    // 根据配置信息生成登出 url
        //    return Redirect("/auth/login");
        //    Assistant.Logger.Warning("退出登录");

        //}

        //[HttpPost,ValidateAntiForgeryToken]
        [ResponseCache(Duration = 600,VaryByQueryKeys = new string[] {"accountId","rd"})]
        public async Task<IActionResult> GetUserInfo(string accountId)
        {
            Logger.Debug("获取用户信息");
            if (await _userBaseRepo.getAnyAsync(u => u.AccountId == accountId))
            {
                var account = await _userBaseRepo.getOneAsync(u => u.AccountId == accountId);
                return Json(_resp.success(account.Adapt<AccountDto>()));

            }
            int uid = 0;
            if(!int.TryParse(accountId,out uid))
            {
                Logger.Debug("获取失败");
                return Redirect("/account/me");
            }
            if (await _userCenterRepo.getAnyAsync(u => u.UID == uid))
            {
                Logger.Debug("获取成功");
                var passportUser = await _userCenterRepo.getOneAsync(u => u.UID == uid);
                if (await _userBaseRepo.getAnyAsync(u => u.AccountId == uid.ToString()))
                {
                    var userBase = await _userBaseRepo.getOneAsync(u => u.AccountId == uid.ToString());
                    return Json(_resp.success(userBase.Adapt<AccountDto>()));
                }
                var newUserBase = new UserBase()
                {
                    AccountId = accountId,
                    Name = passportUser.Name,
                    Password = Utils.GenerateRandomCodePro(8, 2),
                    IdCard = passportUser.IdentityNo,
                    Sex = string.IsNullOrEmpty(passportUser.IdentityNo) ? 0
                    : passportUser.IdentityNo.Length == 18 ? Convert.ToInt32(passportUser.IdentityNo.Substring(16, 1)) % 2 : 0,
                    Email = passportUser.Email,
                    Mobile = passportUser.Mobile,                    
                };

                //await _userBaseRepo.addItemAsync(newUserBase);
                await _userBaseRepo.InsertUserBaseSecurity(newUserBase);
                return Json(_resp.success(newUserBase.Adapt<AccountDto>()));
            }
            return Redirect("/account/me");
            // 如果用户没有进行登录，则跳转到 /auth/login 进行登录
            //return Redirect("/auth/login");
        }

        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "SyncAccountInfo")]
        public async Task SyncAccountInfo(string accountId)
        {
            Logger.Warning($"{DateTime.Now},开始同步用户数据");
            
            if (await _userBaseRepo.getAnyAsync(u => u.AccountId == accountId))
            {
                var ub = await _userBaseRepo.getOneAsync(u => u.AccountId == accountId);
                //这里应该有更新的逻辑
                //但authing 那边的接口还有问题，暂时没法处理，
                //todo...
            }
            else
            {
                UserBase ub = new UserBase();
                ub.AccountId = accountId;
                ub.Name = Utils.GenerateRandomCodePro(8, 3);
                ub.Sex = new Random().Next(0, 2);
                ub.Password = Utils.GenerateRandomCodePro(8, 2);
                await _userBaseRepo.insertOrUpdateAsync(ub);
            }
        }

        /// <summary>
        /// 发送验证码
        /// TODO.. 这里后续要增加限流机制，避免被频繁请求
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateCode([FromServices] IWebHostEnvironment _webHostEnvironment,string to, string username, int length = 4)
        {
            string code = Utils.GenerateRandomCode(length);
            //15+1分钟有效（冗余1分钟）
            await _redisCachingProvider.StringSetAsync(to+"_report", code, DateTime.Now.AddMinutes(16) - DateTime.Now);

            if (to.Contains("@"))
            {
                string htmlContent;
                string templateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "web", "emailcode.html");
                using (StreamReader reader = new StreamReader(templateFilePath))
                {
                    htmlContent = reader.ReadToEnd().Replace("**content**", code);

                }
                await EmailKitHelper.SendVerificationCodeEmailAsync("报名邮箱验证", htmlContent, to, username);
            }
            return Json(_resp.success("发送成功"));
        }

        
        public async Task<IActionResult> VerifyEmailCode(string email,string code)
        {
            if(!await _redisCachingProvider.KeyExistsAsync(email + "_report")) {
                return Json(_resp.error("验证码错误"));
            }

            string verifyCode = await _redisCachingProvider.StringGetAsync(email + "_report");
            if (verifyCode == code)
            {
                await _redisCachingProvider.KeyDelAsync(email + "_report");
                return Json(_resp.success("验证成功"));
            }
            return Json(_resp.error("验证码错误"));
        }
    }
}
