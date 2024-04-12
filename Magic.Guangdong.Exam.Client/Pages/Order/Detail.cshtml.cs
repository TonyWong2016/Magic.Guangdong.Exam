using FreeSql.Internal;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Magic.Guangdong.Exam.Client.Pages.Order
{
    public class DetailModel : PageModel
    {        
        public string Sub { get; set; }

        public void OnGet(string sub)
        {
            Sub = sub;
            if (string.IsNullOrEmpty(Sub))
            {
                Redirect("/Error?msg=invaild");
            }
            //Guid examId;
            //if(!Guid.TryParse(Assistant.Utils.FromBase64Str(Sub), out examId))
            //{
            //    Redirect("/Error?msg=invaild");
            //}
        }
    }
}
