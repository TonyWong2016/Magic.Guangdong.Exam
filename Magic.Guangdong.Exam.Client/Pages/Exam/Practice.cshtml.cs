using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NPOI.HPSF;

namespace Magic.Guangdong.Exam.Client.Pages.Exam
{
    public class PracticeModel : PageModel
    {
        private readonly IUserAnswerRecordClientRepo _userAnswerRecordClientRepo;
        private readonly IExaminationClientRepo _examinationClientRepo;
        public PracticeModel(IUserAnswerRecordClientRepo examRepo,IExaminationClientRepo examinationClientRepo)
        {
            _userAnswerRecordClientRepo = examRepo;
            _examinationClientRepo = examinationClientRepo;
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

        [BindProperty]
        public int Stage { get; set; }

        public async Task<IActionResult>  OnGet(long urid)
        {
            this.urid = urid;
            //页面加载时不要走缓存，读取最新的。
            var record = await _userAnswerRecordClientRepo.GetMyRecord(urid);
            
            if (record.Complated == (int)ExamComplated.Yes)
            {
                Logger.Error("已经完成练习");
                return Redirect("/exam/result?urid=" + urid);
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
