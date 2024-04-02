using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

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

    }
}
