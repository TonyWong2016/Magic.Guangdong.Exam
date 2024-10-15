using EasyCaching.Core;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Magic.Passport.DbServices.Interfaces;

namespace Magic.Guangdong.Exam.Client.Pages.Account
{
    [Authorize]
    public class MeModel : PageModel
    {
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IUserCenterRepo _userCenterRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string idToken = "";
        public MeModel(IRedisCachingProvider redisCachingProvider,IUserCenterRepo userCenterRepo,IHttpContextAccessor httpContextAccessor)
        {
            _redisCachingProvider = redisCachingProvider;
            _userCenterRepo = userCenterRepo;
            _httpContextAccessor = httpContextAccessor;
        }
        public async void OnGet()
        {
            Assistant.Logger.Debug("登录完成");
            if (_httpContextAccessor.HttpContext.Request.Cookies.Any(u => u.Key == "accountId"))
            {
                Assistant.Logger.Debug("cookie存在");
                Console.WriteLine("cookie存在");
            }
            else
            {
                Assistant.Logger.Debug("1.开始记录信息");
                var result = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
                if (result?.Principal == null)
                {
                    return;
                }
                if (result.Principal.Claims.Any(u => u.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))
                {
                    var accountId = result.Principal.Claims.Where(u => u.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
                    Response.Cookies.Append("accountId", accountId, new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddHours(6)
                    });
                }
                if (result.Properties.Items.Any(u => u.Key == ".Token.id_token"))
                {
                    ViewData.Add("idToken", idToken);

                    idToken = result.Properties.Items.Where(u => u.Key == ".Token.id_token").First().Value;
                }
            }
        }
    }
}
