using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Cert;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NPOI.XSSF.UserModel;
using RabbitMQ.Client;
using static FreeSql.Internal.GlobalFilter;

namespace Magic.Guangdong.Exam.Areas.Cert.Controllers
{
    [Area("Cert")]
    public class CertController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ICertRepo _certRepo;
        private readonly ICertTemplateRepo _certTemplateRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ISixLaborHelper _sixLaborHelper;
        private readonly IWebHostEnvironment _en;
        private readonly IRedisCachingProvider _redisCachingProvider;

        public CertController(IResponseHelper resp, ICertRepo certRepo, ICertTemplateRepo certTemplateRepo, IHttpContextAccessor contextAccessor, ISixLaborHelper sixLaborHelper, IWebHostEnvironment en,IRedisCachingProvider redisCachingProvider)
        {
            _resp = resp;
            _certRepo = certRepo;
            _certTemplateRepo = certTemplateRepo;
            _contextAccessor = contextAccessor;
            _sixLaborHelper = sixLaborHelper;
            _en = en;
            _redisCachingProvider = redisCachingProvider;
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

        /// <summary>
        /// 获取证书列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 100,VaryByQueryKeys = new string[] { "whereJsonStr", "rd", "isAsc", "orderby" })]
        public IActionResult GetCerts(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _certRepo.getList(dto, out total), total }));
        }

        public IActionResult Import()
        {
            return View();
        }

        public async Task<IActionResult> ImportExcelData(ImportDto model)
        {
            string key = $"importList-{Security.GenerateMD5Hash(model.Path)}";
            List<ImportTemplateDto> importList;
            if (await _redisCachingProvider.KeyExistsAsync(key))
            {
                importList = JsonHelper.JsonDeserialize<List<ImportTemplateDto>>(await _redisCachingProvider.StringGetAsync(key))
            } else
                importList = await ExcelHelper<ImportTemplateDto>.GetImportData(model.Path);
            int importTotal = importList.Count;
            if (importTotal > model.CertNumLength)
            {
                return Json(_resp.error($"导入的条数为{importTotal}条，高余设定的导入编号上限"));
            }
            var idNumbers = importList.Select(u => u.IdNumber).ToList();
            if (await _certRepo.getAnyAsync(u=> idNumbers.Contains(u.IdNumber) && u.IsDeleted==0))
            {
                return Json(_resp.error($"导入的数据中有重复的id号码"));
            }
            List<string> certNums = new List<string>();
            for (int i = 1; i <= importTotal; i++) {
                string item = i.ToString();
                certNums.Add(model.CertNumPrefix + (i < model.CertNumLength ? new string('0', model.CertNumLength - i) + item : item.Substring(0, model.CertNumLength)));
            }
            if (model.IsOverwrite == 0 && await _certRepo.getAnyAsync(u => certNums.Contains(u.CertNo)))
            {
                
                await _redisCachingProvider.StringSetAsync(key, JsonHelper.JsonSerialize(importList), TimeSpan.FromMinutes(2));
                return Json(_resp.ret(2, "部分编号记录已存在，是否要覆盖原数据"));
            }
        }
    }
}
