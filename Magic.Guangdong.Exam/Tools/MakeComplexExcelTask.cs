using Coravel.Invocable;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Assistant;
using MimeKit;
using Magic.Guangdong.DbServices.Interfaces;

namespace Magic.Guangdong.Exam.Tools
{
    public class MakeComplexExcelTask:IInvocable
    {
        private IReportInfoRepo _reportInfoRepo;
        private IAdminRepo _adminRepo;
        private INpoiExcelOperationService _npoi;
        private readonly IWebHostEnvironment _en;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="applyRepo"></param>
        public MakeComplexExcelTask(IReportInfoRepo reportRepo, IAdminRepo adminRepo, INpoiExcelOperationService npoi, IWebHostEnvironment en)
        {
            _reportInfoRepo = reportRepo;
            _adminRepo = adminRepo;
            _npoi = npoi;
            _en = en;
        }

        /// <inheritdoc/>
        public async Task Invoke()
        {
            if (await RedisHelper.HLenAsync("MakeComplexExcelTask") == 0)
            {
                Console.WriteLine($"{DateTime.Now},无excel生成任务");
                await Logger.writeLogToRedis("无excel生成任务", "info", "magicExam");
                return;
            }
            var rets = RedisHelper.HGetAll("MakeComplexExcelTask");
            string msg = $"{DateTime.Now},excel生成任务启动,共计{rets.Count}个excel文件待生成";
            Console.WriteLine(msg);
            Logger.Warning(msg);
            await Logger.writeLogToRedis(msg, "info","magicExam");
            //todo...
           
        }
    }
}
