using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Order
{
    public class SyncOrderDto
    {
        /// <summary>
        /// 支付平台返回来的订单号，格式不统一，保留先不用
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>
        /// 系统生成的订单号，格式统一，优先用这个
        /// </summary>
        public string OutTradeNo { get; set; }

        public string TotalAmount { get; set; }

        public string Timestamp { get; set; }

        public PayType PayType { get; set; }
    }
}
