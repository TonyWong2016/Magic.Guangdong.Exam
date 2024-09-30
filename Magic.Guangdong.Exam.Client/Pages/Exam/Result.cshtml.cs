using EasyCaching.Core;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NPOI.HSSF.Record.Chart;
using OfficeOpenXml.FormulaParsing.Utilities;

namespace Magic.Guangdong.Exam.Client.Pages.Exam
{
    public class ResultModel : PageModel
    {
        private readonly IUserAnswerRecordClientRepo _userAnswerRecordClientRepo;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IFileRepo _fileRepo;
        public ResultModel(IUserAnswerRecordClientRepo userAnswerRecordClientRepo, IFileRepo fileRepo, IRedisCachingProvider redisCachingProvider)
        {
            _userAnswerRecordClientRepo = userAnswerRecordClientRepo;
            _fileRepo = fileRepo;
            _redisCachingProvider = redisCachingProvider;
        }

        [BindProperty]
        public long Urid { get; set; }

        [BindProperty]
        public int Force { get; set; }

        [BindProperty]
        public Guid ExamId { get; set; }

        [BindProperty]
        public double Score { get; set; }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string ExamTitle { get; set; }

        [BindProperty]
        public string ReportNumber { get; set; }

        [BindProperty]
        public string IdCard { get; set; }

        [BindProperty]
        public string Mobile { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public PaperOpenResult OpenResult { get; set; }

        [BindProperty]
        public ExamMarked Marked { get; set; }

        [BindProperty]
        public ExamComplated Complated { get; set; }

        [BindProperty]
        public ExamType ExamType { get; set; }


        public async Task<IActionResult> OnGet(long urid, int force = 0)
        {
            if (!await _userAnswerRecordClientRepo.getAnyAsync(u => u.Id == urid && u.IsDeleted == 0))
            {
                return Redirect("/Error?msg=" + Assistant.Utils.EncodeUrlParam("�����¼�����ڻ���ʧЧ"));
            }
            var record = await _userAnswerRecordClientRepo.Marking(urid, force == 1);

            Urid = urid;
            Force = force;
            ExamId = record.ExamId;
            ExamTitle = record.ExamTitle;
            ReportNumber = record.ReportNumber;
            IdCard = record.SecurityIdCard;
            Email = record.Email;
            Mobile = record.Mobile;
            Name = record.Name;
            Score = record.Score;
            OpenResult = (PaperOpenResult)record.OpenResult;
            Marked = (ExamMarked)record.Marked;
            ExamType = (ExamType)record.ExamType;
            Complated = (ExamComplated)record.Complated;
            await _redisCachingProvider.HDelAsync("UserExamLog", new List<string>() { record.ReportId });
            await _redisCachingProvider.KeyDelAsync("userRecord_" + record.Id);
            await _redisCachingProvider.KeyDelAsync("myReportExamHistories_" + record.ReportId);
            await _redisCachingProvider.KeyDelAsync("myAccountExamHistories_" + record.AccountId);
            await _redisCachingProvider.KeyDelAsync("userPaper_" + record.Id);
            return Page();
        }
    }
}
