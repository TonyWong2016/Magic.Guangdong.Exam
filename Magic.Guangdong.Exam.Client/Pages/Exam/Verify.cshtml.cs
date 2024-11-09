using FreeSql.Internal;
using Magic.Guangdong.Assistant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Magic.Guangdong.Exam.Client.Pages.Exam
{
    public class VerifyModel : PageModel
    {
        public string examId { get; set; } = "";
        public string groupCode { get; set; } = "";

        public long reportId { get; set; }

        public string reportNumber { get; set; }
        public IActionResult OnGet(string examId, string groupCode, long? reportId)
        {
            if (string.IsNullOrEmpty(examId) && reportId == null)
            {
                Assistant.Logger.Error("非法请求");
                return Redirect("/Error?msg=" + Assistant.Utils.EncodeUrlParam("非法请求"));

            }
            //后面那个条件是为了兼容旧版，真的我当时脑残了用这个参数做判断~
            if (!string.IsNullOrEmpty(groupCode) 
                && groupCode != "auto" 
                && !Request.Cookies.Where(u => u.Key == "accountId").Any())
            {
                string accountId = "nologinrequired-" + groupCode;
                Response.Cookies.Append("accountId", accountId, new CookieOptions()
                {
                    Expires = DateTimeOffset.Now.AddDays(1),
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax
                });
                string idToken = "nologinrequired-" + Assistant.Utils.DateTimeToTimeStamp(DateTime.Now);
                Response.Cookies.Append("idToken", idToken, new CookieOptions()
                {
                    Expires = DateTimeOffset.Now.AddDays(1),
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax
                });
                Response.Cookies.Append("clientsign",Security.GenerateMD5Hash(accountId+idToken+ConfigurationHelper.GetSectionValue("SecretPwd")), new CookieOptions()
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.Now.AddHours(6)
                });
            }
            this.examId = examId;
            this.groupCode = groupCode;
            if (reportId != null)
                this.reportId = (long)reportId;
            //return Redirect($"/Exam/Index?examId={examId}&groupCode={groupCode}&reportId={reportId}");
            return Page();
        }
    }
}
