using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        #region 主动提供reportid的验证相关接口，需要先登录
        /// <summary>
        /// 获取报名的考试记录
        /// 注意，该接口的前提是，先登录，且已经报名了
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetReportExamsForClient(ReportExamDto dto)
        {            
            var items = await _examRepo.GetReportExamsForClient(dto);
            if (items == null || items.Count == 0)
            {
                return Json(_resp.error("没有报名的考试记录"));
            }
            return Json(_resp.success(items));
        }
        
        /// <summary>
        /// 验证报考信息
        /// 注意，这个是主动验证，搭配的接口是GetReportExamsForClient，即需要提供reportid
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IActionResult> InfoVerificationAuto(ReportExamDto dto)
        {
            var result = await _examRepo.InfoVerificationAuto(dto);
            if (!await _redisCachingProvider.HExistsAsync("clientsigns", dto.reportId.ToString())
                      && _contextAccessor.HttpContext != null
                      && _contextAccessor.HttpContext.Request.Cookies.Any(u => u.Key == "clientsign")) { }
            {
                string sign = _contextAccessor.HttpContext.Request.Cookies["clientsign"];
                await _redisCachingProvider.HSetAsync("clientsigns", sign, dto.reportId.ToString());
            }
            if (result == "ok")
                return Json(_resp.success(result));
            else if (result.StartsWith("已参加过考试"))
                return Json(_resp.ret(1, "已参加过考试", result.Split("|")[1]));
            return Json(_resp.error(result));
        }

        #endregion


        #region 不主动提供reportid相关接口，不需要先登录
        /// <summary>
        /// 获取考试列表
        /// 这个就是单纯的获取考试列表，和报名无关
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetExamsForClient(OnlyGetExamDto dto)
        {
            var items = await _examRepo.GetExamsForClient(dto);
            if (items == null || items.Count == 0)
            {
                return Json(_resp.error("没有可用的考试记录"));
            }
            return Json(_resp.success(items));
        }

        /// <summary>
        /// 验证考试信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        //[ResponseCache(Duration = 100,VaryByQueryKeys =new string[] { "examId", "groupCode", "examType", "hashIdcard", "reportNumber" })]
        public async Task<IActionResult> InfoVerificationByNumber(OnlyGetExamDto dto)
        {
            try
            {
                var result = await _examRepo.InfoVerificationByNumber(dto);
                if (result.verifyCode > -1
                      && !await _redisCachingProvider.HExistsAsync("clientsigns", result.reportId.ToString())
                      && _contextAccessor.HttpContext != null
                      && _contextAccessor.HttpContext.Request.Cookies.Any(u => u.Key == "clientsign")) { }
                {
                    string sign = _contextAccessor.HttpContext.Request.Cookies["clientsign"];
                    await _redisCachingProvider.HSetAsync("clientsigns", result.reportId.ToString(),sign);
                }
                return Json(_resp.ret(result.verifyCode, result.verifyMsg, result));
            }
            catch (Exception ex) 
            {
                Assistant.Logger.Error(ex);
                return Json(_resp.error("认证服务异常"));
            }
           
        }
        #endregion

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
                #region 这里还可以加一个判定，验证用户的sign是否和redis里保存的一致不一致就把它踢走或者标记到服务端，有作弊嫌疑
                //if (_contextAccessor.HttpContext == null
                //         || !_contextAccessor.HttpContext.Request.Cookies.Any(u => u.Key == "clientsign"))
                //{
                //    return Json(_resp.error("交卷失败，令牌丢失"));
                //}
                //string sign = _contextAccessor.HttpContext.Request.Cookies["clientsign"];
                //if (await _redisCachingProvider.HGetAsync("clientsigns", dto.reportId.ToString()) != sign)
                //{
                //    return Json(_resp.error("交卷失败，令牌不一致，请确保使用首次验证的设备进行答题"));
                //}
                #endregion
                //这样写实非我愿啊/(ㄒoㄒ)/~~~
                if (dto.isPractice == 0)
                    return Json(await _userAnswerRecordClientRepo.SubmitMyPaper(dto));
                return Json(await _userAnswerRecordClientRepo.SubmitMyPracticePaper(dto));
            }
            Assistant.Logger.Warning($"{DateTime.Now}:发布事务--保存答案");
            if (string.IsNullOrEmpty(dto.submitAnswerStr))
                dto.submitAnswerStr = "[]";
            if (dto.submitAnswerStr.Length > CapConsts.CapMsgMaxLength)
            {
                Assistant.Logger.Warning($"{DateTime.Now}:发布事务--间接保存答案");
                await _redisCachingProvider.StringSetAsync("longAnswer" + dto.recordId, dto.submitAnswerStr, TimeSpan.FromSeconds(60));
                dto.submitAnswerStr = "";               
            }
            await _capPublisher.PublishAsync(CapConsts.ClientPrefix + "SubmitMyAnswer", dto);

            await Task.Run(async () =>
            {
                if (!await _redisCachingProvider.KeyExistsAsync(dto.recordId.ToString()))
                {
                    var record = await _userAnswerRecordClientRepo.GetMyRecord(dto.recordId);
                    await _redisCachingProvider.StringSetAsync(dto.recordId.ToString(),
                        JsonHelper.JsonSerialize(record),
                        DateTime.Now.AddSeconds(90) - DateTime.Now);
                }
            });
            

            return Json(_resp.success(
                    await _redisCachingProvider.StringGetAsync(dto.recordId.ToString())));
        }


        [NonAction]
        [CapSubscribe(CapConsts.ClientPrefix + "SubmitMyAnswer")]
        public async Task SubmitMyAnswer(SubmitMyAnswerDto dto, [FromCap] CapHeader header)
        {
            Logger.Warning($"消费事务---保存答案");
            string msgId = header["cap-msg-id"]??"";
            if (!string.IsNullOrEmpty(msgId) && await _redisCachingProvider.HExistsAsync(CapConsts.MsgIdCacheClientName, msgId))
            {
                Logger.Verbose("已消费");
                return;
            }
            //Logger.Warning(System.Text.Json.JsonSerializer.Serialize(header));
            await Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(dto.submitAnswerStr))
                {
                    if (await _redisCachingProvider.KeyExistsAsync("longAnswer" + dto.recordId))
                    {
                        dto.submitAnswerStr = await _redisCachingProvider.StringGetAsync("longAnswer" + dto.recordId);
                        //不去手动移除，交给redis自动过期
                       // await _redisCachingProvider.KeyDelAsync("longAnswer" + dto.recordId);
                    }
                    else if (await _redisCachingProvider.KeyExistsAsync(dto.recordId.ToString()))
                    {
                        var record = await _redisCachingProvider.StringGetAsync(dto.recordId.ToString());
                        dto.submitAnswerStr = JsonHelper.JsonDeserialize<UserAnswerRecordView>(record).SubmitAnswer;
                    }
                }
                dto.msgId = header["cap-msg-id"] ?? "";
                dto.instance = header["cap-exec-instance-id"] ?? "";
                dto.senttime = header["cap-senttime"] ?? "";
                await _userAnswerRecordClientRepo.SubmitMyPaper(dto);
            });
        }

        public async Task<IActionResult> GetMyAnswer(long urid)
        {
            return Json(_resp.success(await _userAnswerRecordClientRepo.GetUserAnswer(urid)));
        }
    }
}
