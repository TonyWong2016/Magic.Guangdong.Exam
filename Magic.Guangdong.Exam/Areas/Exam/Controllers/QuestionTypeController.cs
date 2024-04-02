using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Exam.Controllers
{
    /// <summary>
    /// 题目类型管理
    /// </summary>
    [Area("Exam")]
    public class QuestionTypeController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IQuestionTypeRepo _typeRepo;
        private readonly IHttpContextAccessor _context;
        private string adminId = "";
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="typeRepo"></param>
        public QuestionTypeController(IResponseHelper resp, IQuestionTypeRepo typeRepo,IHttpContextAccessor httpContextAccessor)
        {
            _resp = resp;
            _typeRepo = typeRepo;
            _context = httpContextAccessor;
            adminId = _context.HttpContext.Request.Cookies.Where(u => u.Key == "userId").Any() ?
                Assistant.Utils.FromBase64Str(_context.HttpContext.Request.Cookies.Where(u => u.Key == "userId").First().Value) : "system";
            _context = httpContextAccessor;
        }

        /// <summary>
        /// 试题题型
        /// </summary>
        /// <returns></returns>
        [RouteMark("试题题型")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 提醒列表
        /// </summary>
        /// <param name="pageDto"></param>
        /// <returns></returns>
        //[RouteMark("题型列表")]
        public IActionResult GetQuestionTypes(PageDto pageDto)
        {
            long total;
            return Json(_resp.success(new { items = _typeRepo.getList(pageDto, out total), total }));
        }

        /// <summary>
        /// 获取题型下拉列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetQuestionTypeSelects()
        {
            var result = await _typeRepo.getListAsync(u => u.IsDeleted == 0);
            return Json(_resp.success(result.OrderBy(u => u.Caption).Select(u => new { value = u.Id, text = u.Caption, type = u.Objective, single = u.SingleAnswer })));
        }

        [RouteMark("创建题型")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 新增题型
        /// </summary>
        /// <param name="questionType"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionType questionType)
        {
            if (await _typeRepo.getAnyAsync(u => u.Caption == questionType.Caption))
            {
                return Json(_resp.ret(-1, $"题目类型【{questionType.Caption}】已存在"));
            }
            questionType.CreatedBy = adminId;
            return Json(_resp.success(await _typeRepo.addItemAsync(questionType)));
        }

        /// <summary>
        /// 编辑页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RouteMark("编辑题型")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!await _typeRepo.getAnyAsync(u => u.Id == id))
            {
                return Content("题目类型不存在");
            }
            return View(await _typeRepo.getOneAsync(u => u.Id == id));
        }

        /// <summary>
        /// 修改题目类型
        /// </summary>
        /// <param name="questionType"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QuestionType questionType)
        {
            //没太大必要做这一步了
            //if (!await _typeRepo.getAnyAsync(u => u.Id == questionType.Id))
            //{
            //    return Json(_resp.ret(-1, $"题目类型【{questionType.Caption}】不存在"));
            //}
            if (await _typeRepo.getAnyAsync(u => u.Id != questionType.Id && u.Caption == questionType.Caption))
            {
                return Json(_resp.ret(-1, $"题目类型【{questionType.Caption}】已存在"));
            }
            questionType.UpdatedBy = adminId;
            questionType.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _typeRepo.updateItemAsync(questionType)));
        }

        /// <summary>
        /// 删除题目类型（fake delete）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        [RouteMark("删除题型")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await _typeRepo.getAnyAsync(u => u.Id == id))
            {
                return Content("题目类型不存在");
            }

            var questionType = await _typeRepo.getOneAsync(u => u.Id == id);
            string[] arr = new string[] { "单选题", "判断题", "多选题", "填空题", "问答题" };

            if (arr.Contains(questionType.Caption))
            {
                return Json(_resp.ret(-1, $"{questionType.Caption}是基础题型，不可删除"));
            }
            questionType.UpdatedBy = adminId;
            questionType.UpdatedAt = DateTime.Now;
            questionType.IsDeleted = 1;
            return Json(_resp.success(await _typeRepo.updateItemAsync(questionType)));
        }
    }
}
