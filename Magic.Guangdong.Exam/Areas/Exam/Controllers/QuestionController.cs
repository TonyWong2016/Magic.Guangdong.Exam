﻿using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Exam.Controllers
{
    [Area("Exam")]
    public class QuestionController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IQuestionRepo _questionRepo;
        private readonly IQuestionTypeRepo _typeRepo;
        private readonly ISubjectRepo _subjectRepo;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="questionRepo"></param>
        /// <param name="typeRepo"></param>
        /// <param name="subjectRepo"></param>
        public QuestionController(IResponseHelper resp, IQuestionRepo questionRepo, IQuestionTypeRepo typeRepo, ISubjectRepo subjectRepo)
        {
            _resp = resp;
            _questionRepo = questionRepo;
            _typeRepo = typeRepo;
            _subjectRepo = subjectRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取题目列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult GetQuestions(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _questionRepo.GetQuestions(dto, out total), total }));
        }

        /// <summary>
        /// 新增题目
        /// </summary>
        /// <returns></returns>
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
        public async Task<IActionResult> SaveQuestion(Question question, List<QuestionItem> items)
        {
            if (string.IsNullOrEmpty(question.CreatedBy))
                question.CreatedBy = HttpContext.User.Claims.First().Value;
            question.UpdatedBy = HttpContext.User.Claims.First().Value;
            return Json(_resp.success(await _questionRepo.AddOrUpdateSingleQuestion(question, items)));
        }

        /// <summary>
        /// 编辑页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
            question.UpdatedBy = HttpContext.User.Claims.First().Value;
            question.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _questionRepo.updateItemAsync(question)));
        }

        /// <summary>
        /// 删除题目类型（fake delete）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromServices] IRelationRepo relationRepo, long id)
        {
            if (await relationRepo.getAnyAsync(u => u.QuestionId == id))
            {
                return Json(_resp.ret(-1, "当前题目已被某些试卷抽中，若要删除题目，请先删除抽中该题目的试卷。若无法知晓具体哪套试卷抽中该题，建议禁用该题目，避免后续的组卷仍然抽中该题。"));
            }
            var question = await _questionRepo.getOneAsync(u => u.Id == id);
            question.UpdatedBy = HttpContext.User.Claims.First().Value;
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
        public async Task<IActionResult> ImportQuestionFromWord([FromServices] IWebHostEnvironment en, Guid subjectId, string columnId, string path)
        {
            //var types = await _typeRepo.getListAsync(u => u.IsDeleted == 0);
            //var importList = await new WordUtils(_questionRepo,typeRepo, _resp).ParseWord("C:\\Users\\Administrator\\Desktop\\测试导入.docx",subjectId);
            path = en.WebRootPath + path;
            var importList = await new Utils.WordUtils(_typeRepo, _subjectRepo).ParseWord(path, subjectId, columnId);
            return Json(_resp.success(await _questionRepo.ImportQuestionsFromWord(importList)));
        }

        /// <summary>
        /// 生成导入模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
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
        public async Task<IActionResult> ImportQuestionFromExcel(string path, string columnId)
        {
            try
            {
                List<ImportQuestionDto> importList = await ExcelHelper<ImportQuestionDto>.GetImportData(path);
                var types = await _typeRepo.getListAsync(u => u.IsDeleted == 0);
                var subjects = await _subjectRepo.getListAsync(u => u.IsDeleted == 0);
                foreach (var item in importList)
                {
                    if (!types.Any(u => u.Caption == item.QuestionType))
                    {
                        return Json(_resp.ret(-1, "有不存在的题型，请确保导入的数据中的题型已录入系统"));
                    }
                    if (!subjects.Any(u => u.Caption == item.QuestionSubject))
                    {
                        return Json(_resp.ret(-1, "有不存在的科目，请确保导入的数据中的科目已录入系统"));
                    }
                    item.SubjectId = subjects.Find(u => u.Caption == item.QuestionSubject).Id;
                    item.QuestionTypeId = types.Find(u => u.Caption == item.QuestionType).Id;
                    item.CreateBy = HttpContext.User.Claims.First().Value;
                    item.ColumnId = columnId;
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