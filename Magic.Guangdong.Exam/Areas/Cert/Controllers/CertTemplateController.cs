using EasyCaching.Core;
using Essensoft.Paylink.Alipay.Domain;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Cert;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Transactions;
using Yitter.IdGenerator;

namespace Magic.Guangdong.Exam.Areas.Cert.Controllers
{
    [Area("Cert")]
    public class CertTemplateController : Controller
    {
        private readonly ICertTemplateRepo _certTemplateRepo;
        private readonly IResponseHelper _resp;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISixLaborHelper _sixLaborHelper;
        private readonly IWebHostEnvironment _en;
        private string adminId = "";
        public CertTemplateController(ICertTemplateRepo certTemplateRepo, IResponseHelper resp, IRedisCachingProvider redisCachingProvider,IHttpContextAccessor httpContextAccessor, ISixLaborHelper sixLaborHelper, IWebHostEnvironment en)
        {
            _certTemplateRepo = certTemplateRepo;
            _resp = resp;
            _redisCachingProvider = redisCachingProvider;
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("userId", out string cookieValue))
            {
                adminId = cookieValue;
            }
            _sixLaborHelper = sixLaborHelper;
            _en = en;
        }

        [RouteMark("证书模板")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            var template = await _certTemplateRepo.getOneAsync(u => u.Id == id);
            return View(template);
        }

        [ResponseCache(Duration = 100,VaryByQueryKeys =new string[] { "whereJsonStr","rd", "isAsc", "orderby" })]
        public IActionResult GetTemplates(PageDto dto)
        {
            long total;
            var list = _certTemplateRepo.getList(dto, out total);
            
            if (total > 0)
            {
                return Json(_resp.success(new
                {
                    items = list.Adapt<List<TemplateDto>>(),
                    total
                }
            ));
            }
            return Json(_resp.ret(1,"无数据"));
        }

        [ResponseCache(Duration = 100,VaryByQueryKeys = new string[] { "activityId","rd" })]
        public async Task<IActionResult> GetTemplateDrops(long activityId=0)
        {
            Expression<Func<CertTemplate, bool>> filter = p => p.IsDeleted == 0;
            if (activityId != 0)
            {
                filter = filter.And(p => p.ActivityId == activityId);

            }
            var ret = await _certTemplateRepo.getListAsync(filter);
            return Json(_resp.success(ret.Select(u => new
            {
                value = u.Id,
                text = u.Title
            })));
        }

        [RouteMark("添加证书模板")]
        public IActionResult Create()
        {
            return View(new TemplateDto());
        }
        
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TemplateDto dto)
        {
            if (await _certTemplateRepo.getAnyAsync(u => u.Title == dto.Title && u.IsDeleted==0))
            {
                return Json(_resp.error("模板名称已存在"));
            }
            
            var model = dto.Adapt<CertTemplate>();
            model.Id = YitIdHelper.NextId();
            model.CreatedBy = adminId;
            return Json(_resp.success(await
                _certTemplateRepo.addItemAsync(model)
                ));
        }

        [RouteMark("编辑证书模板")]
        public async Task<IActionResult> Edit(long id)
        {
            if (!await _certTemplateRepo.getAnyAsync(u => u.Id == id))
                return Json(_resp.error("模板不存在"));
            return View((await _certTemplateRepo.getOneAsync(u => u.Id == id))
                .Adapt<TemplateDto>());
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TemplateDto dto)
        {
            if(await _certTemplateRepo.getAnyAsync(u=>u.Title==dto.Title && u.IsDeleted==0 && u.Id!=dto.Id))
            {
                return Json(_resp.error("模板名称已存在"));
            }
            var template = await _certTemplateRepo.getOneAsync(u => u.Id == dto.Id);

            if (template.IsLock == CertTemplateLockStatus.Lock)
            {
                return Json(_resp.error("该模板已锁定，不能编辑"));
            }

            template = dto.Adapt<CertTemplate>();
            template.UpdatedAt = DateTime.Now;
            template.CreatedBy = adminId;
            return Json(_resp.success(await _certTemplateRepo.updateItemAsync(template)));
        }

        [RouteMark("删除证书模板")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(long id)
        {
            if (!await _certTemplateRepo.getAnyAsync(u => u.Id == id))
                return Json(_resp.error("模板不存在"));
            var template = await _certTemplateRepo.getOneAsync(u => u.Id == id);
            if (template.IsLock == CertTemplateLockStatus.Lock)
            {
                return Json(_resp.error("已有使用该模板下发的证书，导致模板被锁定，不能删除"));
            }
            template.UpdatedAt = DateTime.Now;
            template.IsDeleted = 1;
            template.Remark += "管理员删除："+adminId;
            return Json(_resp.success(await _certTemplateRepo.updateItemAsync(template)));
        }

        public async Task<IActionResult> Make(long? id)
        {
            if (id != null && await _certTemplateRepo.getAnyAsync(u => u.Id == id))
            {
                var template = await _certTemplateRepo.getOneAsync(u => u.Id == id);
                return View(template.Adapt<TemplateDto>());
            }
            return View(new TemplateDto());
        }

        /// <summary>
        /// 保存并预览
        /// </summary>
        /// <param name="config_str"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Preview(long id, string config_str, string canvasJson, string filename = "")
        {
            var config = JsonHelper.JsonDeserialize<CertTemplateDto>(config_str);
            if (string.IsNullOrEmpty(filename))
                filename = config.GetHashCode().ToString();
            var template = await _certTemplateRepo.getOneAsync(u => u.Id == id);
            template.ConfigJsonStrForImg = config_str;
            template.CanvasJson = canvasJson;
            template.CreatedBy = adminId;
            template.Remark += $"{adminId}预览模板";
            template.UpdatedAt = DateTime.Now;
            await _certTemplateRepo.updateItemAsync(template);
            return Json(_resp.success(await _sixLaborHelper.MakeCertPic(_en.WebRootPath, config, filename)));
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTemplate(long id,string templateJson,string canvasJson)
        {
            if(!await _certTemplateRepo.getAnyAsync(u=>u.Id==id && u.IsDeleted==0))
            {
                return Json(_resp.error("模板不存在"));
            }
            var template = await _certTemplateRepo.getOneAsync(u => u.Id == id);
            template.CanvasJson = canvasJson;
            template.ConfigJsonStrForImg = templateJson;
            template.CreatedBy = adminId;
            template.Remark += $"{adminId}修改模板";
            template.UpdatedAt= DateTime.Now;
            return Json(_resp.success(await _certTemplateRepo.updateItemAsync(template)));
        }

        /// <summary>
        /// 克隆模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RouteMark("克隆模板")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> CloneTemplate(long id)
        {
            if(await _certTemplateRepo.CloneTemplate(id, adminId))
            {
                return Json(_resp.success("克隆成功"));
            }
            return Json(_resp.error("克隆失败"));
        }
    }
}
