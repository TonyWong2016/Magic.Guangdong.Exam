using Coravel.Invocable;
using DotNetCore.CAP;
using Essensoft.Paylink.Alipay.Request;
using Essensoft.Paylink.WeChatPay.V3.Domain;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Yitter.IdGenerator;

namespace Magic.Guangdong.Exam.AutoJobs.SyncUnitInfo
{
    public class SyncUnitDataFromXXT : IInvocable
    {
        private readonly IUnitInfoRepo _unitInfoRepo;
        private readonly ISyncRecordRepo _syncRecordRepo;
        private readonly ICapPublisher _capPublisher;
        public SyncUnitDataFromXXT(IUnitInfoRepo unitInfoRepo,ISyncRecordRepo syncRecordRepo,ICapPublisher capPublisher)
        {
            _unitInfoRepo = unitInfoRepo;
            _syncRecordRepo = syncRecordRepo;
            _capPublisher = capPublisher;
        }
        public async Task Invoke()
        {
            Logger.Debug("获取大库单位库数据记录" + DateTime.Now);
            await GetRecord();
        }

        internal async Task GetRecord()
        {
            try
            {
                var client = new HttpClient();
                DateTime fromDate = DateTime.Now.AddDays(-30);
                string responseStr = await RestHelper.Get(
                    new RestParams()
                    {
                        Url = "https://www.xiaoxiaotong.org/api/Article/GetRecord",
                        UrlParms = new Dictionary<string, string>
                        {
                            { "editdatetime", fromDate.ToString() }
                        },
                        Headers = new Dictionary<string, string>
                        {
                            { "Token", "gdjx#7W5ojU" }
                        }
                    }
                );

                Logger.Debug(responseStr);
                var countResp = System.Text.Json.JsonSerializer.Deserialize<RecordCountResponseModel>(responseStr);

                string code = countResp.code;
                if (code != "0")
                {
                    return;
                }
                var localCnt = await _unitInfoRepo.getCountAsync(u => u.OriginNo != "0" && u.CreatedAt>= fromDate);
                if (countResp.result.allcount > localCnt)
                {
                    int needSyncCnt = countResp.result.allcount - Convert.ToInt32(localCnt);
                    Logger.Warning($"发现待同步数据，共计{countResp.result.allcount}条,待同步{needSyncCnt}条");
                    await TransformData(needSyncCnt);
                }
                else
                {
                    Logger.Info("没有需要同步的数据..." + DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        internal async Task TransformData(int total)
        {
            if(!await _syncRecordRepo.getAnyAsync(u => u.Platform == "xxt"))
            {
                await TransformDataFirst(total);
                return;
            }
            int perLimit = 100;//每次传100条记录
            if (total < perLimit)
                perLimit = total;
            int planTime = Convert.ToInt32(Math.Floor((total * 1.0) / (perLimit * 1.0)));
            Logger.Warning($"需要下载{planTime}次数据");
            int lastSyncTimes = await _syncRecordRepo.GetLastRecordByPlatform("xxt");
            ////int total = planTime + lastSyncTimes;
            int slidePlanTime = planTime + lastSyncTimes;//这里要把起点滑动到首次下载的次数
            for (int i = 1; i < planTime+1; i++)
            {
                Logger.Warning($"正在下载第{i + 1}次数据..." + DateTime.Now);

                string responseStr = await RestHelper.Get(
                    new RestParams()
                    {
                        Url = "https://www.xiaoxiaotong.org/api/Article/GetList",
                        UrlParms = new Dictionary<string, string>
                        {
                            { "editdatetime", DateTime.Now.AddDays(-30).ToString() },
                            { "pageindex", i.ToString() },
                            { "pagesize", perLimit.ToString() }
                        },
                        Headers = new Dictionary<string, string>
                        {
                            { "Token", "gdjx#7W5ojU" }
                        }
                    }
                );
                Logger.Info(responseStr);
                var listResp = JsonHelper.JsonDeserialize<RecordListResponseModel>(responseStr);
                await _unitInfoRepo.SyncUnitInfos(listResp.result.desModels);

                await Task.Delay(3000);
            }

            //await _capPublisher.PublishAsync
            await EmailKitHelper.SendEMailToManagerMsgAsync($"第{slidePlanTime}次单位库数据自动同步完成，此次同步了{total}条记录");
        }

        //首次同步，单独调用下
        internal async Task TransformDataFirst(int total)
        {
            int perLimit = 100;//每次传100条记录
            if(total<perLimit)
                perLimit = total;
            int planTime = Convert.ToInt32(Math.Floor((total * 1.0) / (perLimit * 1.0)));
            Logger.Warning($"需要下载{planTime}次数据");
            int lastSyncTimes = await _syncRecordRepo.GetLastRecordByPlatform("xxt");
            //int total = planTime + lastSyncTimes;
            int slidePlanTime = planTime + lastSyncTimes;//这里要把起点滑动到首次下载的次数
            for (int i = lastSyncTimes; i < slidePlanTime; i++)
            {
                Logger.Warning($"正在下载第{i+1}次数据..." + DateTime.Now);

                string responseStr = await RestHelper.Get(
                    new RestParams()
                    {
                        Url = "https://www.xiaoxiaotong.org/api/Article/GetList",
                        UrlParms = new Dictionary<string, string>
                        {
                            { "editdatetime", DateTime.Now.AddDays(-30).ToString() },
                            { "pageindex", i.ToString() },
                            { "pagesize", perLimit.ToString() }
                        },
                        Headers = new Dictionary<string, string>
                        {
                            { "Token", "gdjx#7W5ojU" }
                        }
                    }
                );
                Logger.Info( responseStr );
                var listResp = JsonHelper.JsonDeserialize<RecordListResponseModel>(responseStr);
                await _unitInfoRepo.SyncUnitInfos(listResp.result.desModels);
                
                await Task.Delay(3000);
            }

            //await _capPublisher.PublishAsync
            await EmailKitHelper.SendEMailToManagerMsgAsync($"第{slidePlanTime}次单位库数据自动同步完成，此次同步了{total}条记录");
        }
    }

    internal class RecordCountResponseModel()
    {
        public string code { get; set; }

        public string msg { get; set; }

        public RecordResult result { get; set; }
    }

    internal class RecordResult
    {
        public int allcount { get; set; }
    }

    internal class RecordListResponseModel()
    {
        public string code { get; set; }

        public string msg { get; set; }

        public ListResult result { get; set; }
    }

    internal class ListResult
    {
        public string editdate { get; set; }

        public string pageindex { get; set; }

        public string pagecount { get; set; }

        public string allcount { get; set; }

        public List<UnitListModel> alist { get; set; }

        public List<UnitInfo> desModels
        {
            get
            {
                List<UnitInfo> ret = new List<UnitInfo>();
                if (alist.Any())
                {
                    foreach (var item in alist)
                    {
                        ret.Add(new UnitInfo()
                        {
                            Id = YitIdHelper.NextId(),
                            Status = item.StatusID=="1"?0:1,
                            UnitStatus = BusinessStatus.Operational,
                            Address = item.Address,
                            CityId= string.IsNullOrEmpty(item.CityID)?0: Convert.ToInt32(item.CityID),
                            ProvinceId = string.IsNullOrEmpty(item.CityID) ? 0:Convert.ToInt32(item.CityID.Substring(0,2)),
                            DistrictId = string.IsNullOrEmpty(item.CountyID) ? 0: Convert.ToInt32(item.CountyID),
                            CreatedAt = DateTime.Now,
                            CreatedBy = "sync",
                            Fax = item.Fax,
                            LegalPerson="同步平台不存在该字段",
                            OrganizationCode = item.OrganizationCode,
                            
                            IsDeleted = string.IsNullOrEmpty(item.IsDeleted)?1: Convert.ToInt32(item.IsDeleted),
                            PostCode = item.PostCode,
                            Telephone =item.Telephone,
                            UnitCaption = item.UnitCaption,
                            UnitIntroduction = item.UnitIntroduction.Length>4000? Utils.StripHTML(item.UnitIntroduction.Substring(0,4000)): Utils.StripHTML(item.UnitIntroduction),
                            //UnitIntroduction = item.UnitIntroduction.Replace("\"","'").Replace(".","。"),
                            UnitNo = item.UnitNo,
                            UnitType = string.IsNullOrEmpty(item.UnitType)?UnitType.Other: (UnitType)Convert.ToInt32(item.UnitType),
                            UnitUrl = item.UnitUrl,
                            UpdatedAt = DateTime.Now,
                            OriginNo = item.UnitID,
                        });
                    }
                }
                return ret;
            }
        }
    }

    internal class UnitListModel
    {
        public string UnitID { get; set; } = "";

        public string UnitNo { get; set; } = "";

        public string UnitCaption { get; set; } = "";

        public string UnitType { get; set; } = "";

        public string CityID { get; set; } = "";

        public string Address { get; set; } = "";

        public string PostCode { get; set; } = "";

        public string Telephone { get; set; } = "";

        public string Fax { get; set; } = "";

        public string AddDate { get; set; } = "";

        public string StatusID { get; set; } = "";

        public string CheckStatus { get; set; } = "";

        public string UnitUrl { get; set; } = "";

        public string UnitIntroduction { get; set; } = "";

        public string CountyID { get; set; } = "";

        public string OrganizationCode { get; set; } = "";

        public string IsDeleted { get; set; } = "";

        public string EditDate { get; set; } = "";


    }
}
