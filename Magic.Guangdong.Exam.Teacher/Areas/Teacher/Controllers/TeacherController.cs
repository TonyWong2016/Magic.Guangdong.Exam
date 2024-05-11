using DotNetCore.CAP;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Teacher;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Linq.Expressions;
using System.Text;

namespace Magic.Guangdong.Exam.Teacher.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class TeacherController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ITeacherRepo _teacherRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICapPublisher _capPublisher;
        private readonly ITeacherExamAssignViewRepo _teacherExamAssignViewRepo;
        private string teacherId = "teacher";
        public TeacherController(IResponseHelper resp, ITeacherRepo teacherRepo, IHttpContextAccessor contextAccessor, ICapPublisher capPublisher, ITeacherExamAssignViewRepo teacherExamAssignViewRepo)
        {
            _resp = resp;
            _teacherRepo = teacherRepo;
            _contextAccessor = contextAccessor;
            _capPublisher = capPublisher;
            teacherId = _contextAccessor.HttpContext != null && _contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "teacherId").Any() ?
               Utils.FromBase64Str(_contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "teacherId").First().Value) : "teacher";
            _teacherExamAssignViewRepo = teacherExamAssignViewRepo;
        }


        [ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "whereJsonStr", "pageIndex", "pageSize", "orderby", "isAsc" })]
        public IActionResult GetTeachers(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _teacherRepo.getList(dto, out total), total }));
        }

        [ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "teacherId", "rd" })]
        public async Task<IActionResult> GetTeacherDrops(Guid teacherId)
        {
            return Json(_resp.success(await _teacherRepo.getOneAsync(u => u.Id == teacherId)));

        }

        /// <summary>
        /// 获取考试下拉列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "teacherId", "rd" })]
        public async Task<IActionResult> GetTeacherExamDrops(Guid teacherId)
        {
            return Json(_resp.success(await _teacherExamAssignViewRepo.GetTeacherExams(teacherId)));
        }

        /// <summary>
        /// 获取下拉列表
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "examId" })]
        public async Task<IActionResult> GetPaperMini([FromServices]IPaperRepo _paperRepo, Guid? examId)
        {
            return Json(_resp.success(await _paperRepo.GetPaperMini(examId)));
        }
    }
}
