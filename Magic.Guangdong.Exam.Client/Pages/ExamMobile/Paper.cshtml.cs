using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;


namespace Magic.Guangdong.Exam.Client.Pages.ExamMobile
{
    public class PaperModel : PageModel
    {
        private readonly IUserAnswerRecordClientRepo _userAnswerRecordClientRepo;
        //private readonly IRedisCachingProvider _redisCachingProvider;
        public PaperModel(IUserAnswerRecordClientRepo userAnswerRecordClientRepo)
        {
            _userAnswerRecordClientRepo = userAnswerRecordClientRepo;
            //_redisCachingProvider = redisCachingProvider;
        }

        [BindProperty]
        public long urid { get; set; }

        [BindProperty]
        public DateTime LimitedTime { get; set; }

        [BindProperty]
        public DateTime CreatedAt { get; set; }
        
        [BindProperty]
        public Guid PaperId { get; set; }

        [BindProperty]
        public string ReportNumber { get; set; }

        [BindProperty]
        public string ExamTitle { get; set; }

        [BindProperty]
        public string SubmitAnswer { get; set; }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public double UsedTime { get; set; } = 0;

        [BindProperty]
        public string ReportId { get; set; }

        [BindProperty]
        public double RemainSecond { get; set; } = 0;

        public async Task<IActionResult> OnGet(long urid)
        {
            this.urid = urid;

            //页面加载时不要走缓存，读取最新的。
            var record = await _userAnswerRecordClientRepo.GetMyRecord(urid);
            if (!Request.Headers.Any(u => u.Key == "Referer") || record.IsDeleted == 1)
            {
                Logger.Error("非法请求");
                return Redirect("/Error?msg=" + Utils.EncodeUrlParam("非法请求"));
            }

            if (record.Complated == (int)ExamComplated.Yes)
            {
                Logger.Error("已经完成答题");
                return Redirect("/exammobile/result?urid=" + urid);
            }

            var refer = Request.Headers.Where(u => u.Key == "Referer").First();
            string referUrl = refer.Value.ToString().ToLower();
            //不可以从结果页回到答题页，也不可以从非考试页进入答题页
            if (!referUrl.Contains("exam") || referUrl.Contains("/result") || record.Complated == (int)ExamComplated.Yes)
            {
                return Redirect($"/exammobile/index?examId={record.ExamId}");
            }
            if (record == null || string.IsNullOrEmpty(record.ReportId) || string.IsNullOrEmpty(record.IdNumber))
            {
                return Content("试题尚未初始化或进行了非规范答题，请尝试重新扫码进入答题界面，或者联系管理人员");
            }
            if (record.LimitedTime < DateTime.Now)
            {
                return Redirect($"/exammobile/Result?urid={record.Id}&force=1");
            }

            if (record.IdNumber != record.ReportNumber)
            {
                return Content("试题尚未初始化或进行了非规范答题，请尝试重新扫码进入答题界面，或者联系管理人员");
            }
            LimitedTime = record.LimitedTime;
            CreatedAt = record.CreatedAt;
            PaperId = record.PaperId;
            ReportNumber = record.ReportNumber;
            Name = record.Name;
            ExamTitle = record.ExamTitle;
            SubmitAnswer = record.SubmitAnswer;
            ReportId = record.ReportId;
            
            UsedTime = Math.Floor((DateTime.Now - record.CreatedAt).TotalSeconds);
            RemainSecond = Math.Floor((record.LimitedTime - DateTime.Now).TotalSeconds);
            return Page();
        }
    }
}
