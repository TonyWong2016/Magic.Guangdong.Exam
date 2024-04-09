using Authing.ApiClient.Auth;
using Authing.ApiClient.Auth.Types;
using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthenticationClient _authenticationClient;
        private readonly IConfiguration _configuration;
        private readonly IUserBaseRepo _userBaseRepo;
        private readonly ICapPublisher _capPublisher;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IResponseHelper _resp;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IResponseHelper resp,ICapPublisher capPublisher,AuthenticationClient authenticationClient,IUserBaseRepo userBaseRepo, IRedisCachingProvider redisCachingProvider, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _resp = resp;
            _userBaseRepo = userBaseRepo;
            _capPublisher = capPublisher;
            _authenticationClient = authenticationClient;
            _redisCachingProvider = redisCachingProvider;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }



        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AuthingCallBack([FromQuery] string code)
        {
            // 无效 Code 处理
            if (code == null)
            {
                //throw new BadRequestException("code 无效");
                return Content("code 无效");
            }
            CodeToTokenRes tokenInfo;
            try
            {
                // 错误的 Code 可能会导致换取 Token 失败，出现异常大部分都是 Code 错误的原因
                tokenInfo = await _authenticationClient.GetAccessTokenByCode(code);
                
            }
            catch (Exception ex)
            {
                // 抛出错误处理，传入 Code 有问题
                Assistant.Logger.Error("code 无效"+ex.Message);
                return Content("code 无效");
            }
            var token = tokenInfo.AccessToken;
            Assistant.Logger.Warning(token);
            var accountId = JwtService.DecodeJwtString(token);
            if (accountId == "expired")
            {
                return Json(_resp.error("登录超时"));
            }
            //_capPublisher.Publish("SyncAccountInfo", accountId);
            await _capPublisher.PublishAsync(CapConsts.PREFIX + "SyncAccountInfo", accountId);

            

            _httpContextAccessor.HttpContext.Response.Cookies.Append("authToken", Security.GenerateMD5Hash(token),
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddHours(5),
                    HttpOnly = true,
                    SameSite=SameSiteMode.Lax
                });

            _httpContextAccessor.HttpContext.Response.Cookies.Append("accessToken", token,
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddHours(5),
                    SameSite = SameSiteMode.Lax
                });
            _httpContextAccessor.HttpContext.Response.Cookies.Append("accountId", accountId,
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddHours(5),
                    SameSite = SameSiteMode.Lax                    
                });
            return Redirect("/Account/me");
            
        }
        public IActionResult Login()
        {
            // 配置 OIDC 相关信息
            var oauthOption = new OidcOption
            {
                AppId = _configuration["Authing.Config:AppId"],
                RedirectUri = _configuration["Authing.Config:RedirectUri"]
            };
            // 生成对应的 loginUrl
            var loginUri = _authenticationClient.BuildAuthorizeUrl(oauthOption);
            return Redirect(loginUri);
        }

        public async Task<IActionResult> Logout(string accountId)
        {

            if (await _redisCachingProvider.HExistsAsync("accountToken", accountId))
            {
                string token = await _redisCachingProvider.HGetAsync("accountToken", accountId);
                    
                await _authenticationClient.RevokeToken(token);
                    
                var url = _authenticationClient.BuildLogoutUrl(new LogoutParams
                {
                    Expert = true,
                    IdToken = token.Trim('"'),
                    // 跳转 url 可以自定义，当用户登出成功时将跳转到这个地址，此处默认为 "http://localhost:5000"
                    RedirectUri = "https://localhost:7296",
                });
                return Redirect(url);
            }
            // 根据配置信息生成登出 url
            return Redirect("/auth/login");
            Assistant.Logger.Warning("退出登录");
            
        }

        //[HttpPost,ValidateAntiForgeryToken]
        [ResponseCache(Duration = 600,VaryByQueryKeys = new string[] {"accountId","rd"})]
        public async Task<IActionResult> GetUserInfo(string accountId)
        {
            if (await _userBaseRepo.getAnyAsync(u => u.AccountId == accountId))
                return Json(_resp.success(await _userBaseRepo.getOneAsync(u => u.AccountId == accountId)));
            // 如果用户没有进行登录，则跳转到 /auth/login 进行登录
            return Redirect("/auth/login");
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
    }
}
