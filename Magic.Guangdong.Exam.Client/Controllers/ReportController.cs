﻿using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Yitter.IdGenerator;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class ReportController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IReportInfoRepo _reportInfoRepo;
        private readonly IUserBaseRepo _userBaseRepo;
        private readonly IReportProcessRepo _reportProcessRepo;
        private readonly IExaminationClientRepo _examinationClientRepo;
        private readonly IActivityRepo _activityRepo;
        private readonly ICapPublisher _capPublisher;
        private readonly IRedisCachingProvider _redisProvider;
        private readonly IUnitInfoRepo _unitInfoRepo;
        private readonly IReportExamViewRepo _reportExamViewRepo;
        public ReportController(IResponseHelper resp,
            IWebHostEnvironment webHostEnvironment,
            IExaminationClientRepo examinationClientRepo,
            IReportInfoRepo reportInfoRepo,
            IUserBaseRepo userBaseRepo, 
            IActivityRepo activityRepo,
            ICapPublisher capPublisher,
            IRedisCachingProvider redisProvider, 
            IUnitInfoRepo unitInfoRepo,
            IReportProcessRepo reportProcessRepo,
            IReportExamViewRepo reportExamViewRepo)
        {
            _resp = resp;
            _reportInfoRepo = reportInfoRepo;
            _userBaseRepo = userBaseRepo;
            _activityRepo = activityRepo;
            _capPublisher = capPublisher;
            _redisProvider = redisProvider;
            _unitInfoRepo = unitInfoRepo;
            _examinationClientRepo = examinationClientRepo;
            _reportProcessRepo = reportProcessRepo;
            _webHostEnvironment = webHostEnvironment;
            _reportExamViewRepo = reportExamViewRepo;
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
            {
                await _capPublisher.PublishAsync(CapConsts.ClientPrefix + "CheckReportStatus", dto.Id);
                return Json(_resp.success(new { tradeNo = dto.OrderTradeNumber, reportId = dto.Id }));
            }
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

        public async Task<IActionResult> GetReportOrderListClient(GetReportListDto dto)
        {
            var ret = await _reportInfoRepo.GetReportOrderListClient(dto);
            
            await _capPublisher.PublishAsync(CapConsts.ClientPrefix + "SyncExamReportInfoToPractice", ret);
            return Json(_resp.success(ret));
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

        [NonAction]
        [CapSubscribe(CapConsts.ClientPrefix+ "SyncExamReportInfoToPractice")]
        public async Task SyncExamReportInfoToPractice(ReportOrderList list, [FromCap] CapHeader header)
        {
            string msgId = header["cap-msg-id"] ?? "";
            if (!string.IsNullOrEmpty(msgId) && await _redisProvider.HExistsAsync(CapConsts.MsgIdCacheClientName, msgId))
            {
                Assistant.Logger.Verbose("已消费");
                return;
            }
            //5分钟内只能操作一次
            if (list.total==0 || await _redisProvider.KeyExistsAsync("SyncExamReportInfoToPractice_" + list.items[0].AccountId))
            {
                return;
            }
            if (!list.items.Where(u=>u.ReportStatus==0 && u.Step == 1).Any())
            {
                return;
            }
            
            var examIds = list.items.Where(u => u.ReportStatus == 0 && u.Step == 1)
                .Select(u => u.ExamId)
                .ToList();
            Expression<Func<Examination, bool>> filter = u => examIds.Contains(u.AttachmentId)
            && u.Status == ExamStatus.Enabled
            && u.IsDeleted == 0
            && u.ExamType == ExamType.Practice;
            

            if (!await _examinationClientRepo.getAnyAsync(filter))
            {
                return;//没有符合条件的练习
            }
            //这是找出来的，所有当前账号报过名的考试，下属的练习
            //也就是，用户报名了考试，但这个考试有一个对应的练习，需要用户无需再次填写报名信息，直接就可以参加            
            var practices = await _examinationClientRepo.getListAsync(filter);
            foreach (var practice in practices)
            {
                if(!list.items.Where(u=>u.ExamId==practice.AttachmentId).Any())
                {
                    continue;
                }
                var item = list.items.Where(u => u.ExamId == practice.AttachmentId).FirstOrDefault();
                Expression<Func<ReportInfo, bool>> filterReport = u => u.Id == item.ReportId && u.ExamId == practice.AttachmentId;
                if (await _reportInfoRepo.getAnyAsync(filterReport))
                    continue;
                var reportInfo = await _reportInfoRepo.getOneAsync(filterReport);
                ReportInfoDto dto = new ReportInfoDto()
                {
                    Id = YitIdHelper.NextId(),
                    IdCard = reportInfo.IdCard,
                    AccountId = reportInfo.AccountId,
                    ActivityId = reportInfo.ActivityId,
                    Address = reportInfo.Address,
                    CardType = reportInfo.CardType,
                    CityId = reportInfo.CityId,
                    ConnAvailable = (int)reportInfo.ConnAvailable,
                    DistrictId = reportInfo.DistrictId,
                    Email = reportInfo.Email,
                    ExamId = practice.Id,//注意这个
                    Job = reportInfo.Job,
                    Mobile = reportInfo.Mobile,
                    Name = reportInfo.Name,
                    OrderTradeNumber = $"{practice.Id.ToString().Substring(19, 4).ToUpper()}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{Assistant.Utils.GenerateRandomCodePro(15)}",
                    OtherInfo = reportInfo.OtherInfo + "同步报名信息到其练习模式",
                    ProvinceId = reportInfo.ProvinceId,
                    UnitId = reportInfo.UnitId,
                };
                await _reportInfoRepo.ReportActivity(dto);
            }
            await _redisProvider.StringSetAsync("SyncExamReportInfoToPractice_" + list.items[0].AccountId, "done", DateTime.Now.AddMinutes(5) - DateTime.Now);
        }

        [NonAction]
        [CapSubscribe(CapConsts.ClientPrefix + "CheckReportStatus")]
        public async Task CheckReportStatus(long reportId)
        {
            var reportInfo = await _reportExamViewRepo.getOneAsync(u => u.ReportId == reportId);
            if (reportInfo == null || reportInfo.ReportStatus!=0 || reportInfo.ReportStep!=1)
            {
                Console.WriteLine("异常的报名活动");
                return;
            }
            
            if (reportInfo.ConnAvailable==1 || reportInfo.ConnAvailable==3)
            {
                string htmlContent;
                string templateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "web", "emailnotice.html");
                using (StreamReader reader = new StreamReader(templateFilePath))
                {
                    htmlContent = reader.ReadToEnd()
                        .Replace("**title**","报名成功")
                        .Replace("**content1**", $"您的已经成功报名考试【{reportInfo.Title}】，考试时间为【{reportInfo.StartTime}~{reportInfo.EndTime}】，准考证号为")
                        .Replace("**content2**", $"{reportInfo.ReportNumber}")
                        .Replace("**content3**", $"请牢记您的准考证号，准时参加考试");

                }
                await EmailKitHelper.SendVerificationCodeEmailAsync("报名成功", htmlContent, reportInfo.Email, reportInfo.Name);
                //await _emailSender.SendEmailAsync(reportInfo.Email,"报名成功",)
            }
        }
    }
}
