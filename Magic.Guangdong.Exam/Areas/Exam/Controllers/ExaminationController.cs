﻿using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Dtos.Exam.Examinations;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Exam.Controllers
{
    /// <summary>
    /// 考试
    /// </summary>
    [Area("Exam")]
    public class ExaminationController : Controller
    {
        private readonly IExaminationRepo _examinationRepo;
        private readonly IUserAnswerRecordRepo _userAnswerRecordRepo;
        private readonly ICapPublisher _capBus;
        private readonly IResponseHelper _resp;
        private readonly IRedisCachingProvider _redisCachingProvider;
        public ExaminationController(IExaminationRepo examinationRepo, IUserAnswerRecordRepo userAnswerRecordRepo, ICapPublisher capBus, IResponseHelper resp, IRedisCachingProvider redisCachingProvider)
        {
            _examinationRepo = examinationRepo;
            _userAnswerRecordRepo = userAnswerRecordRepo;
            _capBus = capBus;
            _resp = resp;
            _redisCachingProvider = redisCachingProvider;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.onlineExamer = (await _redisCachingProvider.HGetAllAsync("UserExamLog")).Count();
            ViewBag.clientHost = ConfigurationHelper.GetSectionValue("ExamClientHost");
            ViewBag.allowPure = await _redisCachingProvider.StringGetAsync("allowPure");
            return View();
        }

        /// <summary>
        /// 设定免登录
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPure(string status)
        {
            bool ret = await _redisCachingProvider.StringSetAsync("allowPure", status);
            if (ret)
                return Json(_resp.success(1, "操作成功"));
            return Json(_resp.ret(0, "操作失败"));
        }

        /// <summary>
        /// 获取考试列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult GetExamList(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _examinationRepo.GetExamList(dto, out total), total }));
        }

        /// <summary>
        /// 获取考试下拉列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "id", "type","rd" })]
        public async Task<IActionResult> GetExamMini(string id, int type = 0)
        {
            return Json(_resp.success(await _examinationRepo.GetExamMini(id, type)));
        }

        /// <summary>
        /// 创建考试
        /// </summary>
        /// <returns></returns>
        [RouteMark("创建考试")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 创建考试
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Examination model)
        {
            if (await _examinationRepo.getAnyAsync(u => u.Title == model.Title && u.AssociationId == model.AssociationId && u.IsDeleted == 0))
            {
                return Json(_resp.ret(-1, "当前活动下已创建相同标题的考试"));
            }
            //model.CreatedBy = HttpContext.User.Claims.First().Value;

            return Json(_resp.success(await _examinationRepo.addItemAsync(model)));
        }

        /// <summary>
        /// 编辑考试
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RouteMark("编辑考试")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (await _examinationRepo.getAnyAsync(u => u.Id == id))
            {
                var exam = await _examinationRepo.getOneAsync(u => u.Id == id);
                return View(exam);
            }
            return Redirect("/error?msg=考试不存在");
        }

        /// <summary>
        /// 保存考试
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Examination model)
        {
            model.UpdatedAt = DateTime.Now;
            //return Json(_resp.success(await _examinationRepo.updateItemAsync(model)));
            return Json(_resp.success(await _examinationRepo.UpdateExamInfo(model)));
        }

        [RouteMark("移除考试")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove([FromServices] IUserAnswerRecordRepo recordRepo, Guid id)
        {
            if (await recordRepo.getAnyAsync(u => u.ExamId == id))
            {
                return Json(_resp.error("该考试下已经产生了用户答题记录，不可直接删除，如果要避免考试生效，请将其状态调为禁用"));
            }
            var exam = await _examinationRepo.getOneAsync(u => u.Id == id);
            exam.IsDeleted = 1;
            exam.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _examinationRepo.updateItemAsync(exam), "删除成功"));
        }

        /// <summary>
        /// 克隆考试
        /// </summary>
        /// <param name="examId"></param>
        /// <param name="cloneExamName"></param>
        /// <param name="clonePaperTitle"></param>
        /// <returns></returns>
        [RouteMark("克隆考试")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CloneExam(CloneDto dto)
        {
            var paperIds = await _examinationRepo.CloneExam(dto.examId, dto.adminId, dto.cloneExamName, dto.clonePaperTitle);
            if (paperIds != null && paperIds.Length > 0)
            {
                _capBus.Publish(CapConsts.PREFIX + "GeneratePaper", paperIds, dto.adminId);
                return Json(_resp.success("克隆成功"));
            }
            return Json(_resp.ret(-1, "克隆失败"));
        }
    }
}