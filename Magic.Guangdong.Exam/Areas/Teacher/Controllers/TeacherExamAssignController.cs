using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
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
        private readonly ITeacherRecordScoreLogRepo _teacherRecordScoreLogRepo;
        private readonly IUserAnswerRecordViewRepo _userAnswerRecordViewRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRedisCachingProvider _redisCachingProvider;

        public TeacherExamAssignController(IResponseHelper resp, ITeacherExamAssignRepo teacherExamAssignRepo, ITeacherRecordScoreLogRepo teacherRecordScoreLogRepo, IHttpContextAccessor contextAccessor, ITeacherExamAssignViewRepo teacherExamAssignViewRepo, IUserAnswerRecordViewRepo userAnswerRecordViewRepo, IRedisCachingProvider redisCachingProvider)
        {
            _resp = resp;
            _teacherExamAssignRepo = teacherExamAssignRepo;
            _teacherRecordScoreLogRepo = teacherRecordScoreLogRepo;
            _contextAccessor = contextAccessor;
            _teacherExamAssignViewRepo = teacherExamAssignViewRepo;
            _userAnswerRecordViewRepo = userAnswerRecordViewRepo;
            _redisCachingProvider = redisCachingProvider;
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
            var assign = await _teacherExamAssignRepo.getOneAsync(u => u.Id == id);
            if (await _teacherRecordScoreLogRepo.getAnyAsync(u => u.TeacherId == assign.TeacherId && u.ExamId == assign.ExamId))
            {
                return Json(_resp.error("该老师已经对其负责的考卷提交了判分记录，不可以删除分配关系"));
            }
            
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


        public async Task<IActionResult> Detail(Guid teacherId,long recordId)
        {
            if(!await _teacherExamAssignRepo.getAnyAsync(u => u.TeacherId == teacherId))
            {
                return Content("没有分配信息");
            }
            if (!await _redisCachingProvider.KeyExistsAsync($"subjective_{teacherId}_{recordId}"))
            {
                var result = await _teacherExamAssignViewRepo.GetSubjectiveQuestionAndAnswers(recordId);
                if (result == null)
                    return Content("没有试卷信息");
                result.examInfoDto.TeacherId = teacherId;
                await _redisCachingProvider.StringSetAsync($"subjective_{teacherId}_{recordId}", JsonHelper.JsonSerialize(result), DateTime.Now.AddMinutes(5) - DateTime.Now);
                return View(result);
            }
            return View(JsonHelper.JsonDeserialize<TeacherSubjectiveMarkDto>(await _redisCachingProvider.StringGetAsync($"subjective_{teacherId}_{recordId}")));
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSubjectiveScore(SaveSubjectiveScoreDto dto)
        {
            Console.WriteLine(JsonHelper.JsonSerialize(dto));

            return Json(_resp.success(await _teacherExamAssignRepo.SaveSubjectiveScore(dto)));
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] {"recordId","rd" })]
        public async Task<IActionResult> GetTeacherRecordScoreLog(long recordId)
        {
            var items = await _teacherExamAssignViewRepo.GetSubjectiveScoreLog(recordId);
            if (items.Count > 0)
            {
                return Json(_resp.success(items));
            }
            return Json(_resp.error("还没有产生打分记录"));
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "recordId", "rd" })]
        public async Task<IActionResult> AutoGetNextRecord(long recordId)
        {
            var currRecord = await _userAnswerRecordViewRepo.getOneAsync(u => u.Id == recordId);
            var records = (await _userAnswerRecordViewRepo.getListAsync(u => u.ExamId == currRecord.ExamId)).Select(u => u.Id).ToList();
            int currIndex = records.IndexOf(recordId);
            if (currIndex + 1 < records.Count())
            {
                return Json(_resp.success(records[currIndex + 1]));
            }
            return Json(_resp.error("当前是最后一条记录"));
        }
    }
    
    
}
