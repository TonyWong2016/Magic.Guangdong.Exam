using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Magic.Guangdong.DbServices.Dtos.Exam.Questions;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.Exam.Extensions;

namespace Magic.Guangdong.Exam.Areas.Exam.Controllers
{
    [Area("Exam")]
    public class QuestionController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IQuestionRepo _questionRepo;
        private readonly IQuestionTypeRepo _typeRepo;
        private readonly ISubjectRepo _subjectRepo;
        private readonly IActivityRepo _activityRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string adminId="system";
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="questionRepo"></param>
        /// <param name="typeRepo"></param>
        /// <param name="subjectRepo"></param>
        public QuestionController(IResponseHelper resp, IQuestionRepo questionRepo, IQuestionTypeRepo typeRepo, ISubjectRepo subjectRepo,IActivityRepo activityRepo, IHttpContextAccessor contextAccessor)
        {
            _resp = resp;
            _questionRepo = questionRepo;
            _typeRepo = typeRepo;
            _subjectRepo = subjectRepo;
            _activityRepo = activityRepo;
            _contextAccessor = contextAccessor;
            adminId = _contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").Any() ?
                Assistant.Utils.FromBase64Str(_contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").First().Value) : "system";

        }
        [RouteMark("试题管理")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取题目列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "whereJsonStr", "pageIndex", "pageSize", "orderby", "isAsc","rd" })]
        public IActionResult GetQuestions(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _questionRepo.GetQuestions(dto, out total), total }));
        }

        /// <summary>
        /// 新增题目
        /// </summary>
        /// <returns></returns>
        [RouteMark("新增试题")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 保存题目
        /// </summary>
        /// <param name="question"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveQuestion(SubmitQuestionDto dto)
        {
            if (string.IsNullOrEmpty(dto.question.CreatedBy))
                dto.question.CreatedBy = adminId;
            
            dto.question.UpdatedBy = adminId;
            return Json(_resp.success(await _questionRepo.AddOrUpdateSingleQuestion(dto.question, dto.items)));
        }

        /// <summary>
        /// 编辑页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RouteMark("编辑试题")]
        public async Task<IActionResult> Edit([FromServices] IQuestionViewRepo questionViewRepo, long id)
        {
            if (!await _questionRepo.getAnyAsync(u => u.Id == id))
            {
                return Content("题目不存在");
            }
            return View(await questionViewRepo.getOneAsync(u => u.Id == id));
        }

        /// <summary>
        /// 获取题目选项
        /// </summary>
        /// <param name="questionItemRepo"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetQuestionItems([FromServices] IQuestionItemRepo questionItemRepo, long questionId)
        {
            var ret = await questionItemRepo.getListAsync(u => u.QuestionId == questionId && u.IsDeleted == 0);
            return Json(_resp.success(ret.Select(u => new
            {
                u.Id,
                u.OrderIndex,
                u.Description,
                u.DescriptionText,
                u.Code,
                u.IsAnswer,
                u.IsOption,

            })));
        }

        /// <summary>
        /// 修改题目类型
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Question question)
        {
            if (await _questionRepo.getAnyAsync(u => u.Id != question.Id && u.Title == question.Title))
            {
                return Json(_resp.ret(-1, $"题目类型【{question.Title}】已存在"));
            }
            question.UpdatedBy = adminId;
            question.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _questionRepo.updateItemAsync(question)));
        }

        /// <summary>
        /// 删除题目类型（fake delete）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        [RouteMark("移除试题")]
        public async Task<IActionResult> Delete([FromServices] IRelationRepo relationRepo, long id)
        {
            if (await relationRepo.getAnyAsync(u => u.QuestionId == id && u.IsDeleted==0))
            {
                return Json(_resp.ret(-1, "当前题目已被某些试卷抽中，若要删除题目，请先删除抽中该题目的试卷。若无法知晓具体哪套试卷抽中该题，建议禁用该题目，避免后续的组卷仍然抽中该题。"));
            }
            var question = await _questionRepo.getOneAsync(u => u.Id == id);
            question.UpdatedBy = adminId;
            question.UpdatedAt = DateTime.Now;
            question.IsDeleted = 1;
            return Json(_resp.success(await _questionRepo.updateItemAsync(question)));
        }

        /// <summary>
        /// 导入试题（word格式）
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        [RouteMark("导入word试题")]
        public async Task<IActionResult> ImportQuestionFromWord([FromServices] IWebHostEnvironment en, Guid subjectId, long activityId, string path)
        {
            //var types = await _typeRepo.getListAsync(u => u.IsDeleted == 0);
            //var importList = await new WordUtils(_questionRepo,typeRepo, _resp).ParseWord("C:\\Users\\Administrator\\Desktop\\测试导入.docx",subjectId);
            path = en.WebRootPath + path;
            var importList = await new Utils.WordUtils(_typeRepo, _subjectRepo,_activityRepo).ParseWord(path, subjectId, activityId);
            return Json(_resp.success(await _questionRepo.ImportQuestionsFromWord(importList)));
        }

        /// <summary>
        /// 生成导入模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        [RouteMark("生成excel导入模板")]
        public async Task<IActionResult> GenerationImportTemplate(string templateName = "excel题库导入模板")
        {
            return Json(_resp.success(await ExcelHelper<ImportQuestionDto>.GenerateTemplate(templateName)));
        }

        /// <summary>
        /// 从excel里导入
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        [RouteMark("导入excel试题")]
        public async Task<IActionResult> ImportQuestionFromExcel(string path)
        {
            try
            {
                List<ImportQuestionDto> importList = await ExcelHelper<ImportQuestionDto>.GetImportData(path);
                var types = (await _typeRepo.getListAsync(u => u.IsDeleted == 0)).Select(u => new
                {
                    u.Id,u.Caption
                });
                var subjects = (await _subjectRepo.getListAsync(u => u.IsDeleted == 0)).Select(u => new
                {
                    u.Id,u.Caption
                });
                var activities = (await _activityRepo.getListAsync(u => u.IsDeleted == 0)).Select(u => new
                {
                    u.Id,u.Title
                });
                foreach (var item in importList)
                {
                    if (!types.Any(u => u.Caption == item.QuestionType))
                    {
                        return Json(_resp.ret(-1, "有不存在的题型，请确保导入的数据中的题型已录入系统"));
                    }
                    item.QuestionTypeId = types.Where(u => u.Caption == item.QuestionType).First().Id;

                    if (!subjects.Any(u => u.Caption == item.QuestionSubject))
                    {
                        return Json(_resp.ret(-1, "有不存在的科目，请确保导入的数据中的科目已录入系统"));
                    }
                    item.SubjectId = subjects.Where(u => u.Caption == item.QuestionSubject).First().Id;

                    //item.ActivityId = activities.Any() ? activities.Where(u => u.Title == item.ActivityTitle).First().Id : 0;
                    if (string.IsNullOrEmpty(item.ActivityTitle))
                        item.ActivityId = 0;
                    else if (!activities.Where(u => u.Title == item.ActivityTitle).Any())
                        return Json(_resp.ret(-1, "有不存在的活动，请确保导入的数据中的活动已录入系统，如要录入通用题库则清空活动标题单元格"));
                    else
                        item.ActivityId = activities.Where(u => u.Title == item.ActivityTitle).First().Id;

                    item.CreateBy = adminId;

                }

                return Json(_resp.success(await _questionRepo.ImportQuestionFromExcel(importList)));
            }
            catch
            {
                return Json(_resp.ret(-1, "导入表格出错，请检查导入表格中的数据是否符合要求"));
            }
        }

    }
}
