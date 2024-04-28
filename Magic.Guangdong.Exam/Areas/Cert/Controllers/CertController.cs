using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Cert;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Cert.Controllers
{
    public class CertController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ICertRepo _certRepo;
        private readonly ICertTemplateRepo _certTemplateRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ISixLaborHelper _sixLaborHelper;
        private readonly IWebHostEnvironment _en;

        public CertController(IResponseHelper resp, ICertRepo certRepo, ICertTemplateRepo certTemplateRepo, IHttpContextAccessor contextAccessor, ISixLaborHelper sixLaborHelper, IWebHostEnvironment en)
        {
            _resp = resp;
            _certRepo = certRepo;
            _certTemplateRepo = certTemplateRepo;
            _contextAccessor = contextAccessor;
            _sixLaborHelper = sixLaborHelper;
            _en = en;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 生成证书模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public async Task<IActionResult> GenerationImportTemplate(string templateName = "证书导入模板")
        {
            return Json(_resp.success(await ExcelHelper<ImportTemplateDto>.GenerateTemplate(templateName)));
        }
    }
}
