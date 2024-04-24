using EasyCaching.Core;
using Essensoft.Paylink.Alipay.Domain;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Cert.Controllers
{
    [Area("Cert")]
    public class CertTemplateController : Controller
    {
        private readonly ICertTemplateRepo _certTemplateRepo;
        private readonly IResponseHelper _resp;
        private readonly IRedisCachingProvider _redisCachingProvider;

        public CertTemplateController(ICertTemplateRepo certTemplateRepo, IResponseHelper resp, IRedisCachingProvider redisCachingProvider)
        {
            _certTemplateRepo = certTemplateRepo;
            _resp = resp;
            _redisCachingProvider = redisCachingProvider;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            var template = await _certTemplateRepo.getOneAsync(u => u.Id == id);
            return View(template);
        }
    }
}
