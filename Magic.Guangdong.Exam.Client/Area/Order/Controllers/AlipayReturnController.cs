using DotNetCore.CAP;
using EasyCaching.Core;
using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Notify;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
                //ViewData["response"] = "支付成功";
                Assistant.Logger.Info("支付成功");
                //return View();
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
    }
}
