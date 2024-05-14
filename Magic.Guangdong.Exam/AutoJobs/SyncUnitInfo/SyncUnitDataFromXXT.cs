using Coravel.Invocable;
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
        public SyncUnitDataFromXXT(IUnitInfoRepo unitInfoRepo,ISyncRecordRepo syncRecordRepo)
        {
            _unitInfoRepo = unitInfoRepo;
            _syncRecordRepo = syncRecordRepo;
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
                string responseStr = await RestHelper.Get(
                    new RestParams()
                    {
                        Url = "https://www.xiaoxiaotong.org/api/Article/GetRecord",
                        UrlParms = new Dictionary<string, string>
                        {
                            { "editdatetime", DateTime.Now.AddDays(-30).ToString() }
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
                var localCnt = await _unitInfoRepo.getCountAsync(u => u.OriginNo != "0");
                if (countResp.result.allcount > localCnt)
                {
                    int needSyncCnt = countResp.result.allcount - Convert.ToInt32(localCnt);
                    Logger.Warning($"发现待同步数据，共计{countResp.result.allcount}条,待同步{needSyncCnt}条");
                    await TransformData(needSyncCnt);
                }
            }
            catch (Exception ex)
            {
                Assistant.Logger.Error(ex.Message);
            }
        }



        internal async Task TransformData(int total)
        {
            int perLimit = 100;//每次传100条记录
            int planTime = Convert.ToInt32(Math.Floor((total * 1.0) / (perLimit * 1.0)));
            Logger.Warning($"需要下载{planTime}次数据");
            int lastSyncTimes = await _syncRecordRepo.GetLastRecordByPlatform("xxt");

            for (int i = lastSyncTimes; i < planTime; i++)
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
                //await _unitInfoRepo.addItemsAsync(listResp.result.desModels);

                //await _syncRecordRepo.addItemAsync(new SyncRecord()
                //{
                //    Platform = "xxt",
                //    TargetModel = "单位库",
                //    DestModel = "数据字典-单位库",
                //    Usage = "填充单位库数据",
                //    DataAmount = perLimit,
                //    Times = i+1,
                //    CreatedAt = DateTime.Now,
                //});
                await Task.Delay(1000 * 3);
            }
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
                            CityId=Convert.ToInt32(item.CityID),
                            ProvinceId = Convert.ToInt32(item.CityID.Substring(0,2)),
                            DistrictId = Convert.ToInt32(item.CountyID),
                            CreatedAt = DateTime.Now,
                            CreatedBy = "sync",
                            Fax = item.Fax,
                            LegalPerson="同步平台不存在该字段",
                            OrganizationCode = item.OrganizationCode,
                            
                            IsDeleted = Convert.ToInt32( item.IsDeleted),
                            PostCode = item.PostCode,
                            Telephone =item.Telephone,
                            UnitCaption = item.UnitCaption,
                            UnitIntroduction = item.UnitIntroduction.Length>4000? Utils.StripHTML(item.UnitIntroduction.Substring(0,4000)): Utils.StripHTML(item.UnitIntroduction),
                            UnitNo = item.UnitNo,
                            UnitType = (UnitType)Convert.ToInt32(item.UnitType),
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
        public string UnitID { get; set; }

        public string UnitNo { get; set; }

        public string UnitCaption { get; set; }

        public string UnitType { get; set; }

        public string CityID { get; set; }

        public string Address { get; set; }

        public string PostCode { get; set; }

        public string Telephone { get; set; }

        public string Fax { get; set; }

        public string AddDate { get; set; }

        public string StatusID { get; set; }

        public string CheckStatus { get; set; }

        public string UnitUrl { get; set; }

        public string UnitIntroduction { get; set; }

        public string CountyID { get; set; }

        public string OrganizationCode { get; set; }

        public string IsDeleted { get; set; }

        public string EditDate { get; set; }


    }
}
