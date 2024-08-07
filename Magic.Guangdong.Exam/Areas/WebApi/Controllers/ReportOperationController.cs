using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.WebApi.Controllers
{
    [Area("webapi")]    
    public class ReportOperationController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICapPublisher _capPublisher;
        public ReportOperationController(IResponseHelper resp, IRedisCachingProvider redisCachingProvider, IWebHostEnvironment webHostEnvironment, ICapPublisher capPublisher)
        {
            _resp = resp;
            _redisCachingProvider = redisCachingProvider;
            _webHostEnvironment = webHostEnvironment;
            _capPublisher = capPublisher;
        }

        [HttpPost]
        [WebApiModule]
        public async Task<IActionResult> Test()
        {
            return Json(_resp.success(Guid.NewGuid()));
        }
    }
}
