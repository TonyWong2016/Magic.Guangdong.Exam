using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.Examinations;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Exam.Controllers
{
    [Area("Exam")]
    public class ScoreSchemeController : Controller
    {
        private IScoreSchemeRepo _scoreSchemeRepo;
        private IExaminationRepo _examinationRepo;
        private IResponseHelper _resp;

        public ScoreSchemeController(IScoreSchemeRepo scoreSchemeRepo, IResponseHelper resp, IExaminationRepo examinationRepo)
        {
            _resp = resp;
            _scoreSchemeRepo = scoreSchemeRepo;
            _examinationRepo = examinationRepo;
        }

        [RouteMark("评分标准管理")]
        public IActionResult Index()
        {
            return View();
        }

        //[ResponseCache(Duration = 10,VaryByQueryKeys = new string[] {"rd"})]
        public IActionResult GetList(PageDto pageDto) 
        {
            long total;
            var items = _scoreSchemeRepo.getList(pageDto, out total);
            return Json(_resp.success(new { items = items.Adapt<List<ScoreScheme>>(), total }));
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new string[] { "rd" })]
        public async Task<IActionResult> GetSelectItems()
        {
            return Json(_resp.success((await _scoreSchemeRepo.getListAsync(u => u.IsDeleted == 0))
                .Select(u => new { value = u.Id, text = u.Title })));
        }

        public IActionResult Create()
        {
            return View(new ScoreSchemeDto());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScoreSchemeDto model)
        {
            if (await _scoreSchemeRepo.getAnyAsync(u => u.Title == model.Title))
                return Json(_resp.error("已存在相同名称的评分规则"));
            return Json(_resp.success(await _scoreSchemeRepo.addItemAsync(model.Adapt<ScoreScheme>()),"创建成功"));
        }

        public async Task<IActionResult> Edit(long id)
        {
            if(!await _scoreSchemeRepo.getAnyAsync(u => u.Id.Equals(id)))
            {
                return BadRequest();
            }

            return View(
               ( await _scoreSchemeRepo.getOneAsync(u => u.Id.Equals(id))).Adapt<ScoreSchemeDto>()
                );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ScoreSchemeDto schemeDto)
        {
            if(await _scoreSchemeRepo.getAnyAsync(u=>u.Id!=schemeDto.Id && u.Title == schemeDto.Title))
            {
                return Json(_resp.error("已存在相同名称的评分规则"));
            }
            return Json(_resp.success(await _scoreSchemeRepo.updateItemAsync(schemeDto.Adapt<ScoreScheme>())));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            if(await _examinationRepo.getAnyAsync(u=>u.SchemeId==id && u.IsDeleted == 0))
            {
                return Json(_resp.error("考试列表存在绑定了该标准的记录，要删除当前评分标准的话，请先进行解绑操作"));
            }
            var scheme = await _scoreSchemeRepo.getOneAsync(u => u.Id.Equals(id));
            if (scheme == null)
            {
                return Json(_resp.error("标准不存在"));
            }
            if (scheme.Id == 0)
            {
                return Json(_resp.error("默认标准不可删除"));
            }
            scheme.IsDeleted = 1;
            return Json(_resp.success(await _scoreSchemeRepo.updateItemAsync(scheme)));
        }
    }
}
