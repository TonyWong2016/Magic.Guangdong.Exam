using DotNetCore.CAP;
using EasyCaching.Core;
using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Domain;
using Essensoft.Paylink.Alipay.Request;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Order.Alipay;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;

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
        private readonly IExaminationRepo _examinationRepo;
        public AlipayController(IResponseHelper resp, ICapPublisher capPublisher, IAlipayClient alipayClient, IOptions<AlipayOptions> optionsAccessor, IRedisCachingProvider redisCachingProvider, IOrderRepo orderRepo, IExaminationRepo examinationRepo)
        {
            _resp = resp;
            _capPublisher = capPublisher;
            _client = alipayClient;
            _optionsAccessor = optionsAccessor;
            _redisCachingProvider = redisCachingProvider;
            _orderRepo = orderRepo;
            _examinationRepo = examinationRepo;
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
        [HttpPost]
        public async Task<IActionResult> PagePay2(AlipayTradePagePayViewModel viewModel)
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

        [Route("pagepay")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> PagePay(Guid examId)
        {
            var exam = await _examinationRepo.getOneAsync(u=>u.Id==examId);
            if (exam.Expenses == 0)
                exam.Expenses = 1;
            var model = new AlipayTradePagePayModel()
            {
                Body = $"报名费用【{exam.Title}】_{DateTime.Now}",
                Subject = exam.Title,
                TotalAmount = Math.Round(exam.Expenses, 2).ToString(),                
                //交易单号32位，考试id的4位（21-24）+13位时间戳+15位随机数
                OutTradeNo = $"{examId.ToString().Substring(20, 4)}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{Assistant.Utils.GenerateRandomCodePro(15)}",
                ProductCode = "FAST_INSTANT_TRADE_PAY",                
            };
            var req = new AlipayTradePagePayRequest();
            req.SetBizModel(model);
            string notifyUrl = Url.RouteUrl("/order/alipaynotify/pagepay");
            Console.WriteLine(notifyUrl);
            req.SetNotifyUrl("https://localhost:7296/order/alipaynotify/pagepay");
            req.SetReturnUrl("https://localhost:7296/order/alipayreturn/pagepay");

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

            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor.Value);
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
