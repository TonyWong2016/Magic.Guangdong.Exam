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
        public ReportController(IResponseHelper resp, IReportInfoRepo reportInfoRepo,IUserBaseRepo userBaseRepo, IActivityRepo activityRepo, ICapPublisher capPublisher, IRedisCachingProvider redisProvider)
        {
            _resp = resp;
            _reportInfoRepo = reportInfoRepo;
            _userBaseRepo = userBaseRepo;
            _activityRepo = activityRepo;
            _capPublisher = capPublisher;
            _redisProvider = redisProvider;
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
            if (await _reportInfoRepo.getAnyAsync(r => r.IdCard == dto.IdCard && r.ActivityId == dto.ActivityId))
            {
                return Json(_resp.error("您已经报名该活动了"));
            }

            var reportInfo = dto.Adapt<ReportInfo>();

            return Json(_resp.success(await _reportInfoRepo.addItemAsync(reportInfo)));
        }
    }
}
