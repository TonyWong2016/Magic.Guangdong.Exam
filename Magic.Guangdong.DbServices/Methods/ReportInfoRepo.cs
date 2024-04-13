using Essensoft.Paylink.WeChatPay.V2.Request;
using FreeSql.Internal.Model;
using log4net.Repository.Hierarchy;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using MathNet.Numerics.Distributions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<bool> ReportActivity(ReportInfoDto dto)
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var reportInfoRepo = fsql.Get(conn_str).GetRepository<ReportInfo>();
                    var reportModel = dto.Adapt<ReportInfo>();
                    //准考证号：身份证后4位+10位时间戳+考试id4位+随机字符2位（如果考试id为空，则随机字符为6位）
                    reportModel.ReportNumber =
                        reportModel.IdCard.Substring(reportModel.IdCard.Length - 4, 4) +
                        Utils.DateTimeToTimeStamp(DateTime.Now) +
                       (reportModel.ExamId == null ? Utils.GenerateRandomCodePro(6) : reportModel.ExamId.ToString().Substring(19, 4).ToUpper() + Utils.GenerateRandomCodePro(2));
                    
                    await reportInfoRepo.InsertAsync(reportModel);

                    var reportProcessRepo = fsql.Get(conn_str).GetRepository<ReportProcess>();
                    var ReportProcessModel = new ReportProcess()
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
                    var exam = await examRepo.Where(x => x.Id == dto.ExamId).FirstAsync();
                    if (exam.Expenses > 0)
                    {
                        var orderRepo = fsql.Get(conn_str).GetRepository<Order>();
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
                        await orderRepo.InsertAsync(order);
                        ReportProcessModel.OrderId = order.Id;
                    }

                    await reportProcessRepo.InsertAsync(ReportProcessModel);
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
        /// 获取报名列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ReportOrderList> GetReportOrderList(GetReportListDto dto)
        {
            ReportOrderList reportOrderList = new ReportOrderList();
            var reportOrderRepo = fsql.Get(conn_str).GetRepository<ReportOrderView>();
            var query = reportOrderRepo
                .WhereIf(!string.IsNullOrEmpty(dto.AccountId), u => u.AccountId == dto.AccountId)
                .WhereIf(dto.ActivityId > 0, u => u.ActivityId == dto.ActivityId)
                .WhereIf(dto.ExamId != null, u => u.ExamId == dto.ExamId)
                .WhereIf(dto.OrderId != null, u => u.OrderId == dto.OrderId);
            reportOrderList.total = await query.CountAsync();
            if (reportOrderList.total > 0)
            {
                reportOrderList.items = await query
                    .Page(dto.pageIndex, dto.pageSize)
                    .OrderByDescending(u => u.ReportStatus)
                    .ToListAsync();
            }
            return reportOrderList;
        }
    
        /// <summary>
        /// 获取报名详情
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public async Task<dynamic> GetReportDetailByOutTradeNo(string outTradeNo)
        {
            var order = await fsql.Get(conn_str)
                .Select<Order>()
                .Where(u => u.OutTradeNo == outTradeNo)
                .FirstAsync();

            var detail = await fsql.Get(conn_str)
                .Select<ReportInfo, Examination>()
                .LeftJoin((a, b) => a.ExamId == b.Id)
                .Where((a, b) => a.Id == order.ReportId)
                .ToOneAsync((a, b) => new
                {
                    a.ReportNumber,
                    b.StartTime,
                    b.Title,
                    b.AssociationTitle,
                    b.Address,
                    b.Remark,
                    a.Name,
                    idCard = a.IdCard.Substring(0, 2) + "**************" + a.IdCard.Substring(a.IdCard.Length - 4, 4)
                });

            return detail;
        }
    }

    public class ReportOrderList()
    {
        public List<ReportOrderView> items { get; set; }

        public long total { get; set; }
    }
}
