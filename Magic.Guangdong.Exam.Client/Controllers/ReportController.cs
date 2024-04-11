using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportInfo(ReportInfoDto dto)
        {
            if(!await _userBaseRepo.getAnyAsync(u => u.AccountId == dto.AccountId))
            {
                return Json(_resp.error("用户不存在"));
            }

            if(!await _activityRepo.getAnyAsync(a => a.Id == dto.ActivityId))
            {
                return Json(_resp.error("活动不存在"));
            }

            //证件号作为唯一报名活动的重复性判定元素
            if (await _reportInfoRepo.getAnyAsync(r => r.IdCard == dto.IdCard && (r.ActivityId == dto.ActivityId||r.ExamId==dto.ExamId)))
            {
                return Json(_resp.error("您已经报名该活动了"));
            }

            var reportInfo = dto.Adapt<ReportInfo>();

            return Json(_resp.success(await _reportInfoRepo.addItemAsync(reportInfo)));
        }

        [ResponseCache(Duration = 100,VaryByQueryKeys = new string[] { "activityId","idCard","rd" } )]
        public async Task<IActionResult> CheckReportLog(long activityId, string idCard)
        {
            if (await _reportInfoRepo.getAnyAsync(r => r.IdCard == idCard && r.ActivityId == activityId))
            {
                return Json(_resp.error("您已经报名该活动了"));
            }

            return Json(_resp.success("ok"));
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "keyword", "cityId", "provinceId", "districtId", "rd" })]
        public async Task<IActionResult> GetUnitInfoDrops(string keyword, int provinceId = 0, int cityId = 0, int districtId=0)
        {
            return Json(_resp.success(await _unitInfoRepo.GetUnitDropsAsync(keyword, provinceId, cityId,  districtId, 500)));
        }
    }
}
