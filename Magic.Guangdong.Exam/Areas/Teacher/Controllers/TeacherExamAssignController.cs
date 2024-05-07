using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Teacher;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class TeacherExamAssignController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ITeacherExamAssignRepo _teacherExamAssignRepo;
        private readonly ITeacherRecordScoringRepo _teacherRecordScoringRepo;
        private readonly IHttpContextAccessor _contextAccessor;

        public TeacherExamAssignController(IResponseHelper resp, ITeacherExamAssignRepo teacherExamAssignRepo,ITeacherRecordScoringRepo teacherRecordScoringRepo, IHttpContextAccessor contextAccessor)
        {
            _resp = resp;
            _teacherExamAssignRepo = teacherExamAssignRepo;
            _teacherRecordScoringRepo = teacherRecordScoringRepo;
            _contextAccessor = contextAccessor;
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
            return Json(_resp.success(new { items = _teacherExamAssignRepo.GetTeacherExamAssigns(dto, out total), total }));
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
    }
}
