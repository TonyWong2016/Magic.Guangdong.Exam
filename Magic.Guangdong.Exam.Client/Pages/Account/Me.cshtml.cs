using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Magic.Guangdong.Exam.Client.Pages.Account
{
    public class MeModel : PageModel
    {
        private readonly IRedisCachingProvider redisCachingProvider;
        public MeModel(IRedisCachingProvider redisCachingProvider)
        {
            this.redisCachingProvider = redisCachingProvider;
        }
        public void OnGet()
        {
            
        }
    }
}
