using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Yitter.IdGenerator;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    /// <summary>
    /// 模拟考试行为相关
    /// </summary>
    public class SimulateController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IExaminationClientRepo _examinationClientRepo;
        private readonly ICapPublisher _capPublisher;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly ISimulation1Repo _simulation1Repo;
        private readonly ISimulation2Repo _simulation2Repo;
        public SimulateController(IResponseHelper resp, IExaminationClientRepo examinationClientRepo, ICapPublisher capPublisher, IRedisCachingProvider redisCachingProvider, ISimulation1Repo simulation1, ISimulation2Repo simulation2)
        {
            _resp = resp;
            _examinationClientRepo = examinationClientRepo;
            _capPublisher = capPublisher;
            _redisCachingProvider = redisCachingProvider;
            _simulation1Repo = simulation1;
            _simulation2Repo = simulation2;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 模拟获取考试信息
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 100,VaryByQueryKeys = ["examId"])]
        public async Task<IActionResult> GetExamInfo(Guid examId)
        {
            return Json(await _examinationClientRepo.GetExamsForClient(new OnlyGetExamDto()
            {
                examId = examId,
            }));
        }

        /// <summary>
        /// 模拟验证报名信息
        /// 测试时准备多个正确和错误的身份证号以及准考证号，进行验证
        /// </summary>
        /// <param name="idNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VerifyInfo()
        {
            try
            {
                string[] idNumbers = ["53081732180510DD7DGX", "43161732180511DD7D52", "81031732180511DD7DVK"];
                Random rd = new Random();
                var result = await _examinationClientRepo.InfoVerificationByNumber(new OnlyGetExamDto
                {
                    examId= Guid.Parse("0C140000-569B-0050-DD7D-08DD09FF5460"),
                    reportNumber= idNumbers[rd.Next(0,2)],
                });
                
                return Json(_resp.ret(result.verifyCode, result.verifyMsg, result));
            }
            catch (Exception ex)
            {
                Assistant.Logger.Error(ex);
                return Json(_resp.error("认证服务异常"));
            }
        }


        /// <summary>
        /// 模拟保存草稿
        /// </summary>
        /// <param name="answer">提交一个长字符传</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveDraft(SimulateSaveDto dto)
        {
            if (string.IsNullOrEmpty(dto.answer))
            {
                dto.answer = Utils.GenerateRandomCodeFast(3000);
            }
            if (dto.sid == 0)
            {
                dto.sid = YitIdHelper.NextId();
                await _simulation1Repo.addItemAsync(new DbServices.Entities.Simulation1()
                {
                    Id = dto.sid,
                    SubmitAnswer = dto.answer,
                });
            }
            else
            {
                Logger.Warning($"{DateTime.Now}:发布事务---模拟提交答案");

                await _capPublisher.PublishAsync(CapConsts.ClientPrefix + "SimulateSaveDraft", dto);

            }

            return Json(_resp.success(dto.sid));
        }

        [NonAction]
        [CapSubscribe(CapConsts.ClientPrefix + "SimulateSaveDraft")]
        public async Task SimulateSaveDraft(SimulateSaveDto dto, [FromCap] CapHeader header)
        {
            Logger.Warning($"{DateTime.Now}:消费事务---模拟保存答案");
            //Logger.Warning(System.Text.Json.JsonSerializer.Serialize(header));
            await Task.Run(async () =>
            {
                if (await _simulation1Repo.getAnyAsync(u => u.Id == dto.sid))
                {
                    await _simulation2Repo.addItemAsync(new DbServices.Entities.Simulation2()
                    {
                        Sid = dto.sid,
                        SubmitAnswer = dto.answer,
                    });

                }
                await _simulation1Repo.insertOrUpdateAsync(new DbServices.Entities.Simulation1()
                {
                    Id=dto.sid,
                    SubmitAnswer = dto.answer,
                });
            });
        }
    }

    public class SimulateSaveDto
    {
        public string answer { get; set; }
    
        public long sid { get; set; } = YitIdHelper.NextId();
    }

}
