using Magic.Guangdong.DbServices.Dtos.Order;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    Assistant.Logger.Error("同步支付信息失败");
                    return false;
                }
            }
            
            
        }
    }
}
