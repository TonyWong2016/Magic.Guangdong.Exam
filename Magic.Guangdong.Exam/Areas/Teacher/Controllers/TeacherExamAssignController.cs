﻿using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Teacher;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Magic.Guangdong.Exam.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class TeacherExamAssignController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ITeacherExamAssignRepo _teacherExamAssignRepo;
        private readonly ITeacherExamAssignViewRepo _teacherExamAssignViewRepo;
        private readonly ITeacherRecordScoringRepo _teacherRecordScoringRepo;
        private readonly IUserAnswerRecordViewRepo _userAnswerRecordViewRepo;
        private readonly IPaperRepo _paperRepo;
        private readonly IHttpContextAccessor _contextAccessor;

        public TeacherExamAssignController(IResponseHelper resp, ITeacherExamAssignRepo teacherExamAssignRepo,ITeacherRecordScoringRepo teacherRecordScoringRepo, IHttpContextAccessor contextAccessor, ITeacherExamAssignViewRepo teacherExamAssignViewRepo, IUserAnswerRecordViewRepo userAnswerRecordViewRepo, IPaperRepo paperRepo)
        {
            _resp = resp;
            _teacherExamAssignRepo = teacherExamAssignRepo;
            _teacherRecordScoringRepo = teacherRecordScoringRepo;
            _contextAccessor = contextAccessor;
            _teacherExamAssignViewRepo = teacherExamAssignViewRepo;
            _userAnswerRecordViewRepo = userAnswerRecordViewRepo;
            _paperRepo = paperRepo;
        }

        [RouteMark("教师分配")]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "whereJsonStr", "pageIndex", "pageSize", "orderBy", "isAsc", "rd" })]
        public IActionResult GetAssignList(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _teacherExamAssignViewRepo.getList(dto, out total), total }));
        }
                        

        /// <summary>
        /// 给多位老师分配多门考试
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignExamsToTeachers(TeacherExamAssignStringDto dto)
        {
            return Json(_resp.success(await _teacherExamAssignRepo.AssignByString(dto)));
        }

        /// <summary>
        /// 移除单条分配
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(long id)
        {
            if(await _teacherRecordScoringRepo.getAnyAsync(u => u.AssignId == id))
            {
                return Json(_resp.error("该老师已经对其负责的考卷提交了判分记录，不可以删除分配关系"));
            }
            var assign = await _teacherExamAssignRepo.getOneAsync(u => u.Id == id);
            assign.IsDeleted = 1;
            assign.UpdatedAt = DateTime.Now;
            await _teacherExamAssignRepo.updateItemAsync(assign);
            return Json(_resp.success("removed successfully"));
        }

        [ResponseCache(Duration = 60,VaryByQueryKeys =new string[] { "teacherId","rd" })]
        public async Task<IActionResult> GetTeacherExamDrops(Guid teacherId)
        {
            var assigns = await _teacherExamAssignViewRepo.getListAsync(u => u.TeacherId == teacherId);
            return Json(_resp.success(assigns.Select(u => new
            {
                value = u.ExamId,
                text = u.ExamTitle
            })));
        }

        [RouteMark("判卷列表")]
        public async Task<IActionResult> Papers(Guid teacherId)
        {
            if (!await _teacherExamAssignRepo.getAnyAsync(u => u.TeacherId == teacherId))
                return Content("该老师尚未分配考试，无法查看试卷");
            return View();
        }

        /// <summary>
        /// 获取试卷列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "whereJsonStr", "pageIndex", "pageSize", "orderBy", "isAsc", "rd" })]
        public IActionResult GetTeacherPapers(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _userAnswerRecordViewRepo.GetTeacherPapers(dto, out total), total }));
        }

        public async Task<IActionResult> GetTeacherPaper(Guid paperId,long recordId)
        {
            if (!await _paperRepo.getAnyAsync(u => u.Id == paperId))
            {
                return Content("试卷不存在");
            }
            TeacherSubjectiveMarkDto dto = new TeacherSubjectiveMarkDto();
            dto.MarkPaper = (await _paperRepo.PreviewPaper(paperId)).Adapt<MarkPaperDto>();
            //dto.UserAnswer = await _userAnswerRecordViewRepo.GetUserRecord(recordId);
            return View(await _paperRepo.PreviewPaper(paperId));
            
        }
    }
}