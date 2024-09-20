using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Magic.Guangdong.Exam.Client.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public void OnGet()
        {
           
            RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            Assistant.Logger.Error(RequestId);
            //RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }

}
