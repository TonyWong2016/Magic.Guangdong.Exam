using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Account;
using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;
using Magic.Guangdong.DbServices.Dtos.System.Admins;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Unicode;

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
        private readonly IRedisCachingProvider _redis;
        private string adminId = "";

        public ReportInfoController(IReportInfoRepo reportInfoRepo, 
            IReportCheckHistoryRepo reportHistoryRepo, 
            IResponseHelper resp, 
            INpoiExcelOperationService npoi, 
            IWebHostEnvironment en, 
            IHttpContextAccessor contextAccessor, 
            ICapPublisher capPublisher,
            IRedisCachingProvider redis)
        {
            _reportInfoRepo = reportInfoRepo;
            _reportHistoryRepo = reportHistoryRepo;
            _resp = resp;
            _npoi = npoi;
            _en = en;
            _contextAccessor = contextAccessor;
            _capPublisher = capPublisher;
            _redis = redis;
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

        [RouteMark("脱敏报名信息")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> MaskReportInfo()
        {
            if(!await _reportInfoRepo.getAnyAsync(u => u.IsSecurity == 0))
            {
                return Json(_resp.error("没有需要脱敏的数据"));
            }
            var count = await _reportInfoRepo.MaskReportInfoData();
            if (count > 0)
            {
                return Json(_resp.success(count, $"操作成功，共处理了{count}条记录"));
            }
            return Json(_resp.error("操作失败"));
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> DecryptMaskInfo([FromServices] IAdminRoleRepo adminRoleRepo, [FromServices] IRoleRepo roleRepo, string verifyCode,long reportId)
        {
            bool isSystemRole = false;
            if (string.IsNullOrEmpty(verifyCode))
            {
                var systemRoleIds = (await roleRepo.getListAsync(u => u.Type == 1)).Select(u => u.Id);
                Guid _adminId = Guid.Parse(adminId);
                isSystemRole = await adminRoleRepo.getAnyAsync(u => u.AdminId == _adminId && systemRoleIds.Contains(u.RoleId));
                if (!isSystemRole)
                {
                    return Json(_resp.error("验证码错误"));
                }
            }
            if(!await _redis.KeyExistsAsync("mask_"+verifyCode) && !isSystemRole)
            {
                return Json(_resp.error("验证码错误"));
            }
            var reportInfo = await _reportInfoRepo.getOneAsync(u => u.Id == reportId);
            if (reportInfo.IsSecurity == 0 && !string.IsNullOrEmpty(reportInfo.HashIdcard) && !string.IsNullOrEmpty(reportInfo.HashMobile))
            {
                return Json(_resp.ret(0, "信息没有脱敏", reportInfo.Adapt<ReportInfoDto>()));
            }
            string keyId = reportInfo.ReportNumber.Substring(0, 16);
            string keySecret = reportInfo.ReportNumber.Substring(reportInfo.ReportNumber.Length - 16, 16);
            string encyptIdCard = Assistant.Utils.FromBase64Str(reportInfo.IdCard);
            string encyptMobile = Assistant.Utils.FromBase64Str(reportInfo.Mobile);
            var newModel = reportInfo;
            newModel.IdCard = Assistant.Security.Decrypt(encyptIdCard, Encoding.UTF8.GetBytes(keyId), Encoding.UTF8.GetBytes(keySecret));
            newModel.Mobile = Assistant.Security.Decrypt(encyptMobile, Encoding.UTF8.GetBytes(keyId), Encoding.UTF8.GetBytes(keySecret));
            return Json(_resp.success(newModel.Adapt<ReportInfoDto>()));
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
