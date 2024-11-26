using DotNetCore.CAP;
using EasyCaching.Core;
using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Domain;
using Essensoft.Paylink.Alipay.Request;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Order.Alipay;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class MyOrderController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IOrderRepo _orderRepo;
        private readonly IAlipayClient _alipayClient;
        private readonly IRedisCachingProvider _redisCachingProvider;

        private readonly IOptions<AlipayOptions> _optionsAliAccessor;
        public MyOrderController(IResponseHelper resp, IOrderRepo orderRepo, IAlipayClient alipayClient,IRedisCachingProvider redisCachingProvider, IOptions<AlipayOptions> optionsAliAccessor)
        {
            _resp = resp;
            _orderRepo = orderRepo;
            _alipayClient = alipayClient;
            _redisCachingProvider = redisCachingProvider;
            _optionsAliAccessor = optionsAliAccessor;
        }

        /// <summary>
        /// 统一下单后查询订单支付状态
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 600, VaryByQueryKeys = new string[] { "orderId", "rd" })]
        public async Task<IActionResult> GetOrderDetail(Guid orderId)
        {
            if (!await _orderRepo.getAnyAsync(u => u.Id == orderId))
            {
                //return Json(_resp.error("订单不存在", new { url = "/Error?msg=" + Assistant.Utils.EncodeUrlParam("订单不存在") }));
                Redirect("/Error?msg=" + Assistant.Utils.EncodeUrlParam("订单不存在"));
            }
            var order = await _orderRepo.getOneAsync(u => u.Id == orderId);
            if (order.Status == DbServices.Entities.OrderStatus.Paid)
            {
                //return Json(_resp.ret(1,"已支付",new { url = "/Report/Index" }));
                Redirect("/Report/detail?id=" + order.ReportId);
            }
            
            return Json(_resp.success(order));
        }


        private async Task<AlipayResponse> AlipayTradeQuery(AlipayTradeQueryModel viewMode)
        {
            try
            {
                var model = new AlipayTradeQueryModel
                {
                    OutTradeNo = viewMode.OutTradeNo,
                    TradeNo = viewMode.TradeNo
                };

                var req = new AlipayTradeQueryRequest();
                req.SetBizModel(model);

                return await _alipayClient.CertificateExecuteAsync(req, _optionsAliAccessor.Value);
                
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        [NonAction]
        [CapSubscribe(CapConsts.ClientPrefix + "ConfirmAndSyncOrderInfo")]
        public async Task ConfirmAndSyncOrderInfo(string orderNo, [FromCap] CapHeader header)
        {
            string msgId = header["cap-msg-id"] ?? "";
            if (!string.IsNullOrEmpty(msgId) && await _redisCachingProvider.HExistsAsync(CapConsts.MsgIdCacheClientName, msgId))
            {
                Assistant.Logger.Verbose("已消费");
                return;
            }
            Console.WriteLine($"{DateTime.Now}:消费事务---订单支付成功，开始同步订单信息{orderNo}");
            await Task.CompletedTask;
        }
    }
}
