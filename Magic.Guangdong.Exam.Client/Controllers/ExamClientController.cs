using Coravel.Cache;
using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml.FormulaParsing.Utilities;
using System;
using System.Drawing.Printing;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class ExamClientController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IResponseHelper _resp;
        private readonly IUserAnswerRecordClientRepo _userAnswerRecordClientRepo;
        private readonly IExaminationClientRepo _examRepo;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IPaperRepo _paperRepo;
        private readonly ICapPublisher _capPublisher;
        public ExamClientController(IHttpContextAccessor contextAccessor, IResponseHelper resp, IExaminationClientRepo examinationRepo, IRedisCachingProvider redisCachingProvider, IUserAnswerRecordClientRepo userAnswerRecordClientRepo, IPaperRepo paperRepo, ICapPublisher capPublisher)
        {
            _contextAccessor = contextAccessor;
            _resp = resp;
            _examRepo = examinationRepo;
            _redisCachingProvider = redisCachingProvider;
            _userAnswerRecordClientRepo = userAnswerRecordClientRepo;
            _paperRepo = paperRepo;
            _capPublisher = capPublisher;
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
            else if (result.StartsWith("已参加过考试"))
                return Json(_resp.ret(1, "已参加过考试", result.Split("|")[1]));
            return Json(_resp.error(result));
        }


        public async Task<IActionResult> GetReportExamsForClient(ReportExamDto dto)
        {            
            var items = await _examRepo.GetReportExamsForClient(dto);
            if (items.Count == 0)
            {
                return Json(_resp.error("没有报名的考试记录"));
            }
            return Json(_resp.success(items));
        }

        /// <summary>
        /// 抽卷
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IActionResult> ConfirmMyPaper(ConfirmPaperDto dto)
        {
            return Json(_resp.success(await _userAnswerRecordClientRepo.ConfirmMyPaper(dto)));
        }

        /// <summary>
        /// 抽练习题
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IActionResult> ConfirmMyPracticePaper(ConfirmPaperDto dto)
        {
            return Json(await _userAnswerRecordClientRepo.ConfirmMyPracticePaper(dto));
        }

        /// <summary>
        /// 获取我的试卷
        /// </summary>
        /// <param name="paperId"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> GetMyPaper(Guid paperId)
        {
            if (!await _paperRepo.getAnyAsync(u => u.Id == paperId && u.IsDeleted == 0))
            {
                return Json(-1, "试卷不存在或已被删除，请联系管理人员");
            }
            if (!await _redisCachingProvider.KeyExistsAsync(paperId.ToString()))
            {
                var paper = await _userAnswerRecordClientRepo.GetMyPaper(paperId);
                await _redisCachingProvider.StringSetAsync(paperId.ToString(),
                JsonHelper.JsonSerialize(paper),
                DateTime.Now.AddMinutes(paper.Duration) - DateTime.Now);
            }
            //return Json(
            //       _resp.success(
            //           JsonHelper
            //           .JsonDeserialize<FinalPaperDto>(
            //               await _redisCachingProvider
            //               .StringGetAsync(paperId.ToString()
            //               ))));
            return Json(
                   _resp.success(
                       await _redisCachingProvider
                           .StringGetAsync(paperId.ToString())));
        }

        /// <summary>
        /// 提交一整张试卷
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitMyPaper(SubmitMyAnswerDto dto)
        {
            if (!string.IsNullOrEmpty(dto.submitAnswerStr) && dto.submitAnswerStr == "null")
                dto.submitAnswerStr = "";

            if (dto.complatedMode != (int)ExamComplatedMode.Auto)
            {
                await _redisCachingProvider.HDelAsync("UserExamLog", new List<string>() { dto.reportId.ToString() });
                await _redisCachingProvider.KeyDelAsync("userRecord_" + dto.recordId);
                await _redisCachingProvider.KeyDelAsync("myReportExamHistories_" + dto.reportId);
                await _redisCachingProvider.KeyDelAsync("myAccountExamHistories_" + dto.userId);
                await _redisCachingProvider.KeyDelAsync("userPaper_" + dto.recordId);
                //这样写实非我愿啊/(ㄒoㄒ)/~~~
                if (dto.isPractice == 0)
                    return Json(await _userAnswerRecordClientRepo.SubmitMyPaper(dto));
                return Json(await _userAnswerRecordClientRepo.SubmitMyPracticePaper(dto));
            }

            Console.WriteLine($"{DateTime.Now}:发布事务--保存答案");
            await _capPublisher.PublishAsync(CapConsts.PREFIX + "SubmitMyAnswer", dto);
            if (!await _redisCachingProvider.KeyExistsAsync(dto.recordId.ToString()))
            {
                var record = await _userAnswerRecordClientRepo.GetMyRecord(dto.recordId);
                await _redisCachingProvider.StringSetAsync(dto.recordId.ToString(),
                    JsonHelper.JsonSerialize(record),
                    DateTime.Now.AddMinutes(1) - DateTime.Now);
            }

            return Json(_resp.success(
                    await _redisCachingProvider.StringGetAsync(dto.recordId.ToString())));
        }


        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "SubmitMyAnswer")]
        public async Task SubmitMyAnswer(SubmitMyAnswerDto dto)
        {
            Console.WriteLine($"{DateTime.Now}:消费事务---提交答案");
            await _userAnswerRecordClientRepo.SubmitMyPaper(dto);
        }

        public async Task<IActionResult> GetMyAnswer(long urid)
        {
            return Json(_resp.success(await _userAnswerRecordClientRepo.GetUserAnswer(urid)));
        }
    }
}
