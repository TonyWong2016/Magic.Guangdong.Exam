using BaiduBce.Services.Bos.Model;
using FreeSql.Internal.Model;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class ReportInfoRepo : ExaminationRepository<ReportInfo>, IReportInfoRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public ReportInfoRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        /// <summary>
        /// 报名参与活动
        /// 需要注意几个点
        /// 1.先创建报名记录(ReportInfo)，这个只有个人信息
        /// 2.创建报名进度记录(ReportProcess)，这个包含了报名记录和考试，订单，审核记录等关联的模型)，包含了报名记录和考试，订单，审核记录等关联的模型，
        /// 审核状态，报名进度都是默认值，
        /// 如果考试本身设定了不需要审核，则需要将审核状态的值调整为已审核，同时增加一条审核记录（ReportCheckHistory）
        /// 3.创建订单(Order)
        /// 如果考试本身设定了不需要交费，则要将缴费状态设置为已缴费，同时将2中的报名进度修改为已缴费
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> ReportActivity(ReportInfoDto dto)
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var reportInfoRepo = fsql.Get(conn_str).GetRepository<ReportInfo>();
                    reportInfoRepo.UnitOfWork = uow;
                    var reportModel = dto.Adapt<ReportInfo>();
                    //准考证号：身份证后4位+10位时间戳+考试id4位+随机字符2位（如果考试id为空，则随机字符为6位）
                    reportModel.ReportNumber =
                        reportModel.IdCard.Substring(reportModel.IdCard.Length - 4, 4) +
                        Utils.DateTimeToTimeStamp(DateTime.Now) +
                       (reportModel.ExamId == null ? Utils.GenerateRandomCodePro(6) : reportModel.ExamId.ToString().Substring(19, 4).ToUpper() + Utils.GenerateRandomCodePro(2));
                    if (reportModel.IsSecurity == 0)
                    {
                        reportModel = GetReportSecurityModel(reportModel);
                    }
                    await reportInfoRepo.InsertAsync(reportModel);

                    var reportProcessRepo = fsql.Get(conn_str).GetRepository<ReportProcess>();
                    reportProcessRepo.UnitOfWork = uow;
                    var reportProcessModel = new ReportProcess()
                    {
                        AccountId = dto.AccountId,
                        ExamId = dto.ExamId,
                        ReportId = dto.Id,
                        ActivityId = dto.ActivityId,
                        Status = ReportStatus.UnChecked,
                        Step = ReportStep.Reported,
                    };
                    //await reportProcessRepo.InsertAsync(ReportProcessModel);

                    var examRepo = fsql.Get(conn_str).GetRepository<Examination>();
                    examRepo.UnitOfWork = uow;
                    var exam = await examRepo.Where(x => x.Id == dto.ExamId).FirstAsync();
                    var orderRepo = fsql.Get(conn_str).GetRepository<Order>();
                    orderRepo.UnitOfWork = uow;
                    var order = new Order()
                    {
                        AccountId = dto.AccountId,
                        //ExamId = dto.ExamId,
                        ReportId = reportModel.Id,
                        Expenses = exam.Expenses,
                        //交易单号32位，考试id的4位（21-24）+13位时间戳+15位随机字符
                        //OutTradeNo = $"{exam.Id.ToString().Substring(19, 4).ToUpper()}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{Assistant.Utils.GenerateRandomCodePro(15)}",
                        OutTradeNo = dto.OrderTradeNumber,
                        Subject = $"{exam.AssociationTitle}，{exam.Title}",
                        InvoiceId = 0
                    };
                    reportProcessModel.OrderId = order.Id;
                    if (exam.Expenses == 0)
                    {
                        order.Status = OrderStatus.Paid;//考试不需要交费的话，报名的同时就将缴费状态设置为已缴费
                        reportProcessModel.Step = ReportStep.Paied;
                        order.PayType = PayType.None;
                    }
                    //else
                    //{
                    //    await RedisHelper.HSetAsync("unPaidOrder", dto.OrderTradeNumber, dto.Id);
                    //    await RedisHelper.ExpireAtAsync("unPaidOrder", exam.EndTime);
                    //}
                    //如果考试本身设定了不需要审核
                    if (exam.Audit == ExamAudit.No)
                    {
                        reportProcessModel.Status = ReportStatus.Succeed;
                        var reportHistoryRepo = fsql.Get(conn_str).GetRepository<ReportCheckHistory>();
                        reportHistoryRepo.UnitOfWork = uow;
                        await reportHistoryRepo.InsertAsync(new ReportCheckHistory()
                        {
                            AdminId = "nobody",
                            CheckRemark = "本场考试报名无需审核，报名即通过.",
                            CheckStatus = CheckStatus.Passed,
                            ReportId = reportModel.Id,
                            CreatedAt = DateTime.Now
                        });
                        
                    }
                    await orderRepo.InsertAsync(order);
                    await reportProcessRepo.InsertAsync(reportProcessModel);
                    uow.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Assistant.Logger.Error(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// 克隆一份报名信息，主要作用是
        /// 当用户报名了某一场考试活动后，如果该厂考试下面包含对应的练习模式
        /// 则克隆考试的报名信息，对应到练习上面
        /// </summary>
        /// <returns></returns>
        //public async Task<bool> CloneReportInfo(long reportId)
        //{
        //    return false;
        //}
        public dynamic GetReportInfos(PageDto pageDto, out long total)
        {
            var query = fsql.Get(conn_str).Select<ReportInfoView, Order, UserBase>()
                .LeftJoin((a, b, c) => a.Id == b.ReportId)
                .LeftJoin((a, b, c) => a.AccountId == c.AccountId);
            if (!string.IsNullOrEmpty(pageDto.whereJsonStr))
            {
                DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(pageDto.whereJsonStr);
                query = query
                    .WhereDynamicFilter(dyfilter);
            }
            return query
                .Count(out total)
                .OrderByPropertyNameIf(!string.IsNullOrEmpty(pageDto.orderby), pageDto.orderby, pageDto.isAsc)
                .Page(pageDto.pageindex, pageDto.pagesize)
                .ToList((a, b, c) => new
                {
                    a.Id,
                    a.AccountId,
                    a.Name,
                    //IdCard = a.IdCard.Length > 4 ? (a.IdCard.Substring(0, 2) + "****************" + a.IdCard.Substring(a.IdCard.Length - 4, 4)) : a.IdCard,
                    a.IdCard,
                    a.Email,
                    a.Mobile,
                    a.Job,
                    //area = a.ProvinceName+a.CityName+(string.IsNullOrEmpty(a.DistrictName)?"": a.DistrictName),
                    a.ProvinceName,
                    a.CityName,
                    a.DistrictName,
                    a.ReportNumber,
                    a.CreatedAt,
                    a.ReportStatus,
                    a.ExamId,
                    a.ActivityId,
                    b.Subject,
                    a.SecurityIdCard,
                    a.SecurityMobile,
                    OrderStatus = b.Status,
                    accountName = c.Name
                });
        }

        /// <summary>
        /// 审查报名状态
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> CheckReportInfo(ReportCheckHistoryDto dto)
        {
            if (dto.reportIds == null || dto.reportIds.Length == 0)
            {
                return false;
            }
            var reportProcessRepo = fsql.Get(conn_str).GetRepository<ReportProcess>();
           
            if(await reportProcessRepo.Where(u=>dto.reportIds.Contains(u.ReportId)).CountAsync() != dto.reportIds.Length)
            {
                return false;
            }
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    reportProcessRepo.UnitOfWork = uow;
                    var reportCheckHistoryRepo = fsql.Get(conn_str).GetRepository<ReportCheckHistory>();
                    reportCheckHistoryRepo.UnitOfWork = uow;
                    List<ReportCheckHistory> listHistory = new List<ReportCheckHistory>(dto.reportIds.Length);
                    List<ReportProcess> listProcess = new List<ReportProcess>(dto.reportIds.Length);
                    foreach (var reportId in dto.reportIds)
                    {
                        var reportProcess = await reportProcessRepo.Where(x => x.ReportId == reportId).FirstAsync();
                        reportProcess.Status = dto.reportStatus;
                        reportProcess.UpdatedAt = DateTime.Now;
                        listProcess.Add(reportProcess);
                        listHistory.Add(new ReportCheckHistory()
                        {
                            AdminId = dto.adminId.ToString(),
                            ReportId = reportId,
                            CheckStatus = dto.checkStatus,
                            CheckRemark = dto.checkRemark
                        });

                    }
                    await reportProcessRepo.UpdateAsync(listProcess);
                    await reportCheckHistoryRepo.InsertAsync(listHistory);
                    
                    uow.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Assistant.Logger.Error(ex);
                    return false;
                }
            }
        } 

        /// <summary>
        /// 导出报名列表
        /// </summary>
        /// <param name="whereJsonStr"></param>
        /// <returns></returns>
        public async Task<List<ExportReportInfo>> GetReportInfosForExcel(string whereJsonStr)
        {
            var query = fsql.Get(conn_str).Select<ReportInfoView, Order>()
               .LeftJoin((a, b) => a.Id == b.ReportId);
            if (!string.IsNullOrEmpty(whereJsonStr))
            {
                DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(whereJsonStr);
                query = query
                    .WhereDynamicFilter(dyfilter);
            }
            var list =  await query.ToListAsync((a, b) => new 
            {
                a.Id,
                a.AccountId,
                a.Name,
                IdCard = a.IdCard.Length > 4 ? (a.IdCard.Substring(0, 2) + "****************" + a.IdCard.Substring(a.IdCard.Length - 4, 4)) : a.IdCard,
                a.Email,
                a.Mobile,
                a.Job,
                //area = a.ProvinceName+a.CityName+(string.IsNullOrEmpty(a.DistrictName)?"": a.DistrictName),
                a.ProvinceName,
                a.CityName,
                a.DistrictName,
                a.ReportNumber,
                a.ReportStatus,
                b.Subject,
                orderStatus = b.Status,
                b.RefundNo,
                b.TradeNo,
                a.SecurityIdCard,
                a.SecurityMobile,
                //a.HashIdCard,
                //a.HashMobile
                //b.ExamId,
                //b.ActivityId
            });

            return list.Adapt<List<ExportReportInfo>>();
        }

        /// <summary>
        /// 客户端获取报名列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ReportOrderList> GetReportOrderListClient(GetReportListDto dto)
        {
            ReportOrderList reportOrderList = new ReportOrderList();
            var reportOrderRepo = fsql.Get(conn_str).GetRepository<ReportOrderView>();
            var query = reportOrderRepo
                .WhereIf(!string.IsNullOrEmpty(dto.AccountId), u => u.AccountId == dto.AccountId)
                .WhereIf(dto.ActivityId > 0, u => u.ActivityId == dto.ActivityId)
                .WhereIf(dto.ExamId != null, u => u.ExamId == dto.ExamId)
                .WhereIf(dto.OrderId != null, u => u.OrderId == dto.OrderId)
                .Where(u => u.ExamType == (int)ExamType.Examination);
            reportOrderList.total = await query.CountAsync();
            if (reportOrderList.total > 0)
            {
                reportOrderList.items = await query
                    .Page(dto.pageIndex, dto.pageSize)
                    .OrderBy(u => u.ReportStatus)
                    .OrderByDescending(u=>u.OrderCreatedAt)
                    .ToListAsync();
            }
            return reportOrderList;
        }

        /// <summary>
        /// 通过订单号获取报名详情
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public async Task<dynamic> GetReportDetailByOutTradeNoForClient(string outTradeNo)
        {
            var order = await fsql.Get(conn_str)
                .Select<Order>()
                .Where(u => u.OutTradeNo == outTradeNo)
                .FirstAsync();
            return await GetReportDetailForClient(order.ReportId);
        }

        /// <summary>
        /// 获取报名详情
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public async Task<dynamic> GetReportDetailForClient(long reportId)
        {
            var reportInfo = await fsql.Get(conn_str).Select<ReportExamView>()
                .Where(a => a.ReportId == reportId)
                .ToOneAsync();

            var attachmentInfo = await fsql.Get(conn_str).Select<Examination>()
                .Where(a => a.AttachmentId == reportInfo.ExamId)
                .ToOneAsync(
                u => new
                {
                    u.Title,
                    practiceId = u.Id,
                    practiceTitle = u.Title,
                    
                });

            var lastCheckHistory = await fsql.Get(conn_str).Select<ReportCheckHistory>()
                .Where(a => a.ReportId == reportId)
                .OrderByDescending(a => a.Id)                
                .ToOneAsync(u => new
                {
                    u.Id,
                    u.CheckRemark,
                    u.CreatedAt,
                    u.CheckStatus,
                    //审核记录里只可能有通过和不通过，没有待审
                    checkResult = u.CheckStatus == 0 ? "已通过" : "未通过"
                }) ;
            return new
            {
                reportInfo,
                lastCheckHistory,
                attachmentInfo
            };

        }

        /// <summary>
        /// 安全插入报名数据
        /// </summary>
        /// <param name="reportInfo"></param>
        /// <returns></returns>
        public async Task<bool> InsertReportSecurity(ReportInfo reportInfo)
        {
            if (reportInfo == null)
            {
                return false;
            }
            var maskIdCard = new MaskDataDto();
            string keyId = reportInfo.ReportNumber.Substring(0, 16);
            string keySecret = reportInfo.ReportNumber.Substring(reportInfo.ReportNumber.Length - 16, 16);
           
            maskIdCard.keyId = keyId;
            maskIdCard.keySecret = keySecret;
            maskIdCard.text = reportInfo.IdCard.Trim();
            if (reportInfo.CardType == 0)
            {
                maskIdCard.maskDataType = MaskDataType.ChinaIdCard;
            }
            else
            {
                maskIdCard.maskDataType = MaskDataType.Other;
            }
            if (maskIdCard.valid)
            {
                reportInfo.PrefixIdcard = maskIdCard.splitTexts[0];
                reportInfo.SuffixIdcard = maskIdCard.splitTexts[1];
                reportInfo.IdCard = maskIdCard.encryptText;
                reportInfo.HashIdcard = maskIdCard.hashText;
            }
            else
            {
                return false;
            }

            var maskPhone = new MaskDataDto();
            maskPhone.keyId = keyId;
            maskPhone.keySecret = keySecret;
            maskPhone.text = reportInfo.Mobile.Trim();
            if (reportInfo.CardType == 0)
            {
                maskPhone.maskDataType = MaskDataType.ChinaCellPhone;
            }
            else
            {
                maskPhone.maskDataType = MaskDataType.Other;
            }

            if (maskPhone.valid)
            {
                reportInfo.PrefixMobile = maskPhone.splitTexts[0];
                reportInfo.SuffixMobile = maskPhone.splitTexts[1];
                reportInfo.Mobile = maskPhone.encryptText;
                reportInfo.HashMobile = maskPhone.hashText;
            }
            else
            {
                return false;
            }
            return await fsql.Get(conn_str).Insert(reportInfo).ExecuteAffrowsAsync() == 1;
        }

        public async Task<int> MaskReportInfoData()
        {
            var reportRepo = fsql.Get(conn_str).GetRepository<ReportInfo>();
            var reportInfos = await reportRepo.Where(u => u.IsDeleted == 0 && u.IsSecurity == 0).ToListAsync();
            List<ReportInfo> MaskReportInfoList = new List<ReportInfo>();
            int count = 0;
            foreach (var reportInfo in reportInfos) {
                MaskReportInfoList.Add(GetReportSecurityModel(reportInfo));
                if (MaskReportInfoList.Count % 100 == 0)
                {
                    count += await reportRepo.UpdateAsync(MaskReportInfoList);
                    
                }
            }
            if (MaskReportInfoList.Count > 0)
            {
                count += await reportRepo.UpdateAsync(MaskReportInfoList);
            }
            return count;
        }

        public ReportInfo GetReportSecurityModel(ReportInfo reportInfo)
        {

            var maskIdCard = new MaskDataDto();
            string keyId = reportInfo.ReportNumber.Substring(0, 16);
            string keySecret = reportInfo.ReportNumber.Substring(reportInfo.ReportNumber.Length - 16, 16);

            maskIdCard.keyId = keyId;
            maskIdCard.keySecret = keySecret;
            maskIdCard.text = reportInfo.IdCard.Trim();
            if (reportInfo.CardType == 0)
            {
                maskIdCard.maskDataType = MaskDataType.ChinaIdCard;
            }
            else
            {
                maskIdCard.maskDataType = MaskDataType.Other;
            }
            if (maskIdCard.valid)
            {
                reportInfo.PrefixIdcard = maskIdCard.splitTexts[0];
                reportInfo.SuffixIdcard = maskIdCard.splitTexts[1];
                reportInfo.IdCard = maskIdCard.encryptText;
                reportInfo.HashIdcard = maskIdCard.hashText;
            }

            var maskPhone = new MaskDataDto();
            maskPhone.keyId = keyId;
            maskPhone.keySecret = keySecret;
            maskPhone.text = reportInfo.Mobile.Trim();
            if (reportInfo.CardType == 0)
            {
                maskPhone.maskDataType = MaskDataType.ChinaCellPhone;
            }
            else
            {
                maskPhone.maskDataType = MaskDataType.Other;
            }

            if (maskPhone.valid)
            {
                reportInfo.PrefixMobile = maskPhone.splitTexts[0];
                reportInfo.SuffixMobile = maskPhone.splitTexts[1];
                reportInfo.Mobile = maskPhone.encryptText;
                reportInfo.HashMobile = maskPhone.hashText;
            }
            reportInfo.IsSecurity = 1;
            return reportInfo;
        }

        
    }

    
}
