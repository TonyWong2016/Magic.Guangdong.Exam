using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Report.Controllers
{
    [Area("Report")]
    public class ReportCheckHistoryController : Controller
    {
        private readonly IReportCheckHistoryRepo _reportCheckHistoryRepo;
        private readonly IResponseHelper _resp;

        public ReportCheckHistoryController(IReportCheckHistoryRepo reportCheckHistoryRepo, IResponseHelper responseHelper)
        {
            _reportCheckHistoryRepo = reportCheckHistoryRepo;
            _resp = responseHelper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 100,VaryByQueryKeys =new string[] {"reportId","rd"})]
        public async Task<IActionResult> GetCheckHistory(long reportId)
        {
            return Json(_resp.success(await _reportCheckHistoryRepo.GetReportCheckHistory(reportId)));
        }
    }
}
