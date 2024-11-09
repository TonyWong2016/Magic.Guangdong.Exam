using EasyCaching.Core;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Magic.Passport.DbServices.Interfaces;
using Microsoft.Identity.Client;

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
            var requestCookies = _httpContextAccessor.HttpContext.Request.Cookies;
            if (requestCookies.Any(u => u.Key == "accountId") &&
                requestCookies.Any(u => u.Key == "idToken") &&
                !string.IsNullOrEmpty(requestCookies.Where(u => u.Key == "idToken").FirstOrDefault().Value) &&
                requestCookies.Any(u=>u.Key== "clientsign") 
                )
            {
                Assistant.Logger.Debug($"凭证仍然有效" +
                    $" \r\n accountId:{requestCookies.Where(u => u.Key == "accountId").First().Value}," +
                    $" \r\n idToken:{requestCookies.Where(u => u.Key == "idToken").First().Value}");

                return;
            }
            Assistant.Logger.Debug("1.无凭证，开始记录信息");
            var result = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
            if (result?.Principal == null)
            {
                return;
            }
            string unsignstr = "";
            if (result.Principal.Claims.Any(u => u.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))
            {
                var accountId = result.Principal.Claims.Where(u => u.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
                Assistant.Logger.Debug($"2.记录accountId:{accountId}");
                Response.Cookies.Append("accountId", accountId, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddHours(6)
                });
                unsignstr += accountId;
            }
            if (result.Properties.Items.Any(u => u.Key == ".Token.id_token"))
            {
                ViewData.Add("idToken", idToken);
                Assistant.Logger.Debug($"2.记录idToken:{idToken}");
                idToken = result.Properties.Items.Where(u => u.Key == ".Token.id_token").First().Value;
                
                //这里idtoken记录到Redis也是ok的，但由于搞不清用户中心那边具体的策略到底是怎么设置的，放在服务端很可能缓存长期存在，浪费资源
                //这里就放到客户端，跟着用户id一样的生命周期
                Response.Cookies.Append("idToken", idToken, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddHours(6)
                });
                unsignstr += idToken;

            }
            string sign = Assistant.Security.GenerateMD5Hash(unsignstr + Assistant.ConfigurationHelper.GetSectionValue("SecretPwd"));

            Response.Cookies.Append("clientsign", sign, new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddHours(6)
            });
            Assistant.Logger.Debug("登录完成");
        }


    }
}
