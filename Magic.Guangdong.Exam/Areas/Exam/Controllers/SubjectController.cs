using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Exam.Controllers
{
    /// <summary>
    /// 科目管理
    /// </summary>
    [Area("Exam")]
    public class SubjectController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ISubjectRepo _subjectRepo;
        private readonly IQuestionRepo _questionRepo;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="capBus"></param>
        /// <param name="subjectRepo"></param>
        public SubjectController(IResponseHelper resp, IQuestionRepo questionRepo, ISubjectRepo subjectRepo)
        {
            _resp = resp;
            _subjectRepo = subjectRepo;
            _questionRepo = questionRepo;
        }
        [RouteMark("科目管理")]
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 获取科目列表
        /// </summary>
        /// <param name="pageDto"></param>
        /// <returns></returns>
        public IActionResult GetSubjects(PageDto pageDto)
        {
            long total;
            return Json(_resp.success(new { items = _subjectRepo.getList(pageDto, out total), total }));
        }

        /// <summary>
        /// 获取科目下拉列表
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 200, VaryByQueryKeys = new string[] { "rd" })]
        public async Task<IActionResult> GetSubjectSelects()
        {
            var result = await _subjectRepo.getListAsync(u => u.IsDeleted == 0);
            return Json(_resp.success(result.Select(u => new { value = u.Id, text = u.Caption, title = u.Caption })));
        }
        [RouteMark("创建科目")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 新增科目
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject subject)
        {
            if (await _subjectRepo.getAnyAsync(u => u.Caption == subject.Caption))
            {
                return Json(_resp.ret(-1, $"学科【{subject.Caption}】已存在"));
            }
            subject.CreatedBy = HttpContext.User.Claims.First().Value;
            return Json(_resp.success(await _subjectRepo.addItemAsync(subject)));
        }

        /// <summary>
        /// 编辑页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RouteMark("编辑科目")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!await _subjectRepo.getAnyAsync(u => u.Id == id))
            {
                return Content("学科不存在");
            }
            return View(await _subjectRepo.getOneAsync(u => u.Id == id));
        }

        /// <summary>
        /// 修改学科
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Subject subject)
        {
            //没太大必要做这一步了
            //if (!await _subjectRepo.getAnyAsync(u => u.Id == subject.Id))
            //{
            //    return Json(_resp.ret(-1, $"学科【{subject.Caption}】不存在"));
            //}
            if (await _subjectRepo.getAnyAsync(u => u.Id != subject.Id && u.Caption == subject.Caption))
            {
                return Json(_resp.ret(-1, $"学科【{subject.Caption}】已存在"));
            }
            subject.UpdatedBy = HttpContext.User.Claims.First().Value;
            subject.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _subjectRepo.updateItemAsync(subject)));
        }

        /// <summary>
        /// 删除学科（fake delete）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RouteMark("删除科目")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await _subjectRepo.getAnyAsync(u => u.Id == id))
            {
                return Content("学科不存在");
            }
            if (await _questionRepo.getAnyAsync(u => u.SubjectId == id))
            {
                return Json(_resp.ret(-1, "当前学科下存在绑定的题目，请先将题库中归属该科目的题目清除，再执行删除科目的操作"));
            }
            var subject = await _subjectRepo.getOneAsync(u => u.Id == id);
            subject.UpdatedBy = HttpContext.User.Claims.First().Value;
            subject.UpdatedAt = DateTime.Now;
            subject.IsDeleted = 1;
            return Json(_resp.success(await _subjectRepo.updateItemAsync(subject)));
        }
    }
}
