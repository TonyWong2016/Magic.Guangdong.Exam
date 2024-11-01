using FreeSql.Internal.Model;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Order;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class OrderRepo : ExaminationRepository<Order>, IOrderRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public OrderRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<bool> SyncOrderInfo(SyncOrderDto dto)
        {
            using(var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var orderRepo = fsql.Get(conn_str).GetRepository<Order>();
                    orderRepo.UnitOfWork = uow;
                    if (!await orderRepo.Where(u => u.OutTradeNo == dto.OutTradeNo && u.IsDeleted == 0).AnyAsync())
                    {
                        return false;
                    }
                    var order = await orderRepo.Where(u => u.OutTradeNo == dto.OutTradeNo).FirstAsync();
                    order.Status = OrderStatus.Paid;
                    order.PayType = dto.PayType;
                    order.PayTime = dto.Timestamp;
                    order.UpdatedAt = DateTime.Now;
                    order.TradeNo = dto.TradeNo;
                    //await orderRepo.UpdateAsync(order);

                    var reportProcessRepo = fsql.Get(conn_str).GetRepository<ReportProcess>();
                    reportProcessRepo.UnitOfWork = uow;
                    if(!await reportProcessRepo.Where(u => u.OrderId == order.Id).AnyAsync())
                    {
                        return false;
                    }
                    var reportProcess = await reportProcessRepo.Where(u => u.OrderId == order.Id).FirstAsync();
                    reportProcess.Status = ReportStatus.Succeed;
                    reportProcess.Step = ReportStep.Paied;
                    reportProcess.UpdatedAt = DateTime.Now;

                    await orderRepo.UpdateAsync(order);
                    await reportProcessRepo.UpdateAsync(reportProcess);
                    uow.Commit();
                    return true;
                }
                catch(Exception ex)
                {
                    uow.Rollback();
                    Assistant.Logger.Error("同步支付信息失败"+ex.Message);
                    return false;
                }
            }
            
            
        }

        /// <summary>
        /// 同步退款
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public async Task<bool> SyncRefundOrderInfo(string outTradeNo)
        {
            using(var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var orderRepo = fsql.Get(conn_str).GetRepository<Order>();
                    orderRepo.UnitOfWork = uow;
                    if (!await orderRepo.Where(u => u.OutTradeNo == outTradeNo || u.Status == OrderStatus.Paid).AnyAsync())
                    {
                        return false;
                    }
                    var order = await orderRepo.Where(u => u.OutTradeNo == outTradeNo).FirstAsync();
                    order.Status = OrderStatus.Refund;
                    order.UpdatedAt = DateTime.Now;
                    order.RefundNo = $"RE{order.Id.ToString("N").ToUpper()}";
                    order.Remark = "后台操作退款";

                    var reportProcessRepo = fsql.Get(conn_str).GetRepository<ReportProcess>();
                    reportProcessRepo.UnitOfWork = uow;
                    var reportProcess = await reportProcessRepo.Where(u => u.OrderId == order.Id).FirstAsync();
                    reportProcess.Status = ReportStatus.Refunded;
                    //后台操作不修改前台客户的报名进度字段
                    //reportProcess.Step = ReportStep.Failed;
                    reportProcess.UpdatedAt = DateTime.Now;

                    await orderRepo.UpdateAsync(order);
                    await reportProcessRepo.UpdateAsync(reportProcess);
                    uow.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    Assistant.Logger.Error("同步退款信息失败"+ex.Message);
                    return false;
                }
            }
        }

        public dynamic GetOrderList(PageDto pageDto, out long total)
        {
            var query = fsql.Get(conn_str).Select<Order, ReportInfo>()
                .LeftJoin((o, r) => o.ReportId == r.Id)
                .Where((o, r) => o.IsDeleted == 0);
            if (!string.IsNullOrEmpty(pageDto.whereJsonStr))
            {
                DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(pageDto.whereJsonStr);
                query = query
                    .WhereDynamicFilter(dyfilter);
            }
            return query
                .Count(out total)
                .OrderByPropertyNameIf(!string.IsNullOrEmpty(pageDto.orderby), pageDto.orderby, pageDto.isAsc)
                .OrderByPropertyNameIf(string.IsNullOrEmpty(pageDto.orderby), "CreatedAt",false)
                .Page(pageDto.pageindex, pageDto.pagesize)
                .ToList((o, r) => new
                {
                    o.Id,
                    o.OutTradeNo,
                    o.TradeNo,
                    o.Subject,
                    o.PayType,
                    o.PayTime,
                    o.Expenses,
                    o.InvoiceId,
                    o.AccountId,
                    o.CreatedAt,
                    o.Status,
                    o.RefundNo,
                    IdCard = r.IdCard.Length > 4 ? (r.IdCard.Substring(0, 2) + "****************" + r.IdCard.Substring(r.IdCard.Length - 4, 4)) : r.IdCard,
                    r.Name,
                    r.Email,
                    r.Mobile
                });
        }
    }
}
