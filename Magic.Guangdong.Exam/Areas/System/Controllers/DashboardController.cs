using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    public class DashboardController : Controller
    {
        private IResponseHelper _resp;
        private IReportExamViewRepo _reportExamViewrepo;
        public DashboardController(IResponseHelper resp, IReportExamViewRepo reportExamViewrepo)
        {
            _resp = resp;
            _reportExamViewrepo = reportExamViewrepo;
        }
        [RouteMark("数据看板")]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 100,VaryByQueryKeys = new string[] { "rd" },VaryByHeader = "Authorization")]
        public async Task<IActionResult> GetData()
        {
            var data = await _reportExamViewrepo.GetDashboardData();
            return Json(_resp.success(data));
        }
    }
}
