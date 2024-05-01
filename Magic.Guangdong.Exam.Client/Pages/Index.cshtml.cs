using Magic.Guangdong.Assistant.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Magic.Guangdong.Exam.Client.Pages
{
    //[Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IResponseHelper _resp;

        public IndexModel(ILogger<IndexModel> logger,IResponseHelper responseHelper)
        {
            _logger = logger;
            _resp = responseHelper;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostTest()
        {
            return new JsonResult(_resp.success("yes"));
        }
    }
}
