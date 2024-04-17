using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Yitter.IdGenerator;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class ReportController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IReportInfoRepo _reportInfoRepo;
        private readonly IUserBaseRepo _userBaseRepo;
        private readonly IActivityRepo _activityRepo;
        private readonly ICapPublisher _capPublisher;
        private readonly IRedisCachingProvider _redisProvider;
        private readonly IUnitInfoRepo _unitInfoRepo;
        public ReportController(IResponseHelper resp, IReportInfoRepo reportInfoRepo,IUserBaseRepo userBaseRepo, IActivityRepo activityRepo, ICapPublisher capPublisher, IRedisCachingProvider redisProvider, IUnitInfoRepo unitInfoRepo)
        {
            _resp = resp;
            _reportInfoRepo = reportInfoRepo;
            _userBaseRepo = userBaseRepo;
            _activityRepo = activityRepo;
            _capPublisher = capPublisher;
            _redisProvider = redisProvider;
            _unitInfoRepo = unitInfoRepo;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportInfo(ReportInfoDto dto)
        {
            if (!await _userBaseRepo.getAnyAsync(u => u.AccountId == dto.AccountId))
            {
                return Json(_resp.error("用户不存在"));
            }

            if (!await _activityRepo.getAnyAsync(a => a.Id == dto.ActivityId))
            {
                return Json(_resp.error("活动不存在"));
            }

            //证件号作为唯一报名活动的重复性判定元素
            if (await _reportInfoRepo.getAnyAsync(r => r.IdCard == dto.IdCard && (r.ActivityId == dto.ActivityId || r.ExamId == dto.ExamId)))
            {
                return Json(_resp.error("您已经报名该活动了"));
            }
            dto.Id = YitIdHelper.NextId();
            //var reportInfo = dto.Adapt<ReportInfo>();
            dto.OrderTradeNumber = $"{dto.ExamId.ToString().Substring(19, 4).ToUpper()}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{Assistant.Utils.GenerateRandomCodePro(15)}";
            if (await _reportInfoRepo.ReportActivity(dto))
                return Json(_resp.success(new { tradeNo= dto.OrderTradeNumber ,reportId = dto.Id}));
            return Json(_resp.error("报名失败"));
        }

        [ResponseCache(Duration = 100,VaryByQueryKeys = new string[] { "examId", "idCard","rd" } )]
        public async Task<IActionResult> CheckReportLogByIdCard(Guid examId, string idCard)
        {
            if (await _reportInfoRepo.getAnyAsync(r => r.IdCard == idCard && r.ExamId == examId))
            {
                return Json(_resp.error("您已经报名该考试了"));
            }

            return Json(_resp.success("ok"));
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "examId", "idCard", "rd" })]
        public async Task<IActionResult> CheckReportLogById(Guid examId, string accountId)
        {
            if (await _reportInfoRepo.getAnyAsync(r => r.AccountId == accountId && r.ExamId == examId))
            {
                return Json(_resp.error("您账号下已经报名该考试了"));
            }

            return Json(_resp.success("ok"));
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "keyword","unitType", "cityId", "provinceId", "districtId", "rd" })]
        public async Task<IActionResult> GetUnitInfoDrops(string keyword, int unitType, int provinceId = 0, int cityId = 0, int districtId=0)
        {
            return Json(_resp.success(await _unitInfoRepo.GetUnitDropsAsync(keyword,unitType, provinceId, cityId,  districtId, 500)));
        }

        public async Task<IActionResult> GetReportOrderList(GetReportListDto dto)
        {            
            return Json(_resp.success(await _reportInfoRepo.GetReportOrderListClient(dto)));
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "outTradeNo", "rd" })]
        public async Task<IActionResult> GetReportDetailByOutTradeNoForClient(string outTradeNo)
        {
            return Json(_resp.success(await _reportInfoRepo.GetReportDetailByOutTradeNoForClient(outTradeNo)));
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "reportId", "rd" })]
        public async Task<IActionResult> GetReportDetailForClient(long reportId)
        {
            return Json(_resp.success(await _reportInfoRepo.GetReportDetailForClient(reportId)));
        }
    }
}
