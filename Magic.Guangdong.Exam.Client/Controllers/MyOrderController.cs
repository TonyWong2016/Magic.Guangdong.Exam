using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Domain;
using Essensoft.Paylink.Alipay.Request;
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
        private readonly IOptions<AlipayOptions> _optionsAliAccessor;

        public MyOrderController(IResponseHelper resp, IOrderRepo orderRepo, IAlipayClient alipayClient, IOptions<AlipayOptions> optionsAliAccessor)
        {
            _resp = resp;
            _orderRepo = orderRepo;
            _alipayClient = alipayClient;
            _optionsAliAccessor = optionsAliAccessor;
        }

        /// <summary>
        /// 统一下单后查询订单支付状态
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 600, VaryByQueryKeys = new string[] { "outTradeNo", "rd" })]
        public async Task<IActionResult> ConfirmOrderStatus(string outTradeNo)
        {
            if (!await _orderRepo.getAnyAsync(u => u.OutTradeNo == outTradeNo))
            {
                //return Redirect("/Error?msg=" + Assistant.Utils.EncodeUrlParam("订单不存在"));
                return Json(_resp.error("订单不存在", new { url = "/Error?msg=" + Assistant.Utils.EncodeUrlParam("订单不存在") }));
            }
            var order = await _orderRepo.getOneAsync(u=>u.OutTradeNo== outTradeNo);
            if (order.Status == DbServices.Entities.OrderStatus.Paid)
            {
                return Json(_resp.ret(1,"已支付",new { url = "/Report/Index" }));
            }
            //if(order.PayType==DbServices.Entities.PayType.AliPay)
            //{
            //    var model = new AlipayTradeQueryModel
            //    {
            //        OutTradeNo = order.OutTradeNo,
            //        TradeNo = order.TradeNo
            //    };
            //    return Json(_resp.ret(1,"有差异",await AlipayTradeQuery(model)));
            //}
            ////微信，todo。。
            //return Json(_resp.error("查询失败"));
            return Json(_resp.success("待支付"));
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
    }
}
