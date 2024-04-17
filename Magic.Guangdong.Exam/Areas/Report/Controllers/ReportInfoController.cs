using DotNetCore.CAP;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;
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
        private readonly IReportCheckHistoryRepo _reportHistoryRepo;
        private readonly IResponseHelper _resp;
        private readonly INpoiExcelOperationService _npoi;
        private readonly IWebHostEnvironment _en;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICapPublisher _capPublisher;
        private string adminId = "";

        public ReportInfoController(IReportInfoRepo reportInfoRepo, IReportCheckHistoryRepo reportHistoryRepo, IResponseHelper resp, INpoiExcelOperationService npoi, IWebHostEnvironment en, IHttpContextAccessor contextAccessor, ICapPublisher capPublisher)
        {
            _reportInfoRepo = reportInfoRepo;
            _reportHistoryRepo = reportHistoryRepo;
            _resp = resp;
            _npoi = npoi;
            _en = en;
            _contextAccessor = contextAccessor;
            _capPublisher = capPublisher;
            adminId = (_contextAccessor!=null && _contextAccessor.HttpContext != null && _contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").Any()) ?
               Assistant.Utils.FromBase64Str(_contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").First().Value) : "system";
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

        [RouteMark("审核报名信息")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckReportInfo(long reportId,int checkResult,string checkRemark)
        {
            if(!await _reportInfoRepo.getAnyAsync(u => u.Id == reportId))
            {
                return Json(_resp.error("报名信息不存在"));
            }


            if (!Enum.IsDefined(typeof(CheckStatus), checkResult))
            {
                return Json(_resp.error("审核结果不正确"));
            }
            var dto = new ReportCheckHistoryDto()
            {
                reportIds = new long[] { reportId },
                checkStatus = (CheckStatus)checkResult,
                adminId = adminId,
                checkRemark = checkRemark
            };
            if (await _reportInfoRepo.CheckReportInfo(dto))
            {
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "ReportNotice", dto.reportIds);
                return Json(_resp.success("审核成功"));
            }
            return Json(_resp.error("审核失败"));
            
        }

        /// <summary>
        /// 通知报名人员
        /// </summary>
        /// <param name="reportIds"></param>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "ReportNotice")]
        public async Task ReportNotice(long[] reportIds)
        {
            Assistant.Logger.Warning($"{DateTime.Now},开始发送通知,共计{reportIds.Length}条");
            await _reportHistoryRepo.NoticeReportResult(reportIds);
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
