using DotNetCore.CAP;
using Essensoft.Paylink.Alipay.Domain;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.System.Tags;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NPOI.OpenXmlFormats.Vml;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    public class TagsController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ITagsRepo _tagsRepo;
        private readonly ITagRelationsRepo _tagRelationsRepo;

        public TagsController(IResponseHelper responseHelper, ITagsRepo tagsRepo, ITagRelationsRepo tagRelationsRepo) {
            _resp = responseHelper;
            _tagsRepo = tagsRepo;
            _tagRelationsRepo = tagRelationsRepo;
        }

        [Extensions.RouteMark("标签管理")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult GetList(PageDto dto)
        {
            long total = 0;
            return Json(_resp.success(new { items = _tagsRepo.getList(dto, out total), total }));
        }

        [ResponseCache(Duration = 10, VaryByQueryKeys = ["rd"])]
        public async Task<IActionResult> GetItems()
        {
            var items = await _tagsRepo.getListAsync(u => u.IsDeleted == 0);
            return Json(_resp.success(items.Select(u => new { value = u.Id, text = u.Title, name = u.Title })));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title)
        {
            if (await _tagsRepo.getAnyAsync(u => u.Title == title && u.IsDeleted == 0))
            {
                return Json(_resp.error("已存在该标签"));
            }
            return Json(_resp.success(await _tagsRepo.addItemAsync(
                new DbServices.Entities.Tags() { Title = title })));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tags tag)
        {
            if (await _tagsRepo.getAnyAsync(u => u.Id != tag.Id && u.Title == tag.Title && u.IsDeleted == 0))
            {
                return Json(_resp.error("已存在该标签"));
            }
            // var tag = await _tagsRepo.getOneAsync(u => u.Id == id);

            tag.UpdatedAt = DateTime.Now;
            await _tagsRepo.updateItemAsync(tag);
            return Json(_resp.success(true));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(long id,bool force=false)
        {
            int ret = await _tagsRepo.RemoveRelationsByTagId(id, force);
            if (ret < 0)
            {
                return Json(_resp.error("删除失败"));
               
            }
            else if(ret == 1)
            {
                return Json(_resp.ret(ret, "删除失败，已存在绑定的标签资源"));
            }
            return Json(_resp.success(true));
            //var relations = await _tagsRelationsRepo.getListAsync(u => u.TagId == id);

        }


        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "BuildPaperTagRelation")]
        public async Task BuildPaperTagRelation(TagPaperDto dto)
        {
            Assistant.Logger.Warning($"{DateTime.Now},开始关联资源【试卷-标签】,共计{dto.PaperIds.Length}张");
            string[] parts = dto.Tags.Split(',');
            List<TagRelations> tagRelations = new List<TagRelations>();
            foreach (string tagId in parts)
            {
                foreach (Guid paperId in dto.PaperIds)
                {
                    string associationId = paperId.ToString();
                    string hashStr = Security.GenerateMD5Hash(tagId + associationId + "Paper");
                    tagRelations.Add(new TagRelations()
                    {
                        TagId = Convert.ToInt64(tagId),
                        AssociationId = paperId.ToString(),
                        TableName = "Paper",
                        OriginalId = "",
                        HashRelation = hashStr
                    });
                }
            }
            //
            //await _paperRepo.GeneratePaper(paperIds, adminId);
            await _tagRelationsRepo.addItemsAsync(tagRelations);
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = ["paperId"])]
        public async Task<IActionResult> GetPaperTags(string paperId)
        {
            return Json(_resp.success( await _tagRelationsRepo.GetPaperTags(paperId)));
        }
    }
}
