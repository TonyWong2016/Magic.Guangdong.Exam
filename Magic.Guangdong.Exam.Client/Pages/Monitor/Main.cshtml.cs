using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Exam.Client.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace Magic.Guangdong.Exam.Client.Pages.Monitor
{
    public class MainModel : PageModel
    {
        private readonly IResponseHelper _resp;
        private IHubContext<MyHub> _myHub;
        //private readonly IExaminationClientRepo _examRepo;
        public string pushUrl { get; set; } = "";

        public MainModel(IResponseHelper responseHelper,IHubContext<MyHub> myhub)
        {
            _resp = responseHelper;
            _myHub = myhub;
        }

        public void OnGet()
        {
            DateOnly dto = DateOnly.FromDateTime(DateTime.Now);
            TimeOnly tmo = new TimeOnly(23, 59, 59);
            //pushUrl = GetSafeUrl("86fb0aee6a39e0f6b3390399d905b19f", Utils.GenerateRandomCodeFast(6),Utils.GetUnixTimestamp(new DateTime(2025, 2, 21, 23, 59, 59, DateTimeKind.Utc)));
            pushUrl = GetSafeUrl("86fb0aee6a39e0f6b3390399d905b19f", Utils.GenerateRandomCodeFast(6), Utils.GetUnixTimestamp(new DateTime(dto,tmo, DateTimeKind.Utc)));
        }

        /*
         * KEY + streamName + txTime
         */
        public string GetSafeUrl(string key, string streamName, long txTime)
        {
            string input = $"{key}{streamName}{txTime:X}";

            string txSecret = Security.ByteArrayToHexString(Security.MD5Hash(input));

            string url = $"webrtc://pushtx.xiaoxiaotong.org/live/{streamName}?txSecret={txSecret}&txTime={txTime:X}";
            Logger.Debug(url);
            return url;
        }
    }
}
