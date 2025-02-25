using Magic.Guangdong.Exam.Client.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace Magic.Guangdong.Exam.Client.Pages.Monitor
{
    public class WatchModel : PageModel
    {
        private IHubContext<MyHub> _myHub;
        public WatchModel(IHubContext<MyHub> myhub)
        {
            _myHub = myhub;
        }
        public async Task OnGet()
        {
            await _myHub.Clients.All.SendAsync("ReceiveMessage", "≤‚ ‘œ˚œ¢£∫"+DateTimeOffset.UtcNow);
        }
    }
}
