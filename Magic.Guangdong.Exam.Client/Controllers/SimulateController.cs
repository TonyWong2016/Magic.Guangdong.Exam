using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
            return Content("hi");
        }

        /// <summary>
        /// 模拟获取考试信息
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 100,VaryByQueryKeys = ["examId"])]
        public async Task<IActionResult> GetExamInfo(Guid examId)
        {
            //模拟header验证耗时
            await Task.Delay(new Random().Next(10, 300));
            var ret = await _examinationClientRepo.GetExamsForClient(new OnlyGetExamDto()
            {
                examId = examId
            });
            return Json(_resp.success(ret));
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
                var exams = await _examinationClientRepo.getListAsync(u => u.IsDeleted == 0);
                string[] idNumbers = [
                    "53081732180510DD7DGX",
                    "43161732180511DD7D52",
                    "81031732180511DD7DVK"
                ];
                if (exams.Count == 0)
                {
                    return Json(_resp.success(0, "没有可用的考试数据"));
                }
                var exam = new Examination();

                if (exams.Count > 1)
                    exam = exams[new Random().Next(0, exams.Count - 1)];
                else
                    exam = exams.FirstOrDefault();
                int rd = new Random().Next(0, 2);
                var result = await _examinationClientRepo.InfoVerificationByNumber(new OnlyGetExamDto()
                {
                    examId = exam.Id,
                    reportNumber = idNumbers[rd],
                });

                //return Json(_resp.ret(result.verifyCode, result.verifyMsg, result));
                return Json(_resp.success(result, result.verifyMsg));
            }
            catch (Exception ex)
            {
                Assistant.Logger.Error(ex);
                return Json(_resp.error("认证服务异常"));
            }
        }

        public async Task<IActionResult> InitSimulationData()
        {
            if(await _simulation1Repo.getAnyAsync(u => u.Id > 0))
            {
                await _simulation1Repo.delItemAsync(u => u.Id > 0);
                await _simulation2Repo.delItemAsync(u => u.Id > 0);
            }
            List<Simulation1> list = new List<Simulation1>();
            for (int i = 0; i < 101; i++)
            {
                list.Add(new Simulation1()
                    {
                        Id = YitIdHelper.NextId(),
                        SubmitAnswer = "测试备用",
                    }
                );
            }
            await _simulation1Repo.addItemsAsync(list);
            return Json(_resp.success(101));
            
        }

        /// <summary>
        /// 模拟保存草稿
        /// </summary>
        /// <param name="answer">提交一个长字符传</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveDraft(SimulateSaveDto dto)
        {
            
            int rd = new Random().Next(0, 100);

            var randomOne = (await _simulation1Repo.getListAsync(u => u.Id > 0))[rd];
            dto.sid=randomOne.Id;
            dto.answer = Utils.GenerateRandomCodeFast(new Random().Next(2, 10000));
            await _capPublisher.PublishAsync(CapConsts.ClientPrefix + "SimulateSaveDraft", dto);

            Logger.Warning($"{DateTime.Now}:发布事务---模拟提交答案");
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
                        CapMsgId = header["cap-msg-id"]??"",
                        CapInstance = header["cap-exec-instance-id"]??"",
                        CapSenttime = header["cap-senttime"] ?? ""
                    });

                }
                await _simulation1Repo.insertOrUpdateAsync(new DbServices.Entities.Simulation1()
                {
                    Id = dto.sid,
                    SubmitAnswer = dto.answer,
                    UpdatedAt = DateTime.Now
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
