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

            //ҳ�����ʱ��Ҫ�߻��棬��ȡ���µġ�
            var record = await _userAnswerRecordClientRepo.GetMyRecord(urid);
            if (!Request.Headers.Any(u => u.Key == "Referer") || record.IsDeleted == 1)
            {
                Logger.Error("�Ƿ�����");
                return Redirect("/Error?msg=" + Utils.EncodeUrlParam("�Ƿ�����"));
            }

            if (record.Complated == (int)ExamComplated.Yes)
            {
                Logger.Error("�Ѿ���ɴ���");
                return Redirect("/exammobile/result?urid=" + urid);
            }

            var refer = Request.Headers.Where(u => u.Key == "Referer").First();
            string referUrl = refer.Value.ToString().ToLower();
            //�����Դӽ��ҳ�ص�����ҳ��Ҳ�����Դӷǿ���ҳ�������ҳ
            if (!referUrl.Contains("exam") || referUrl.Contains("/result") || record.Complated == (int)ExamComplated.Yes)
            {
                return Redirect($"/exammobile/index?examId={record.ExamId}");
            }
            if (record == null || string.IsNullOrEmpty(record.ReportId) || string.IsNullOrEmpty(record.IdNumber))
            {
                return Content("������δ��ʼ��������˷ǹ淶���⣬�볢������ɨ����������棬������ϵ������Ա");
            }
            if (record.LimitedTime < DateTime.Now)
            {
                return Redirect($"/exammobile/Result?urid={record.Id}&force=1");
            }

            if (record.IdNumber != record.ReportNumber)
            {
                return Content("������δ��ʼ��������˷ǹ淶���⣬�볢������ɨ����������棬������ϵ������Ա");
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