using EasyCaching.Core;
using Essensoft.Paylink.Alipay.Domain;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Cryptography;
using Newtonsoft.Json;

namespace Magic.Guangdong.Exam.Areas.WebApi.Controllers
{
    [Area("webapi")]
    public class ReceiveApiController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IFileRepo _fileRepo;
        private readonly IRedisCachingProvider _redisProvider;


        public ReceiveApiController(IResponseHelper responseHelper, IFileRepo fileRepo,IRedisCachingProvider redisCachingProvider)
        {
            _resp = responseHelper;
            _fileRepo = fileRepo;
            _redisProvider = redisCachingProvider;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return Json(_resp.success(DateTime.Now));
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CssPushCallback([FromBody] CallbackRequestModel model)
        {
            Logger.Warning($"推拉流回调：CssPushCallback: {JsonConvert.SerializeObject(model)}");
            string sign = Security.GenerateMD5Hash("magicexam" + model.t);
            if (model.sign != sign)
            {
                Logger.Error("签名验证失败，" + sign);
                return Json(_resp.error("我靠，你谁啊！"));                
            }
            Logger.Info("签名验证通过，" + sign);
            //
            return Json(_resp.success("success"));
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CssRecordCallback([FromBody] CallbackRecordModel model)
        {
            Logger.Warning($"录制回调：CallbackRecordModel: {JsonConvert.SerializeObject(model)}");
            string sign = Security.GenerateMD5Hash("magicexam"+model.t);
            if (model.sign != sign)
            {
                Logger.Error("签名验证失败，" + sign);
                return Json(_resp.error("我靠，你谁啊！"));
            }
            Logger.Info("签名验证通过，" + sign);
            return Json(_resp.success("success"));
        }

        private string HandleRecordVideoUrl(string mediaUrl,string downloadName)
        {
            long timeStamp = Convert.ToInt64((DateTime.Now.AddDays(1) - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
            string[] parts = mediaUrl.Split('/');
            string dir = "/";
            foreach (string part in parts)
            {
                if (part.Contains(".") || part.Contains("/") || part.Contains(":") || string.IsNullOrEmpty(part))
                    continue;
                dir += $"{part}/";
            }
            //16进制Unix时间戳
            string t = Convert.ToString(timeStamp, 16).ToLower().PadLeft(8, '0');
            string us = Utils.GenerateRandomCodeFast(10);
            //Console.WriteLine(urlKey + dir + t + us);
            //签名=md5(防盗key + dir + 16进制时间戳 + 随机数)
            string sign = Security.GenerateMD5Hash("BVOd6Rbjd5bwVW6ynCcj" + dir + t + us);
            string downloadUrl = $"{mediaUrl}?download_name={downloadName}&t={t}&us={us}&sign={sign}";
            Logger.Warning(downloadUrl);
            return downloadUrl;
        }

    }

    public class CallbackRequestModel
    {
        public int appid { get; set; }
        public string app { get; set; }
        public string appname { get; set; }
        public string stream_id { get; set; }
        public string channel_id { get; set; }
        public long event_time { get; set; }

        public int event_type { get; set; }
        public string sequence { get; set; }
        public string node { get; set; }
        public string user_ip { get; set; }
        public string stream_param { get; set; }
        public string push_duration { get; set; }
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public int set_id { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public long t { get; set; }

        public string sign { get; set; }
    }

    public class CallbackRecordModel
    {
        public long t { get; set; }

        public string sign { get; set; }

        public int event_type { get; set; }

        public int appid { get; set; }

        public string app { get; set; }

        public string callback_ext { get; set; }

        public string appname { get; set; }

        public string stream_id { get; set; }

        public string channel_id { get; set; }

        public string file_id { get; set; }

        public string record_file_id { get; set; }

        public string file_format { get; set; }

        public string task_id { get; set; }

        public long start_time { get; set; }

        public long end_time { get; set; }

        public int start_time_usec { get; set; }

        public int end_time_usec { get; set; }

        public int duration { get; set; }

        public ulong file_size { get; set; }

        public string stream_param { get; set; }

        public string video_url { get; set; }

        public long media_start_time { get; set; }

        public int record_bps { get; set; }

       
    }
}
