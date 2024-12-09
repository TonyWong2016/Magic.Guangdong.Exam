using Coravel.Invocable;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using NPOI.HSSF.Record;
using System.Linq;

namespace Magic.Guangdong.Exam.AutoJobs.DelayTasks
{
    public class AutoChecking : IInvocable
    {
        private readonly IUserAnswerRecordRepo _userAnswerRecordRepo;
        private readonly IUserAnswerSubmitRecordRepo _userAnswerSubmitRecordRepo;
        private readonly IRedisCachingProvider _rediscachingProvider;
        private readonly ICertRepo _certRepo;
        private readonly IActivityRepo _activityRepo;
        public AutoChecking(IUserAnswerRecordRepo userAnswerRecordRepo,IUserAnswerSubmitRecordRepo userAnswerSubmitRecordRepo,ICertRepo certRepo,IRedisCachingProvider redisCachingProvider, IActivityRepo activityRepo)
        {
            _userAnswerRecordRepo = userAnswerRecordRepo;
            _userAnswerSubmitRecordRepo = userAnswerSubmitRecordRepo;
            _certRepo = certRepo;
            _activityRepo = activityRepo;
            _rediscachingProvider = redisCachingProvider;
        }

        public async Task Invoke()
        {
            
            await CheckMarking();

            await CheckCert();
        }

        private async Task CheckMarking()
        {
            await Task.Delay(new Random().Next(100, 5000));
            if (await _rediscachingProvider.KeyExistsAsync("autocheckscore"))
            {
                Assistant.Logger.Warning("自动检查成绩的服务已分配到其他终端");
                return;
            }
            await _rediscachingProvider.StringSetAsync("autocheckscore", DateTime.Now.ToString(), TimeSpan.FromSeconds(300));
            Assistant.Logger.Info("开始检查粗心大意交了卷没等成绩出来就走了的卷子");
            var noMarkingRecords = (await _userAnswerRecordRepo.getListAsync(
                u => u.IsDeleted == 0
                && (u.CreatedAt.Date == DateTime.Today || u.CreatedAt.Date == DateTime.Today.AddDays(-1))
                && (u.Complated == DbServices.Entities.ExamComplated.Yes || u.LimitedTime < DateTime.Now.AddMinutes(2))
                && u.ObjectiveScore == 0
                && u.Marked == DbServices.Entities.ExamMarked.No))
                .Select(u => new { u.Id, u.Complated, u.ComplatedMode, u.UserName, u.IdNumber })
                .ToList();

            if (noMarkingRecords.Count > 0)
            {
                Assistant.Logger.Warning($"还真有{noMarkingRecords.Count}张卷子");
                foreach (var record in noMarkingRecords)
                {
                    Assistant.Logger.Warning($"--urid:{record.Id},答题人:{record.UserName},证件号:{record.IdNumber}--");
                    await _userAnswerSubmitRecordRepo.ScoreObjectivePart(record.Id, (int)record.ComplatedMode);
                }
            }
        }

        private async Task CheckCert()
        {
            try
            {
                await Task.Delay(new Random().Next(100, 5000));
                if (await _rediscachingProvider.KeyExistsAsync("autoupdatecert"))
                {
                    Assistant.Logger.Warning("自动更新证书reportid的任务已分配到其他终端");
                    return;
                }
                await _rediscachingProvider.StringSetAsync("autoupdatecert", DateTime.Now.ToString(), TimeSpan.FromSeconds(300));

                Assistant.Logger.Info("自动检查并更新证书的reportid");
                var activityDrops = (await _activityRepo.GetActivityDrops()).Select(u => u.Id);
                var externalCerts = await _certRepo.getListAsync(u => u.IsExternal == 0
                && (!activityDrops.Contains(u.ActivityId) || u.ActivityId == 0 || u.ExamId == Guid.Empty)
                && u.IsDeleted == 0
                && u.UpdatedAt > DateTime.Now.AddDays(-2));
                if (externalCerts.Count > 0)
                {
                    externalCerts.ForEach(item =>
                    {
                        item.IsExternal = 1;
                        item.Remark = "外部证书";

                    });
                    await _certRepo.updateItemsAsync(externalCerts);
                }

                var selfCerts = (await _certRepo.getListAsync(u => u.IsExternal == 0
                && activityDrops.Contains(u.ActivityId)
                && u.IsDeleted == 0
                && u.ReportId == null
                && u.UpdatedAt > DateTime.Now.AddDays(-2)));
                if (selfCerts.Count == 0)
                {
                    Assistant.Logger.Warning("2天内生成或修改的证书没有需要更新的reportid");
                    return;
                }

                var idNumbers = selfCerts.Select(u => u.IdNumber)
               .ToArray();
                var kvs = await _userAnswerRecordRepo.GetReportIdsByIdNumber(idNumbers);
                if (kvs.Count == 0)
                {
                    Assistant.Logger.Warning("2天内生成或修改的证书没有需要更新的reportid");
                    return;
                }
                List<Cert> list = new List<Cert>();
                foreach (var item in kvs)
                {
                    var certs = selfCerts.Where(u => u.IdNumber == item.key).ToList();
                    if (certs == null)
                        continue;
                    foreach (var cert in certs)
                    {
                        cert.ReportId = Convert.ToInt64(item.value);
                        cert.UpdatedAt = DateTime.Now;
                        cert.Remark = "延时自动更新reportid";
                        list.Add(cert);
                    }
                }
                if (list.Count == 0)
                {
                    Assistant.Logger.Warning("2天内生成或修改的证书没有需要更新的reportid");
                    return;
                }
                if (list.Count > 0 && list.Count <= 500)
                {
                    await _certRepo.updateItemsAsync(list);

                }
                else if (list.Count > 500)
                {
                    var batchsize = 500;
                    for (int i = 0; i < list.Count; i += batchsize)
                    {
                        int count = Math.Min(batchsize, list.Count - i);
                        var batch = list.GetRange(i, count);

                        // 批量更新逻辑
                        await _certRepo.updateItemsAsync(batch);
                    }
                }
                Assistant.Logger.Info($"证书延时自动更新reportId完成，共计更新了{list.Count}条记录");
            }
            catch (Exception ex) {
                Assistant.Logger.Error("自动更新证书失败" + ex.Message);

                await EmailKitHelper.SendEMailToDevMsgAsync("自动更新证书失败" + ex.Message);
            }
        }
    }
}
