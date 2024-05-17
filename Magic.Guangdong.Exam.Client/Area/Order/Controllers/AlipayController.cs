using DotNetCore.CAP;
using EasyCaching.Core;
using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Domain;
using Essensoft.Paylink.Alipay.Request;
using FreeSql.Internal;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Order;
using Magic.Guangdong.DbServices.Dtos.Order.Alipay;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Web;

namespace Magic.Guangdong.Exam.Client.Area.Order.Controllers
{
    [Route("order/alipay")]
    public class AlipayController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ICapPublisher _capPublisher;
        private readonly IAlipayClient _client;
        private readonly IOptions<AlipayOptions> _optionsAccessor;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IOrderRepo _orderRepo;


        public AlipayController(IResponseHelper resp, ICapPublisher capPublisher, IAlipayClient alipayClient, IOptions<AlipayOptions> optionsAccessor, IRedisCachingProvider redisCachingProvider, IOrderRepo orderRepo)
        {
            _resp = resp;
            _capPublisher = capPublisher;
            _client = alipayClient;
            _optionsAccessor = optionsAccessor;
            _redisCachingProvider = redisCachingProvider;
            _orderRepo = orderRepo;

        }
        public IActionResult Index()
        {
            return Content("Alipay");
        }

        /// <summary>
        /// 电脑网站支付
        /// </summary>
        [HttpGet]
        public IActionResult PagePay()
        {
            return View();
        }

        /// <summary>
        /// 电脑网站支付
        /// </summary>
        /// <param name="viewModel"></param>
        [Route("pagepay")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> PagePay(string orderTradeNumber)
        {
            try
            {
                if (!await _orderRepo.getAnyAsync(u => u.OutTradeNo == orderTradeNumber))
                    return Redirect("/Error?msg=" + Assistant.Utils.EncodeUrlParam("订单不存在"));
                
                
                var order = await _orderRepo.getOneAsync(u => u.OutTradeNo == orderTradeNumber);

                if(order.Status == DbServices.Entities.OrderStatus.Paid)
                {
                    return Redirect("/Error?msg=" + Assistant.Utils.EncodeUrlParam("订单已支付"));

                }

                var model = new AlipayTradePagePayModel()
                {
                    Body = $"报名费用【{order.Subject}】_{DateTime.Now}",
                    Subject = order.Subject,
                    TotalAmount = Math.Round(order.Expenses, 2).ToString(),
                    //我们自己生成的订单，传入支付宝参数中，支付宝里也有一个TradeNo参数，那个咱不传了，用支付宝自己生成的
                    OutTradeNo = order.OutTradeNo,
                    ProductCode = "FAST_INSTANT_TRADE_PAY",
                };
                var req = new AlipayTradePagePayRequest();
                req.SetBizModel(model);
                req.SetNotifyUrl("https://localhost:7296/order/alipaynotify/pagepay");
                req.SetReturnUrl("https://localhost:7296/order/alipayreturn/pagepay");

                var response = await _client.PageExecuteAsync(req, _optionsAccessor.Value);
                return Content(response.Body, "text/html", Encoding.UTF8);
            }
            catch(Exception ex)
            {
                Assistant.Logger.Error("拉起支付宝支付失败");
                return Redirect("/Error?msg=" + Assistant.Utils.EncodeUrlParam("拉起支付宝支付失败，请重试"));
            }
        }

        /// <summary>
        /// 手机网站支付
        /// </summary>
        [HttpGet]
        public IActionResult WapPay()
        {
            return View();
        }

        /// <summary>
        /// 手机网站支付
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> WapPay(AlipayTradeWapPayViewModel viewMode)
        {
            var model = new AlipayTradeWapPayModel
            {
                Body = viewMode.Body,
                Subject = viewMode.Subject,
                TotalAmount = viewMode.TotalAmount,
                OutTradeNo = viewMode.OutTradeNo,
                ProductCode = viewMode.ProductCode
            };
            var req = new AlipayTradeWapPayRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewMode.NotifyUrl);
            req.SetReturnUrl(viewMode.ReturnUrl);

            var response = await _client.PageExecuteAsync(req, _optionsAccessor.Value);
            return Content(response.Body, "text/html", Encoding.UTF8);
        }

        /// <summary>
        /// 交易查询
        /// </summary>
        [HttpGet]
        public IActionResult Query()
        {
            return Content("请提交合适的参数");
            //return View();
        }

        /// <summary>
        /// 交易查询
        /// </summary>
        [Route("query")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Query(AlipayTradeQueryViewModel viewMode)
        {
            var model = new AlipayTradeQueryModel
            {
                OutTradeNo = viewMode.OutTradeNo,
                TradeNo = viewMode.TradeNo
            };

            var req = new AlipayTradeQueryRequest();
            req.SetBizModel(model);

            var response = await _client.ExecuteAsync(req, _optionsAccessor.Value);
            if(response.TradeStatus== "TRADE_SUCCESS")
            {
                //发布一个消息
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "SyncOrderInfo", new SyncOrderDto()
                {
                    OutTradeNo = response.OutTradeNo,
                    TradeNo = response.TradeNo,
                    PayType = PayType.AliPay,
                    TotalAmount = response.TotalAmount,
                    Timestamp = Assistant.Utils.DateTimeToTimeStamp(Convert.ToDateTime(response.SendPayDate)).ToString()
                }) ;
            }
            //ViewData["response"] = ((AlipayResponse)response).Body;
            //return View();
            return Json(_resp.success(response));
        }

        /// <summary>
        /// 交易退款
        /// </summary>
        [HttpGet]
        public IActionResult Refund()
        {
            return View();
        }

        /// <summary>
        /// 交易退款
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Refund(AlipayTradeRefundViewModel viewMode)
        {
            var model = new AlipayTradeRefundModel
            {
                OutTradeNo = viewMode.OutTradeNo,
                TradeNo = viewMode.TradeNo,
                RefundAmount = viewMode.RefundAmount,
                OutRequestNo = viewMode.OutRequestNo,
                RefundReason = viewMode.RefundReason
            };

            var req = new AlipayTradeRefundRequest();
            req.SetBizModel(model);

            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor.Value);
            ViewData["response"] = response.Body;
            return View();
        }

        /// <summary>
        /// 退款查询
        /// </summary>
        [HttpGet]
        public IActionResult RefundQuery()
        {
            return View();
        }

        /// <summary>
        /// 退款查询
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RefundQuery(AlipayTradeRefundQueryViewModel viewMode)
        {
            var model = new AlipayTradeFastpayRefundQueryModel
            {
                OutTradeNo = viewMode.OutTradeNo,
                TradeNo = viewMode.TradeNo,
                OutRequestNo = viewMode.OutRequestNo
            };

            var req = new AlipayTradeFastpayRefundQueryRequest();
            req.SetBizModel(model);

            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor.Value);
            ViewData["response"] = response.Body;
            return View();
        }

        /// <summary>
        /// 交易关闭
        /// </summary>
        [HttpGet]
        public IActionResult Close()
        {
            return View();
        }

        /// <summary>
        /// 交易关闭
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Close(AlipayTradeCloseViewModel viewMode)
        {
            var model = new AlipayTradeCloseModel
            {
                OutTradeNo = viewMode.OutTradeNo,
                TradeNo = viewMode.TradeNo,
            };

            var req = new AlipayTradeCloseRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewMode.NotifyUrl);

            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor.Value);
            ViewData["response"] = response.Body;
            return View();
        }
    }
}
