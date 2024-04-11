using EasyCaching.Core;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class ExamController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IResponseHelper _resp;
        private readonly IExaminationRepo _examRepo;
        private readonly IRedisCachingProvider _redisCachingProvider;

        public ExamController(IHttpContextAccessor contextAccessor, IResponseHelper resp, IExaminationRepo examinationRepo, IRedisCachingProvider redisCachingProvider)
        {
            _contextAccessor = contextAccessor;
            _resp = resp;
            _examRepo = examinationRepo;
            _redisCachingProvider = redisCachingProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 100,VaryByQueryKeys = new string[] { "whereJsonStr","pageIndex","pageSize","orderby","isAsc" })]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> GetExams(PageDto dto)
        {
            return new JsonResult(_resp.success(
               await _examRepo.getListAsync(dto)));
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "id" })]
        public async Task<IActionResult> GetExam(Guid id)
        {
            var exam = await _examRepo.getOneAsync(u => u.Id == id);
            return new JsonResult(_resp.success(
               new
               {
                   title = exam.Title,
                   expenses = exam.Expenses,
                   id = exam.Id
               }
               ));
        }
    }
}
