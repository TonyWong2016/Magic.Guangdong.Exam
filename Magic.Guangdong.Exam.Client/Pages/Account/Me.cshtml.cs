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
                Assistant.Logger.Debug($"ƾ֤��Ȼ��Ч" +
                    $" \r\n accountId:{requestCookies.Where(u => u.Key == "accountId").First().Value}," +
                    $" \r\n idToken:{requestCookies.Where(u => u.Key == "idToken").First().Value}");

                return;
            }
            Assistant.Logger.Debug("1.��ƾ֤����ʼ��¼��Ϣ");
            var result = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
            if (result?.Principal == null)
            {
                return;
            }
            string unsignstr = "";
            if (result.Principal.Claims.Any(u => u.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))
            {
                var accountId = result.Principal.Claims.Where(u => u.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
                Assistant.Logger.Debug($"2.��¼accountId:{accountId}");
                Response.Cookies.Append("accountId", accountId, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddHours(6)
                });
                unsignstr += accountId;
            }
            if (result.Properties.Items.Any(u => u.Key == ".Token.id_token"))
            {
                ViewData.Add("idToken", idToken);
                Assistant.Logger.Debug($"2.��¼idToken:{idToken}");
                idToken = result.Properties.Items.Where(u => u.Key == ".Token.id_token").First().Value;
                
                //����idtoken��¼��RedisҲ��ok�ģ������ڸ㲻���û������Ǳ߾���Ĳ��Ե�������ô���õģ����ڷ���˺ܿ��ܻ��泤�ڴ��ڣ��˷���Դ
                //����ͷŵ��ͻ��ˣ������û�idһ������������
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
            Assistant.Logger.Debug("��¼���");
        }


    }
}
