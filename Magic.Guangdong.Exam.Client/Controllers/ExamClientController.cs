using EasyCaching.Core;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class ExamClientController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IResponseHelper _resp;
        private readonly IUserAnswerRecordClientRepo _userAnswerRecordClientRepo;
        private readonly IExaminationClientRepo _examRepo;
        private readonly IRedisCachingProvider _redisCachingProvider;

        public ExamClientController(IHttpContextAccessor contextAccessor, IResponseHelper resp, IExaminationClientRepo examinationRepo, IRedisCachingProvider redisCachingProvider, IUserAnswerRecordClientRepo userAnswerRecordClientRepo)
        {
            _contextAccessor = contextAccessor;
            _resp = resp;
            _examRepo = examinationRepo;
            _redisCachingProvider = redisCachingProvider;
            _userAnswerRecordClientRepo = userAnswerRecordClientRepo;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> InfoVerificationAuto(ReportExamDto dto)
        {
            var result = await _examRepo.InfoVerificationAuto(dto);
            if (result == "ok")
                return Json(_resp.success(result));
            return Json(_resp.error(result));
        }

        public async Task<IActionResult> GetReportExamsForClient(ReportExamDto dto)
        {
            var items = await _examRepo.GetReportExamsForClient(dto);
            if(items.Count == 0)
            {
                return Json(_resp.error("没有报名的考试记录"));
            }
            return Json(_resp.success(items));
        }

        public async Task<IActionResult> ConfirmMyPaper(ConfirmPaperDto dto)
        {
            return null;
        }
    }
}
