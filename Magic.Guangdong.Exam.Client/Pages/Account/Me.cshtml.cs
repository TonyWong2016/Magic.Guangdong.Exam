using EasyCaching.Core;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Magic.Guangdong.Exam.Client.Pages.Account
{
    [Authorize]
    public class MeModel : PageModel
    {
        private readonly IRedisCachingProvider redisCachingProvider;
        public MeModel(IRedisCachingProvider redisCachingProvider)
        {
            this.redisCachingProvider = redisCachingProvider;
        }
        public async void OnGet()
        {
            var result = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
            if (result?.Principal != null)
            {
                // Access the user's claims here.
                var userName = result.Principal.Identity.Name;
                var userEmail = result.Principal.FindFirstValue(ClaimTypes.Email);
                var accountId = result.Principal.Claims.Where(u => u.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
                // You can now use this information for further processing.
                // For example, create a session or store the data in a database.
                HttpContext.Response.Cookies.Append("accountId", accountId.ToString(), new CookieOptions
                {
                    Expires = DateTime.Now.AddHours(6)
                });
            }
        }
    }
}
