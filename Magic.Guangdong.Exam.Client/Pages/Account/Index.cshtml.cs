using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Magic.Guangdong.Exam.Client.Pages.Account
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public async void OnGet()
        {
            if (!User.Claims.Any(u => u.Type == "sub"))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme);
                //return View();
            }
            var userId = User.Claims.Where(u => u.Type == "sub").FirstOrDefault().Value;
            //var userModel = await userCenterRepo.getUser(Convert.ToInt32(userId));

            HttpContext.Response.Cookies.Append("accountId", userId.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddHours(2)
            });
        }
    }
}
