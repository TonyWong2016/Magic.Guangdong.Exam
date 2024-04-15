using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Report.Controllers
{
    [Area("Report")]
    public class ReportInfoController : Controller
    {
        private readonly IReportInfoRepo _reportInfoRepo;
        private readonly IResponseHelper _resp;
        private readonly INpoiExcelOperationService _npoi;
        private readonly IWebHostEnvironment _en;
        public ReportInfoController(IReportInfoRepo reportInfoRepo, IResponseHelper resp, INpoiExcelOperationService npoi, IWebHostEnvironment en)
        {
            _reportInfoRepo = reportInfoRepo;
            _resp = resp;
            _npoi = npoi;
            _en = en;
        }

        [RouteMark("报名管理")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetReportInfos(PageDto dto)
        {
            long total;
            return Json(_resp.success(new {items= _reportInfoRepo.GetReportInfos(dto, out total),total}));
        }

        [RouteMark("导出报名信息")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportReportInfos(string whereJsonStr, string adminId)
        {
            var list = await _reportInfoRepo.GetReportInfosForExcel(whereJsonStr);
            if(list.Count() == 0)
            {
                return Json(_resp.error("没有数据"));
            }
            if (list.Count() > 1000)
            {
                return Json(await _npoi.SubmitExcelTask("报名信息", whereJsonStr, adminId));
            }

            return Json(await _npoi.ExcelDataExportTemplate("报名信息", "报名信息表", list, _en.WebRootPath));
        }

        /// <summary>
        /// 提交导出任务
        /// </summary>
        /// <param name="whereJsonStr"></param>
        /// <param name="adminId"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> submitExportTask(string whereJsonStr, string adminId, string title)
        {
            return Json(await _npoi.SubmitExcelTask(title, whereJsonStr, adminId));
        }
    }
}
