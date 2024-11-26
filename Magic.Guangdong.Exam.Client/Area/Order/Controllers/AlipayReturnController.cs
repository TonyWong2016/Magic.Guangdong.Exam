using Azure;
using DotNetCore.CAP;
using EasyCaching.Core;
using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Notify;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Order;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Web;

namespace Magic.Guangdong.Exam.Client.Area.Order.Controllers
{
    [Route("order/alipayreturn")]
    public class AlipayReturnController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ICapPublisher _capPublisher;
        private readonly IAlipayNotifyClient _client;
        private readonly IOptions<AlipayOptions> _optionsAccessor;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IOrderRepo _orderRepo;

        public AlipayReturnController(IResponseHelper resp, ICapPublisher capPublisher, IAlipayNotifyClient client, IOptions<AlipayOptions> optionsAccessor, IRedisCachingProvider redisCachingProvider, IOrderRepo orderRepo)
        {
            _resp = resp;
            _capPublisher = capPublisher;
            _client = client;
            _optionsAccessor = optionsAccessor;
            _redisCachingProvider = redisCachingProvider;
            _orderRepo = orderRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 电脑网站支付 - 同步跳转
        /// </summary>
        [Route("pagepay")]
        [HttpGet]
        public async Task<IActionResult> PagePay()
        {
            try
            {
                var notify = await _client.ExecuteAsync<AlipayTradePagePayReturn>(Request, _optionsAccessor.Value);
                Assistant.Logger.Debug(Assistant.JsonHelper.JsonSerialize(notify));
                //ViewData["response"] = "支付成功";
                Assistant.Logger.Info("支付成功");

                string sub = Assistant.Utils.EncodeUrlParam($"{{\"tradeNo\":\"{notify.TradeNo}\",\"outTradeNo\":\"{notify.OutTradeNo}\",\"payType\":\"alipay\",\"totalAmount\":{notify.TotalAmount}}}");
                //发布一个消息
                await _capPublisher.PublishAsync(CapConsts.ClientPrefix + "SyncOrderInfo", new SyncOrderDto()
                {
                    OutTradeNo = notify.OutTradeNo,
                    TradeNo = notify.TradeNo,
                    PayType = PayType.AliPay,
                    TotalAmount = notify.TotalAmount,
                    Timestamp = notify.Timestamp
                });

                var order = await _orderRepo.getOneAsync(u => u.TradeNo == notify.OutTradeNo);
                await _capPublisher.PublishAsync(CapConsts.ClientPrefix + "CheckReportStatus", order.ReportId);
                return Redirect("/order/result?sub=" + sub);
            }
            catch (AlipayException ex)
            {
                Assistant.Logger.Error("出现异常: " + ex.Message);
                //ViewData["response"] = "出现错误";
                //return View();
                return Redirect("/Error?msg=" + Assistant.Utils.EncodeUrlParam("支付异常，请进到【我的费用】栏目查看"));
            }
        }

        /// <summary>
        /// 手机网站支付 - 同步跳转
        /// </summary>
        [HttpGet]
        [Route("wappay")]
        public async Task<IActionResult> WapPay()
        {
            try
            {
                var notify = await _client.ExecuteAsync<AlipayTradeWapPayReturn>(Request, _optionsAccessor.Value);
                //ViewData["response"] = "支付成功";
                Assistant.Logger.Info("支付成功");
                //return View();3
                return Json(_resp.success("支付成功"));

            }
            catch (AlipayException ex)
            {
                Assistant.Logger.Error("出现异常: " + ex.Message);
                //ViewData["response"] = "出现错误";
                //return View();
                return Redirect("/Error?msg=payerror");

            }
        }

        [NonAction]
        [CapSubscribe(CapConsts.ClientPrefix + "SyncOrderInfo")]
        public async Task SyncOrderInfo(SyncOrderDto dto, [FromCap] CapHeader header)
        {
            Assistant.Logger.Warning($"消费事务---同步订单");
            if (await _redisCachingProvider.HExistsAsync(CapConsts.MsgIdCacheOaName, header["cap-msg-id"]))
            {
                Assistant.Logger.Verbose("已消费");
                return;
            }
            await _orderRepo.SyncOrderInfo(dto);
        }
    }
}
