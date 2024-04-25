using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Net;

namespace Magic.Guangdong.Exam.Areas.Exam.Controllers
{
    /// <summary>
    /// 答题记录管理
    /// </summary>
    [Area("exam")]
    public class ManageUserRecordController : Controller
    {
        private readonly IPaperRepo _paperRepo;
        private readonly IResponseHelper _resp;
        private readonly IUserAnswerRecordViewRepo _userAnswerRecordViewRepo;
        private readonly IWebHostEnvironment _en;
        private readonly ICapPublisher _capBus;
        private readonly IRedisCachingProvider _provider;
        private readonly IUserBaseRepo _userRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private string adminId = "system";
        private static int isnotice = 0;
        public ManageUserRecordController(IPaperRepo paperRepo, IResponseHelper resp, IUserAnswerRecordViewRepo userAnswerRecordViewRepo, IWebHostEnvironment en, ICapPublisher capBus, IRedisCachingProvider provider, IUserBaseRepo userRepo, IHttpContextAccessor context)
        {
            _paperRepo = paperRepo;
            _resp = resp;
            _contextAccessor = context;
            _capBus = capBus;
            _provider = provider;
            _userRepo = userRepo;
            _en = en;
            _userAnswerRecordViewRepo = userAnswerRecordViewRepo;
            _userRepo = userRepo;
            adminId = (_contextAccessor.HttpContext != null && _contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").Any()) ?
               Assistant.Utils.FromBase64Str(_contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").First().Value) : "system";

        }
        [RouteMark("答题记录")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取用户答题列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult GetUserRecord(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _userAnswerRecordViewRepo.GetUserRecord(dto, out total), total }));
        }

        /// <summary>
        /// 导出答题记录
        /// </summary>
        /// <param name="npoi"></param>
        /// <param name="whereJsonStr"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        [RouteMark("导出答题记录")]
        public async Task<IActionResult> ExportUserRecord([FromServices] INpoiExcelOperationService npoi, string whereJsonStr)
        {
            var records = await _userAnswerRecordViewRepo.GetUserRecordForExport(whereJsonStr);
            return Json(await npoi.ExcelDataExportTemplate("学生答题记录", "学生答题记录", records, _en.WebRootPath));
        }

        /// <summary>
        /// 答题详情
        /// </summary>
        /// <param name="paperId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Detail(Guid paperId)
        {
            if (await _paperRepo.getAnyAsync(u => u.Id == paperId))
            {
                return View(await _paperRepo.PreviewPaper(paperId));
            }
            return Content("看啥呢");
        }

        /// <summary>
        /// 获取用户答案
        /// </summary>
        /// <param name="urid"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetRecordDetail(long urid)
        {
            return Json(_resp.success(await _userAnswerRecordViewRepo.GetUserAnswer(urid)));
        }

        /// <summary>
        /// 后台强制交卷
        /// </summary>
        /// <param name="urid"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceMarking(long urid)
        {
            var record = await _userAnswerRecordViewRepo.ForceMarking(urid);
            await _provider.HDelAsync("UserExamLog", new List<string>() { record.ReportId });
            await _provider.KeyDelAsync("userRecord_" + record.Id);
            await _provider.KeyDelAsync("myReportExamHistories_" + record.ReportId);
            await _provider.KeyDelAsync("myAccountExamHistories_" + record.AccountId);
            await _provider.KeyDelAsync("userPaper_" + record.Id);
            return Json(_resp.success(record));
        }

        /// <summary>
        /// 强制给分-指定考试下的所有考生
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceMarkingAll(Guid examId)
        {
            try
            {
                var notComplatedRecords = await _userAnswerRecordViewRepo.GetNotComplatedList(examId);
                if (!notComplatedRecords.Any())
                {
                    return Json(_resp.ret(0, "没有需要强制提交的记录，请确认当前考试是否未严格交卷类型"));
                }
                foreach (var item in notComplatedRecords)
                {
                    var record = await _userAnswerRecordViewRepo.ForceMarking(item);
                    await _provider.HDelAsync("UserExamLog", new List<string>() { record.ReportId });
                }
                return Json(_resp.success(true, "操作成功"));
            }
            catch (Exception ex)
            {
                return Json(_resp.ret(-1, $"操作失败,{ex.Message},{ex.StackTrace}"));
            }
        }

        /// <summary>
        /// 删除答题记录
        /// </summary>
        /// <param name="urid"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRecord(long urid, int notice)
        {
            adminId = HttpContext.User.Claims.First().Value;
            isnotice = notice;
            bool ret = await _userAnswerRecordViewRepo.RemoveUserRecord(urid, adminId);
            if (ret)
            {
                _capBus.Publish(CapConsts.PREFIX + "RemoveUserRecord", urid, adminId);
                return Json(_resp.success(1, "移除成功"));
            }
            else
                return Json(_resp.ret(-1, "操作失败"));
        }

        /// <summary>
        /// 删除答题记录
        /// </summary>
        /// <param name="urId"></param>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "RemoveUserRecord")]
        public async Task RemoveUserRecord(long urId)
        {
            try
            {
                var record = await _userAnswerRecordViewRepo.getOneAsync(u => u.Id == urId);
                Logger.Warning($"{DateTime.Now},开始移除用户【{record.Name}】的答题记录");
                //int applyId = 0;
               
                var userInfo = await _userRepo.getOneAsync(u => u.AccountId == record.AccountId);
                if (userInfo == null)
                    return;
                string content = $"<p>以下内容为系统自动发送，请勿直接回复！</p>" +
                    $"<p>您账号{WebUtility.UrlDecode(record.Name)}下的一条答题记录已被管理员删除</p>" +
                    $"<p>记录详情如下</p>" +
                    $"<p>考试信息：{record.ExamTitle}</p>" +
                    $"<p>答题人证件号码：{record.IdNumber}</p>" +
                    $"<p>原答题分数：{record.Score}</p>" +
                    $"<p>创建答题时间：{record.CreatedAt}</p>" +
                    $"<p>删除时间：{record.UpdatedAt}</p>" +
                    $"<br /><image src='https://manage.declare.htgjjl.com/images/emaillogo.png' />";

                string[] emailBCcs = new string[] { "wangteng@xxt.org.cn" };
                if (!string.IsNullOrEmpty(ConfigurationHelper.GetSectionValue("mailBcc")))
                {
                    emailBCcs = ConfigurationHelper.GetSectionValue("mailBcc").Split(',');
                }
                //await CipAssistant.EmailHelper.SendEmailProAsync(userInfo.Email, "答题记录被删除", content, null, new string[] { "wangteng@xxt.org.cn" });
                List<MailboxAddress> to = new List<MailboxAddress>();
                to.Add(new MailboxAddress(record.Name, record.Email));
                to.Add(new MailboxAddress("tony", "wangteng@xxt.org.cn"));
                if (isnotice == 1)
                    await EmailKitHelper.SendEMailAsync("答题记录被删除", content, to);
                

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Logger.Error(ex.Message + "/r/n" + ex.StackTrace);
            }
        }
    }
}
