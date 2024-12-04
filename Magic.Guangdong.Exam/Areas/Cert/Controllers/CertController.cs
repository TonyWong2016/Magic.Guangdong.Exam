using DotNetCore.CAP;
using EasyCaching.Core;
using Essensoft.Paylink.Alipay.Domain;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Cert;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Channels;

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
        private readonly IHubContext<Tools.ConnectionHub> _myHub;
        private readonly ICapPublisher _capPublisher;
        private readonly IHttpClientFactory _httpClientFactory;
        private string adminId = "";
        public CertController(IResponseHelper resp, ICertRepo certRepo, ICertTemplateRepo certTemplateRepo, IHttpContextAccessor contextAccessor, ISixLaborHelper sixLaborHelper, IWebHostEnvironment en, IRedisCachingProvider redisCachingProvider, IHubContext<Tools.ConnectionHub> myHub, ICapPublisher capPublisher, IHttpClientFactory httpClientFactory)
        {
            _resp = resp;
            _certRepo = certRepo;
            _certTemplateRepo = certTemplateRepo;
            _contextAccessor = contextAccessor;
            _sixLaborHelper = sixLaborHelper;
            _en = en;
            _redisCachingProvider = redisCachingProvider;
            _myHub = myHub;
            _capPublisher = capPublisher;
            _httpClientFactory = httpClientFactory;
            if (_contextAccessor.HttpContext != null && _contextAccessor.HttpContext.Request.Cookies.TryGetValue("userId", out string cookieValue))
            {
                adminId = cookieValue;
            }
            // 创建一个有界的通道，用于传递消息（这里简单示例，可按需调整缓冲区等配置）
            //_channel = Channel.CreateBounded<string>(new BoundedChannelOptions(UtilConsts.SSECapacity)
            //{
            //    FullMode = BoundedChannelFullMode.DropOldest
            //});
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
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "whereJsonStr", "rd", "isAsc", "orderby" })]
        public IActionResult GetCerts(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _certRepo.getList(dto, out total), total }));
        }

        public IActionResult Import()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcelData(ImportDto model)
        {
            if (await _redisCachingProvider.KeyExistsAsync("generationCertMark"))
            {
                return Json(_resp.error("当前存在正在进行中的证书生成任务，请等待其完成后在进行新的任务"));

            }
            string key = $"importList-{Security.GenerateMD5Hash(model.Path)}";
            List<ImportTemplateDto> importList;
            if (await _redisCachingProvider.KeyExistsAsync(key))
            {
                importList = JsonHelper.JsonDeserialize<List<ImportTemplateDto>>(await _redisCachingProvider.StringGetAsync(key));
            }
            else
            {
                importList = await ExcelHelper<ImportTemplateDto>.GetImportData(model.Path);
            }

            //await _certTemplateRepo.CacheActivitiesAndExams(importList);

            int importTotal = importList.Count;
            if (importTotal > model.CertNumLength)
            {
                return Json(_resp.error($"导入的条数为{importTotal}条，高余设定的导入编号上限"));
            }
            var idNumbers = importList.Select(u => u.IdNumber).ToList();
            if (model.IsOverwrite == 0 && await _certRepo.getAnyAsync(u => idNumbers.Contains(u.IdNumber) && u.IsDeleted == 0))
            {
                return Json(_resp.ret(1,$"导入的数据中有重复的id号码"));
            }
            List<string> orginCertNums = importList.Select(u => u.CertNo).ToList();
            List<string> certNums = new List<string>();
            foreach (string item in orginCertNums)
            {
                certNums.Add(model.CertNumPrefix + (item.Length < model.CertNumLength ? new string('0', model.CertNumLength - item.Length) + item : item.Substring(0, model.CertNumLength)));
            }

            if (model.IsOverwrite == 0 && await _certRepo.getAnyAsync(u => certNums.Contains(u.CertNo) && u.IsDeleted == 0))
            {

                await _redisCachingProvider.StringSetAsync(key, JsonHelper.JsonSerialize(importList), TimeSpan.FromMinutes(2));
                return Json(_resp.ret(2, "部分编号记录已存在，是否要覆盖原数据"));
            }

            //await _myHub.Clients.Client(adminId).SendAsync("ReceiveMessage", "后台", "生成证书可能需要较长时间，取决于模板大小和您导入名单的数量，这是一个异步操作，生成情况会实时反馈到前台，期间您不必一直在当前页面停留");
            foreach (var items in importList.Chunk(10))
            {
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "InsertCertData", new ImportCapDto { ImportList = items.ToList(), ImportModel = model });

            }

            return Json(_resp.ret(0, "正在后台生成，稍后可在后台查看", importList.Count));
        }

        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "InsertCertData")]
        public async Task InsertCertData(ImportCapDto dto)
        {
            try
            {
                await _redisCachingProvider.StringSetAsync("generationCertMark", dto.ImportList.Count.ToString(), TimeSpan.FromHours(2));
                //序列化模板信息
                string resourceHost = ConfigurationHelper.GetSectionValue("resourceHost");
                var template = await _certTemplateRepo.getOneAsync(u => u.Id == dto.ImportModel.TemplateId);
                if (template == null)
                    return;
                var certConfig = JsonHelper.JsonDeserialize<CertTemplateDto>(template.ConfigJsonStrForImg);
                certConfig.certTempUrl = template.Url;
                string requestUrl = template.Url;
                if (!certConfig.certTempUrl.StartsWith("http"))
                {
                    requestUrl = resourceHost + template.Url;
                }
                using (var client = _httpClientFactory.CreateClient(UtilConsts.CLIENTFACTORYNAME))
                {
                    certConfig.certTempData = await client.GetByteArrayAsync(requestUrl);
                }
                List<DbServices.Entities.Cert> certList = new List<DbServices.Entities.Cert>();
                int finished = 0;
                foreach (var item in dto.ImportList)
                {
                    item.CertNo = dto.ImportModel.CertNumPrefix + (item.CertNo.Length < dto.ImportModel.CertNumLength ? new string('0', dto.ImportModel.CertNumLength - item.CertNo.Length) + item.CertNo : item.CertNo.Substring(0, dto.ImportModel.CertNumLength));

                    Type tImport = typeof(ImportTemplateDto);
                    Type tContent = typeof(TemplateContentModel);
                    var tmpContent = new TemplateContentModel();
                    foreach (var prop in tImport.GetProperties())
                    {
                        if (certConfig.contentList.Any(u => u.key.ToLower() == prop.Name.ToLower()))
                        {
                            tContent.GetProperty("content").SetValue(tmpContent, prop.GetValue(item));

                            certConfig.contentList.Where(u => u.key.ToLower() == prop.Name.ToLower()).First().content = tmpContent.content;
                        }
                    }
                    string filename = $"{dto.ImportModel.TemplateId}-{item.CertNo}";
                    string path = await _sixLaborHelper.MakeCertPic(_en.WebRootPath, certConfig, filename);

                    //long activityId = 0;
                    //if (await _redisCachingProvider.HExistsAsync("ImportActivities", item.ActivityTitle))
                    //{
                    //    activityId = Convert.ToInt64(await _redisCachingProvider.HGetAsync("ImportActivities", item.ActivityTitle));
                    //}
                    //Guid examId = Guid.Empty;
                    //if (await _redisCachingProvider.HExistsAsync("ImportExams", item.ExamTitle))
                    //{
                    //    examId = Guid.Parse(await _redisCachingProvider.HGetAsync("ImportExams", item.ExamTitle));
                    //}
                    certList.Add(new DbServices.Entities.Cert()
                    {
                        TemplateId = dto.ImportModel.TemplateId,
                        CertNo = item.CertNo,
                        IdNumber = item.IdNumber,
                        AwardName = item.AwardName,
                        Url = path,
                        CertContent = JsonHelper.JsonSerialize(certConfig.contentList.Select(u => new { u.key, u.content })),
                        
                        ActivityId=dto.ImportModel.ActivityId,
                        ExamId = dto.ImportModel.ExamId,
                        Title = dto.ImportModel.Title,
                        Status = DbServices.Entities.CertStatus.Enable,
                        AccountId = adminId
                    });
                    finished++;

                    //await _myHub.Clients.Client(adminId).SendAsync("ReceiveMessage", "后台", $"{item.AwardName}的证书生成完成,{finished}/{dto.ImportList.Count}");
                    // 这里模拟进度消息，格式化为字符串，真实场景按业务逻辑生成合适的进度表示消息
                    double precent = Math.Round((double)finished / dto.ImportList.Count, 2) * 100;
                    var progressMessage = $"{{\"progress\":\"{precent}\", \"msg\": \"{item.AwardName}的证书生成完成,{finished}/{dto.ImportList.Count}\" }}";
                    await _redisCachingProvider.StringSetAsync("certProgress", progressMessage, TimeSpan.FromMinutes(1));
                }
                
                await _certRepo.InsertCertBatch(certList);
                await _redisCachingProvider.KeyDelAsync("generationCertMark");

            }
            catch (Exception ex)
            {
                await _redisCachingProvider.KeyDelAsync("generationCertMark");

                Logger.Error($"导入错误:{ex.Message},\n{ex.StackTrace}");
            }
        }

        [HttpGet("progress")]
        public async Task GetCertGenerationProgress(string adminId, string examId)
        {
            Response.Headers["Content-Type"]="text/event-stream";
            Response.Headers["Cache-Control"]= "no-cache";
            if (HttpContext.Request.Protocol.StartsWith("HTTP/1.1"))
            {
                Response.Headers["Connection"] = "keep-alive";
            }            
            try
            {
                while (true)
                {
                    await Task.Delay(2000);
                    // 从通道中读取消息（这里等待消息到来）
                    var message = await _redisCachingProvider.StringGetAsync("certProgress");

                    if (string.IsNullOrEmpty(message))
                        return;
                    // 按照SSE协议格式发送数据到客户端
                    await Response.WriteAsync($"data:{message}\n\n");
                    await Response.Body.FlushAsync();
                }

            }
            catch (Exception ex)
            {
                // 可以记录异常等处理
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // 清理资源等操作
                await _redisCachingProvider.KeyDelAsync("certProgress");
            }
        }
    }

    public class ImportCapDto
    {
        public List<ImportTemplateDto> ImportList { get; set; }

        public ImportDto ImportModel { get; set; }
    }
}
