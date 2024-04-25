using EasyCaching.Core;
using Essensoft.Paylink.Alipay.Domain;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Cert;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Magic.Guangdong.Exam.Areas.Cert.Controllers
{
    [Area("Cert")]
    public class CertTemplateController : Controller
    {
        private readonly ICertTemplateRepo _certTemplateRepo;
        private readonly IResponseHelper _resp;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string adminId = "";
        public CertTemplateController(ICertTemplateRepo certTemplateRepo, IResponseHelper resp, IRedisCachingProvider redisCachingProvider,IHttpContextAccessor httpContextAccessor)
        {
            _certTemplateRepo = certTemplateRepo;
            _resp = resp;
            _redisCachingProvider = redisCachingProvider;
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("userId", out string cookieValue))
            {
                adminId = cookieValue;
            }
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
            return Json(_resp.success(
                await _certTemplateRepo.getListAsync(filter)));
        }

        [RouteMark("添加证书模板")]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TemplateDto dto)
        {
            if (await _certTemplateRepo.getAnyAsync(u => u.Title == dto.Title))
            {
                return Json(_resp.error("模板名称已存在"));
            }
            return Json(_resp.success(await
                _certTemplateRepo.addItemAsync(dto.Adapt<CertTemplate>())
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
    }
}
