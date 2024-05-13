using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Teacher;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Magic.Guangdong.Exam.Teacher.Areas.Teacher.Controllers
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
        private readonly ITeacherRecordScoringRepo _teacherRecordScoringRepo;
        private readonly ITeacherRepo _teacherRepo;
        private string _teacherId = "";

        public TeacherExamAssignController(IResponseHelper resp, ITeacherExamAssignRepo teacherExamAssignRepo, ITeacherRecordScoreLogRepo teacherRecordScoreLogRepo, IHttpContextAccessor contextAccessor, ITeacherExamAssignViewRepo teacherExamAssignViewRepo, IUserAnswerRecordViewRepo userAnswerRecordViewRepo, IRedisCachingProvider redisCachingProvider, ITeacherRecordScoringRepo teacherRecordScoringRepo, ITeacherRepo teacherRepo)
        {
            _resp = resp;
            _teacherExamAssignRepo = teacherExamAssignRepo;
            _teacherRecordScoreLogRepo = teacherRecordScoreLogRepo;
            _teacherRecordScoringRepo = teacherRecordScoringRepo;
            _contextAccessor = contextAccessor;
            _teacherExamAssignViewRepo = teacherExamAssignViewRepo;
            _userAnswerRecordViewRepo = userAnswerRecordViewRepo;
            _teacherRepo = teacherRepo;

            _redisCachingProvider = redisCachingProvider;
            _teacherId = (_contextAccessor != null && _contextAccessor.HttpContext != null && _contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "teacherId").Any()) ?
               Assistant.Utils.FromBase64Str(_contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "teacherId").First().Value) : "system";
        }

        #region 管理端功能，客户端不需要
        //public IActionResult Index()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "whereJsonStr", "pageIndex", "pageSize", "orderBy", "isAsc", "rd" })]
        //public IActionResult GetAssignList(PageDto dto)
        //{
        //    long total;
        //    return Json(_resp.success(new { items = _teacherExamAssignViewRepo.getList(dto, out total), total }));
        //}


        ///// <summary>
        ///// 给多位老师分配多门考试
        ///// </summary>
        ///// <param name="dto"></param>
        ///// <returns></returns>
        //[HttpPost, ValidateAntiForgeryToken]
        //public async Task<IActionResult> AssignExamsToTeachers(TeacherExamAssignStringDto dto)
        //{
        //    return Json(_resp.success(await _teacherExamAssignRepo.AssignByString(dto)));
        //}

        /// <summary>
        /// 移除单条分配（管理端功能，客户端不需要）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpPost, ValidateAntiForgeryToken]
        //public async Task<IActionResult> Remove(long id)
        //{
        //    var assign = await _teacherExamAssignRepo.getOneAsync(u => u.Id == id);
        //    if (await _teacherRecordScoreLogRepo.getAnyAsync(u => u.TeacherId == assign.TeacherId && u.ExamId == assign.ExamId))
        //    {
        //        return Json(_resp.error("该老师已经对其负责的考卷提交了判分记录，不可以删除分配关系"));
        //    }

        //    assign.IsDeleted = 1;
        //    assign.UpdatedAt = DateTime.Now;
        //    await _teacherExamAssignRepo.updateItemAsync(assign);
        //    return Json(_resp.success("removed successfully"));
        //}
        #endregion

        [ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "teacherId", "rd" })]
        public async Task<IActionResult> GetTeacherExamDrops(Guid teacherId)
        {
            var assigns = await _teacherExamAssignViewRepo.getListAsync(u => u.TeacherId == teacherId);
            return Json(_resp.success(assigns.Select(u => new
            {
                value = u.ExamId,
                text = u.ExamTitle
            })));
        }

        public async Task<IActionResult> DashBoard(Guid? teacherId)
        {
            Guid tmpId;
            if (teacherId == null && !string.IsNullOrEmpty(_teacherId) && Guid.TryParse(_teacherId, out tmpId))
                teacherId = tmpId;
            if (!await _teacherExamAssignRepo.getAnyAsync(u => u.TeacherId == teacherId && u.IsDeleted == 0))
                return Content("该老师尚未分配考试，无法查看试卷");

            return View((await _teacherRepo.getOneAsync(u=>u.Id==teacherId && u.IsDeleted==0)).Adapt<TeacherClientDto>());
        }

        public async Task<IActionResult> Papers(Guid? teacherId)
        {
            Guid tmpId;
            if (teacherId == null && !string.IsNullOrEmpty(_teacherId) && Guid.TryParse(_teacherId, out tmpId))
                teacherId = tmpId;
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


        public async Task<IActionResult> Detail(Guid teacherId, long recordId)
        {
            if (_contextAccessor.HttpContext == null || !_contextAccessor.HttpContext.Request.Headers.Referer.Any())
            {
                return Content("非法访问");
            }
            // 使用UrlHelper生成预期的URL（注意：此步骤假定路由配置已正确设置）
            string expectedReferrerUrl = Url.Action(nameof(Papers));
            // 获取实际的Referer URL
            string actualReferrerUrl = HttpContext.Request.Headers["Referer"].ToString();
            int queryIndex = actualReferrerUrl.IndexOf('?');
            if (queryIndex >= 0)
            {
                actualReferrerUrl = actualReferrerUrl.Substring(0, queryIndex);
            }
            // 比较实际Referer与预期的URL
            if (String.IsNullOrEmpty(actualReferrerUrl) || !actualReferrerUrl.EndsWith(expectedReferrerUrl, StringComparison.OrdinalIgnoreCase))
            {
                return Content("非法访问");
            }


            if (!await _teacherExamAssignRepo.getAnyAsync(u => u.TeacherId == teacherId))
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

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "recordId", "rd" })]
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

        [ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "submitRecordId", "rd" })]
        public async Task<IActionResult> GetLastMarkDetail(long submitRecordId)
        {
            var records = await _teacherRecordScoringRepo.getListAsync(u => u.SubmitRecordId == submitRecordId);
            if (!records.Any())
            {
                return Json(_resp.error("未找到该题目的判分记录"));
            }
            return Json(_resp.success(records.OrderByDescending(u => u.Id).Select(u => new { u.Id, u.SubjectiveItemScore, u.Remark, u.CreatedAt })));
        }



        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "teacherId" })]
        public async Task<IActionResult> GetTeacherSummaryData(Guid teacherId)
        {
            var datas = await _teacherExamAssignViewRepo.GetTeacherSummaryData(teacherId);
            var paperlist = await _teacherExamAssignViewRepo.GetTeacherPapersSummaryData(teacherId);
            var last7DaysList = await _teacherExamAssignViewRepo.GetTeacherMarkedCntLast7Days(teacherId);
            return Json(_resp.success(new { datas, paperlist, last7DaysList }));
        }


    }
}
