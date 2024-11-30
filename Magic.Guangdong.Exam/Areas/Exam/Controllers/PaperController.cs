using DotNetCore.CAP;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Dtos.System.Tags;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Exam.Controllers
{
    /// <summary>
    /// 试卷管理
    /// </summary>
    [Area("Exam")]
    public class PaperController : Controller
    {
        private readonly IPaperRepo _paperRepo;
        private readonly IExaminationRepo _examinationRepo;
        private readonly IResponseHelper _resp;
        private readonly ICapPublisher _capBus;
        private readonly IHttpContextAccessor _contextAccessor;
        private string adminId = "";
        public PaperController(IPaperRepo paperRepo, IExaminationRepo examinationRepo, IResponseHelper resp, ICapPublisher capBus, IHttpContextAccessor contextAccessor)
        {
            _paperRepo = paperRepo;
            _examinationRepo = examinationRepo;
            _resp = resp;
            _capBus = capBus;
            _contextAccessor = contextAccessor;
            adminId = (_contextAccessor.HttpContext!=null && _contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").Any()) ?
               Assistant.Utils.FromBase64Str(_contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").First().Value) : "system";

        }

        [RouteMark("试卷管理")]
        public IActionResult Index()
        {
            return View();
        }

        [RouteMark("生成试卷")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPaperRule(GeneratePaperDto model)
        {
            try
            {
                model.adminId = adminId;
                Guid[] paperIds = await _paperRepo.SetPaperRule(model);

                if(paperIds == null)
                {
                    return Json(_resp.ret(-1, "抽题失败，请检查题库里符合所选题型，科目，难度条件的题目总量是否满足设定的题目数量"));

                }
                Assistant.Logger.Debug("开始抽题喽");
                int i = 1;
                foreach (var item in paperIds.Chunk(50))
                {
                    Assistant.Logger.Debug($"第{i}把") ;
                    Assistant.Logger.Debug(string.Join(',', item));
                    i++;
                    if (!string.IsNullOrEmpty(model.tags))
                    {
                        TagPaperDto dto = new TagPaperDto()
                        {
                            PaperIds = item,
                            Tags = model.tags
                        };
                        await _capBus.PublishAsync(CapConsts.PREFIX + "BuildPaperTagRelation", dto, model.adminId);
                    }
                    await _capBus.PublishAsync(CapConsts.PREFIX + "GeneratePaper", item, model.adminId);

                }
                return Json(_resp.success(paperIds,"组卷成功，请回到列表页检查是否符合要求"));
            }
            catch (Exception ex)
            {
                Assistant.Logger.Error(ex);
                return Json(_resp.ret(-1, "抽题失败，" + ex.Message));

            }
            //if (!string.IsNullOrEmpty(model.tags) && paperIds != null)
            //{
            //    TagPaperDto dto = new TagPaperDto()
            //    {
            //        PaperIds = paperIds,
            //        Tags = model.tags
            //    };                
            //    await _capBus.PublishAsync(CapConsts.PREFIX + "BuildPaperTagRelation", dto, model.adminId);
            //}

            //if (paperIds != null)
            //{
            //    await _capBus.PublishAsync(CapConsts.PREFIX + "GeneratePaper", paperIds, model.adminId);
            //    return Json(_resp.success(paperIds));
            //}

        }

        /// <summary>
        /// 创建试卷
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [RouteMark("创建试卷")]
        public async Task<IActionResult> Create(Guid examId)
        {
            if (await _examinationRepo.getAnyAsync(u => u.Id.Equals(examId)))
            {

                return View();
            }
            return Redirect("/error?msg=考试不存在，木有鱼丸何来粗面~");
            //return View();
        }

        /// <summary>
        /// 获取试卷列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult GetPaperList(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _paperRepo.GetPaperList(dto, out total), total }));
        }

        /// <summary>
        /// 获取下拉列表
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "examId" })]
        public async Task<IActionResult> GetPaperMini(Guid? examId)
        {
            return Json(_resp.success(await _paperRepo.GetPaperMini(examId)));
        }

        /// <summary>
        /// 合成试卷的事件
        /// </summary>
        /// <param name="paperIds"></param>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "GeneratePaper")]
        public async Task GeneratePaper(Guid[] paperIds)
        {
            Assistant.Logger.Warning($"{DateTime.Now},开始生成试卷,共计{paperIds.Length}张");
            await Task.Run(()=>_paperRepo.GeneratePaper(paperIds, adminId));
        }

        /// <summary>
        /// 预览试卷（后台用）
        /// </summary>
        /// <param name="paperId"></param>
        /// <returns></returns>
        [RouteMark("预览试卷")]
        public async Task<IActionResult> Preview(Guid paperId)
        {
            if (await _paperRepo.getAnyAsync(u => u.Id == paperId))
            {
                return View(await _paperRepo.PreviewPaper(paperId));
            }
            return Content("看啥呢");
        }

        /// <summary>
        /// 提交试卷(后台或测试用，非前台用户使用)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPaper(SubmitPaperDto dto)
        {
            return Json(_resp.success(await _paperRepo.SubmitPaper(dto)));
        }

        /// <summary>
        /// 校准答案
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [RouteMark("校准答案")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPaperForCorrection(SubmitPaperForCorrectionDto dto)
        {
            return Json(_resp.success(await _paperRepo.SubmitPaperForCorrection(dto)));
        }

        /// <summary>
        /// 判分,好像已经作废了
        /// </summary>
        /// <param name="idNumber"></param>
        /// <param name="paperId"></param>
        /// <returns></returns>
        [RouteMark("试卷判分")]
        public async Task<IActionResult> PaperMarking(string idNumber, Guid paperId)
        {
            await Task.Delay(10);
            //return Json(_resp.success(await _paperRepo.Marking(idNumber, paperId, adminId)));
            return Json(_resp.ret(0, "使用常规方法判分"));
        }

        /// <summary>
        /// 删除试卷
        /// </summary>
        /// <param name="paperId"></param>
        /// <returns></returns>
        [RouteMark("删除试卷")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove([FromServices] IUserAnswerRecordViewRepo recordRepo, Guid Id)
        {
            if (await recordRepo.getAnyAsync(u => u.PaperId == Id))
            {
                return Json(_resp.error( "当前试卷已被用户抽取，不可删除！如果想避免该试卷被考生再次抽取，可将试卷状态调为禁用"));
            }
            var paper = await _paperRepo.getOneAsync(u => u.Id == Id);
            paper.IsDeleted = 1;
            paper.UpdatedAt = DateTime.Now;
            paper.Status = 0;
            paper.UpdatedBy = adminId;
            int ret = await _paperRepo.updateItemAsync(paper);
            if (ret > 0)
                return Json(_resp.success(ret));
            return Json(_resp.ret(-1, "删除失败"));
        }

        /// <summary>
        /// 自动分表
        /// </summary>
        /// <param name="relationRepo"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<IActionResult> AutoShardingRelation([FromServices] IRelationRepo relationRepo, string key = "tony")
        {
            if (key != "tony")
            {
                return Content("？？？");
            }
            return Json(_resp.success(await relationRepo.AutoShardingRelation(null)));
        }
    }
}
