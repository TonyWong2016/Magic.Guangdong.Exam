using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Order;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IOrderRepo:IExaminationRepository<Order>
    {
        /// <summary>
        /// 同步支付订单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> SyncOrderInfo(SyncOrderDto dto);

        /// <summary>
        /// 同步退款订单
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        Task<bool> SyncRefundOrderInfo(string outTradeNo);

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="pageDto"></param>
        /// <returns></returns>
        dynamic GetOrderList(PageDto pageDto, out long total);
    }
}
