using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Exam.Controllers
{
    [Area("Exam")]
    public class UserRecordController : Controller
    {
        private readonly IPaperRepo _paperRepo;
        private readonly IExaminationRepo _examinationRepo;
        private readonly IResponseHelper _resp;
        private readonly IUserAnswerRecordRepo _userAnswerRecordRepo;
        private readonly IUserBaseRepo _userBaseRepo;
        private readonly IActivityReportRepo _activityReportRepo;
        private readonly ICapPublisher _capBus;
        //IRedisDatabaseProvider _redisDatabaseProvider;
        private readonly IRedisCachingProvider _provider;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="paperRepo"></param>
        /// <param name="examinationRepo"></param>
        /// <param name="resp"></param>
        /// <param name="capBus"></param>
        public UserRecordController(IPaperRepo paperRepo, IExaminationRepo examinationRepo, IResponseHelper resp, IUserAnswerRecordRepo userAnswerRecordRepo, ICapPublisher capBus, IRedisCachingProvider provider, IUserBaseRepo userBaseRepo, IActivityReportRepo activityReportRepo)
        {
            _paperRepo = paperRepo;
            _examinationRepo = examinationRepo;
            _resp = resp;
            _userAnswerRecordRepo = userAnswerRecordRepo;
            _capBus = capBus;
            //_redisDatabaseProvider = redisDatabaseProvider;
            _provider = provider;
            _userBaseRepo = userBaseRepo;
            _activityReportRepo = activityReportRepo;
        }

        //[RouteMark("答题记录")]
        public IActionResult Index(string associationId)
        {
            return View();
        }

        /// <summary>
        /// 获取当前活动下的考试
        /// </summary>
        /// <param name="associationId"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 600, VaryByQueryKeys = new string[] { "associationId" })]
        public async Task<IActionResult> GetExaminations(string associationId)
        {
            return Json(_resp.success(await _userAnswerRecordRepo.GetExaminations(associationId)));
        }

        /// <summary>
        /// 1.验证信息
        /// </summary>
        /// <param name="idNumber"></param>
        /// <param name="associationId"></param>
        /// <returns></returns>
        //[ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "idNumber", "associationId", "rd" })]
        public async Task<IActionResult> InfoVerification(string idNumber, string reportId)
        {
            if (!await _userBaseRepo.getAnyAsync(u=>u.IdCard==idNumber))
            {
                return Json(_resp.error("用户信息不存在"));
            }
            var userInfo = await _userBaseRepo.getOneAsync(u => u.IdCard == idNumber);
            if(!await _activityReportRepo.getAnyAsync(u=>u.AccountId== userInfo.AccountId))
            {
                return Json(_resp.error("用户没有报名当前活动"));
            }
            //预防答题中多人进入
            if (await _provider.HExistsAsync("UserExamLog", reportId) && await _provider.HGetAsync("UserExamLog", reportId) != idNumber)
            {
                return Json(_resp.ret(-1, "当前赛队信息已被锁定，目前正有其他成员正代表该赛队答题。"));
            }

            if (await _userAnswerRecordRepo.getAnyAsync(u => u.IdNumber == idNumber && u.ReportId == reportId))
            {
                return Json(_resp.success(new { userInfo, record = await _userAnswerRecordRepo.getOneAsync(u => u.IdNumber == idNumber && u.ReportId == reportId) }));
            }
            return Json(_resp.success(new { userInfo }));
        }

        /// <summary>
        /// 2.抽题确定自己的试卷
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmMyPaper(UserPaperRecordDto dto)
        {
            //var ret = await _userAnswerRecordRepo.ConfirmMyPaper(dto);

            return Json(await _userAnswerRecordRepo.ConfirmMyPaper(dto));
        }


        /// <summary>
        /// 获取试题
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
            return Json(_resp.success(await _userAnswerRecordRepo.GetMyPaper(paperId)));
        }

        public async Task<IActionResult> Paper(long urid)
        {

            if (!Request.Headers.Any(u => u.Key == "Referer"))
            {
                //RedirectToAction("Index");
                return Redirect("/examination/index");
            }
            var refer = Request.Headers.Where(u => u.Key == "Referer").First();
            if (!refer.Value.ToString().ToLower().Contains("examination/index"))
            {
                return Redirect("/examination/index");
            }
            var record = await _userAnswerRecordRepo.getOneAsync(u => u.Id == urid);
            if (record.LimitedTime < DateTime.Now)
            {
                return Redirect($"/examination/Result?urid={record.Id}&force=1");
            }
            if (record == null || string.IsNullOrEmpty(record.ReportId) || string.IsNullOrEmpty(record.IdNumber))
            {
                return Content("试题尚未初始化，或者进行了非规范答题，请联系管理人员");
            }
            if (!await _provider.HExistsAsync("UserExamLog", record.ReportId))
            {
                await _provider.HSetAsync("UserExamLog", record.ReportId, record.IdNumber);
            }
            return View();
        }

        /// <summary>
        /// 一道道地提交答案（航天的活动用的不是这种方式）
        /// 这个用到了缓存的工厂方法，
        /// 暂时只能放到控制器里，所以各种逻辑堆叠的有点多。
        /// </summary>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitOneByOne(UserAnswerDto dto)
        {
            if (await _provider.KeyExistsAsync(dto.recordId.ToString()))
            {
                await _provider.RPushXAsync(dto.recordId.ToString(), dto.answerDto);

            }
            else if (await _userAnswerRecordRepo.getAnyAsync(u => u.Id == dto.recordId))
            {
                var list = new List<AnswerDto>
                {
                    dto.answerDto
                };
                await _provider.RPushAsync(dto.recordId.ToString(), list);
            }
            else
            {
                return Json(_resp.ret(-1, "未创建答题记录"));
            }


            if (dto.finished == 1)
            {
                List<AnswerDto> answerList = await _provider.LRangeAsync<AnswerDto>(dto.recordId.ToString(), 0, -1);
                List<SubmitAnswerDto> submitList = new List<SubmitAnswerDto>();
                foreach (var answer in answerList)
                {
                    if (submitList.Any(u => u.questionId == answer.questionId))
                    {
                        var submitAnswer = submitList.Where(u => u.questionId == answer.questionId).First();
                        submitAnswer.userAnswer = answer.userAnswer;
                    }
                    submitList.Add(new SubmitAnswerDto()
                    {
                        questionId = answer.questionId,
                        userAnswer = answer.userAnswer,
                    });
                }

                var record = await _userAnswerRecordRepo.getOneAsync(u => u.Id == dto.recordId);
                record.SubmitAnswer = JsonHelper.JsonSerialize(submitList);
                record.Complated = 1;
                record.ComplatedMode = record.ComplatedMode == 0 ? 1 : 0;
                record.UpdatedAt = DateTime.Now;
                record.Remark += $"[{record.IdNumber}],完成考试；";

                int ret = await _userAnswerRecordRepo.updateItemAsync(record);
                if (ret == 0)
                {
                    return Json(_resp.ret(-1, "提交失败，请及时联系管理人员进行信息确认"));
                }

                //删除提交的答案记录
                await _provider.KeyDelAsync(record.Id.ToString());
                //删除锁定的答题记录
                if (await _provider.HExistsAsync("UserExamLog", record.ReportId))
                {
                    await _provider.HDelAsync("UserExamLog"
                        , new List<string>() {
                            record.Id.ToString()
                        });
                }
            }
            return Json(_resp.success("提交成功"));
        }

        /// <summary>
        /// 提交一整张试卷
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitMyPaper(SubmitMyAnswerDto dto)
        {
            //int ret = await _userAnswerRecordRepo.SubmitMyPaper(dto);
            return Json(await _userAnswerRecordRepo.SubmitMyPaper(dto));
        }

        /// <summary>
        /// 结果
        /// </summary>
        /// <param name="urid"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public async Task<IActionResult> Result(long urid, int force = 0)
        {
            if (!await _userAnswerRecordRepo.getAnyAsync(u => u.Id == urid))
                return Redirect("/examination/index");

            ViewBag.AttachBase = ConfigurationHelper.GetSectionValue("attachBase");
            var record = await _userAnswerRecordRepo.Marking(urid, force == 1);
            return View(record);
        }



    }
}
