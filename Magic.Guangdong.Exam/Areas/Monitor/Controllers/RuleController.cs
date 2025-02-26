using EasyCaching.Core;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Monitor;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Monitor.Controllers
{
    [Area("Monitor")]
    public class RuleController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IMonitorRuleRepo _monitorRuleRepo;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IHttpContextAccessor _contextAccessor;

        private string adminId = "system";
        public RuleController(IResponseHelper resp, IMonitorRuleRepo monitorRuleRepo, IRedisCachingProvider redisCachingProvider, IHttpContextAccessor contextAccessor)
        {
            _resp = resp;
            _monitorRuleRepo = monitorRuleRepo;
            _redisCachingProvider = redisCachingProvider;
            _contextAccessor = contextAccessor;
            adminId = (_contextAccessor.HttpContext != null && _contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").Any()) ?
              Assistant.Utils.FromBase64Str(_contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").First().Value) : "system";

        }

        [RouteMark("监控规则管理")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取规则列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult GetRuleList(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _monitorRuleRepo.getList(dto, out total), total }));
        }

        /// <summary>
        /// 获取规则下拉列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "rd" })]
        public async Task<IActionResult> GetRuleMini()
        {
            return Json(_resp.success(await _monitorRuleRepo.GetRuleMini()));
        }

        [RouteMark("创建监控规则")]
        public IActionResult Create()
        {
            return View(new MainMonitorDto());
        }

        /// <summary>
        /// 创建考试
        /// </summary>
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(M)
    }
}
