using DotNetCore.CAP;
using EasyCaching.Core;
using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Domain;
using Essensoft.Paylink.Alipay.Request;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Order.Alipay;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;

namespace Magic.Guangdong.Exam.Areas.Order.Controllers
{
    [Area("Order")]
    public class AlipayController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ICapPublisher _capPublisher;
        private readonly IAlipayClient _client;
        private readonly IOptions<AlipayOptions> _optionsAccessor;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IOrderRepo _orderRepo;

        public AlipayController(IResponseHelper resp, ICapPublisher capPublisher, IAlipayClient client, IOptions<AlipayOptions> optionsAccessor, IRedisCachingProvider redisCachingProvider, IOrderRepo orderRepo)
        {
            _resp = resp;
            _capPublisher = capPublisher;
            _client = client;
            _optionsAccessor = optionsAccessor;
            _redisCachingProvider = redisCachingProvider;
            _orderRepo = orderRepo;

        }

        [RouteMark("订单管理")]
        public IActionResult Index()
        {
            return View();
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
        [HttpPost]
        public async Task<IActionResult> PagePay(AlipayTradePagePayViewModel viewModel)
        {
            var model = new AlipayTradePagePayModel
            {
                Body = viewModel.Body,
                Subject = viewModel.Subject,
                TotalAmount = viewModel.TotalAmount,
                OutTradeNo = viewModel.OutTradeNo,
                ProductCode = viewModel.ProductCode
            };
            var req = new AlipayTradePagePayRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewModel.NotifyUrl);
            req.SetReturnUrl(viewModel.ReturnUrl);

            var response = await _client.PageExecuteAsync(req, _optionsAccessor.Value);
            return Content(response.Body, "text/html", Encoding.UTF8);
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
            return View();
        }

        /// <summary>
        /// 交易查询
        /// </summary>
        [HttpPost]
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
            ViewData["response"] = ((AlipayResponse)response).Body;
            return View();
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
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Refund(string outTradeNo)
        {
            try
            {
                var order = await _orderRepo.getOneAsync(u => u.OutTradeNo == outTradeNo && u.Status == OrderStatus.Paid);
                if (order == null)
                    return Json(_resp.error("订单无法退款"));
                if(order.Status == OrderStatus.Refund)
                    return Json(_resp.error("订单已退款"));

                var model = new AlipayTradeRefundModel
                {
                    OutTradeNo = order.OutTradeNo,
                    TradeNo = order.TradeNo,
                    RefundAmount = Math.Round(order.Expenses, 2).ToString(),
                    OutRequestNo = $"RE{order.Id.ToString("N").ToUpper()}",
                    RefundReason = "后台操作退款"
                };

                var req = new AlipayTradeRefundRequest();
                req.SetBizModel(model);

                //var response = await _client.CertificateExecuteAsync(req, _optionsAccessor.Value);
                var response = await _client.ExecuteAsync(req, _optionsAccessor.Value);
                Console.WriteLine(response.Body);
                ViewData["response"] = response.Body;
                //return View();
                return Json(_resp.success(response.Body));
            }
            catch(Exception ex)
            {
                return Json(_resp.error("退款失败：" + ex.Message));
            }
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
        [HttpPost,ValidateAntiForgeryToken]
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

            //var response = await _client.CertificateExecuteAsync(req, _optionsAccessor.Value);
            //ViewData["response"] = response.Body;
            //return View();
            var response = await _client.ExecuteAsync(req, _optionsAccessor.Value);
            Console.WriteLine(response.Body);
            //ViewData["response"] = response.Body;
            //return View();
            return Json(_resp.success(response.Body));
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
        [HttpPost,ValidateAntiForgeryToken]
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

            var response = await _client.ExecuteAsync(req, _optionsAccessor.Value);
            Console.WriteLine(response.Body);
            ViewData["response"] = response.Body;
            return Json(_resp.success(response.Body));
        }

        /// <summary>
        /// 统一转账
        /// </summary>
        [HttpGet]
        public IActionResult Transfer()
        {
            return View();
        }

        /// <summary>
        /// 统一转账
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Transfer(AlipayTransferViewModel viewMode)
        {
            var model = new AlipayFundTransUniTransferModel
            {
                OutBizNo = viewMode.OutBizNo,
                TransAmount = viewMode.TransAmount,
                ProductCode = viewMode.ProductCode,
                BizScene = viewMode.BizScene,
                PayeeInfo = new Participant { Identity = viewMode.PayeeIdentity, IdentityType = viewMode.PayeeIdentityType, Name = viewMode.PayeeName },
                Remark = viewMode.Remark
            };
            var req = new AlipayFundTransUniTransferRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor.Value);
            ViewData["response"] = response.Body;
            return View();
        }

        /// <summary>
        /// 查询统一转账订单
        /// </summary>
        [HttpGet]
        public IActionResult TransQuery()
        {
            return View();
        }

        /// <summary>
        /// 查询统一转账订单
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> TransQuery(AlipayTransQueryViewModel viewMode)
        {
            var model = new AlipayFundTransCommonQueryModel
            {
                OutBizNo = viewMode.OutBizNo,
                OrderId = viewMode.OrderId
            };

            var req = new AlipayFundTransCommonQueryRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor.Value);
            ViewData["response"] = response.Body;
            return View();
        }

        /// <summary>
        /// 余额查询
        /// </summary>
        [HttpGet]
        public IActionResult AccountQuery()
        {
            return View();
        }

        /// <summary>
        /// 余额查询
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AccountQuery(AlipayAccountQueryViewModel viewModel)
        {
            var model = new AlipayFundAccountQueryModel
            {
                AlipayUserId = viewModel.AlipayUserId,
                AccountType = viewModel.AccountType
            };

            var req = new AlipayFundAccountQueryRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor.Value);
            ViewData["response"] = response.Body;
            return View();
        }
    }
}
