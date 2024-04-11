using DotNetCore.CAP;
using EasyCaching.Core;
using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Notify;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Magic.Guangdong.Exam.Client.Area.Order.Controllers
{
    [Route("order/alipaynotify")]
    public class AlipayNotifyController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ICapPublisher _capPublisher;
        private readonly IAlipayNotifyClient _client;
        private readonly IOptions<AlipayOptions> _optionsAccessor;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IOrderRepo _orderRepo;

        public AlipayNotifyController(IResponseHelper resp, ICapPublisher capPublisher, IAlipayNotifyClient client, IOptions<AlipayOptions> optionsAccessor, IRedisCachingProvider redisCachingProvider, IOrderRepo orderRepo)
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
        /// 应用网关
        /// </summary>
        /// <returns></returns>
        [Route("gateway")]
        [HttpPost]
        public async Task<IActionResult> Gateway()
        {
            try
            {
                var msg_method = Request.Form["msg_method"].ToString();
                switch (msg_method)
                {
                    // 资金单据状态变更通知
                    case "alipay.fund.trans.order.changed":
                        {
                            var notify = await _client.CertificateExecuteAsync<AlipayFundTransOrderChangedNotify>(Request, _optionsAccessor.Value);
                            return AlipayNotifyResult.Success;
                        }
                    // 第三方应用授权取消消息
                    case "alipay.open.auth.appauth.cancelled":
                        {
                            var notify = await _client.CertificateExecuteAsync<AlipayOpenAuthAppauthCancelledNotify>(Request, _optionsAccessor.Value);
                            return AlipayNotifyResult.Success;
                        }
                    // 用户授权取消消息
                    case "alipay.open.auth.userauth.cancelled":
                        {
                            var notify = await _client.CertificateExecuteAsync<AlipayOpenAuthUserauthCancelledNotify>(Request, _optionsAccessor.Value);
                            return AlipayNotifyResult.Success;
                        }
                    // 小程序审核通过通知
                    case "alipay.open.mini.version.audit.passed":
                        {
                            var notify = await _client.CertificateExecuteAsync<AlipayOpenMiniVersionAuditPassedNotify>(Request, _optionsAccessor.Value);
                            return AlipayNotifyResult.Success;
                        }
                    // 小程序审核驳回通知
                    case "alipay.open.mini.version.audit.rejected":
                        {
                            var notify = await _client.CertificateExecuteAsync<AlipayOpenMiniVersionAuditRejectedNotify>(Request, _optionsAccessor.Value);
                            return AlipayNotifyResult.Success;
                        }
                    // 收单退款冲退完成通知
                    case "alipay.trade.refund.depositback.completed":
                        {
                            var notify = await _client.CertificateExecuteAsync<AlipayTradeRefundDepositbackCompletedNotify>(Request, _optionsAccessor.Value);
                            return AlipayNotifyResult.Success;
                        }
                    // 收单资金结算到银行账户，结算退票的异步通知
                    case "alipay.trade.settle.dishonoured":
                        {
                            var notify = await _client.CertificateExecuteAsync<AlipayTradeSettleDishonouredNotify>(Request, _optionsAccessor.Value);
                            return AlipayNotifyResult.Success;
                        }
                    // 收单资金结算到银行账户，结算失败的异步通知
                    case "alipay.trade.settle.fail":
                        {
                            var notify = await _client.CertificateExecuteAsync<AlipayTradeSettleFailNotify>(Request, _optionsAccessor.Value);
                            return AlipayNotifyResult.Success;
                        }
                    // 收单资金结算到银行账户，结算成功的异步通知
                    case "alipay.trade.settle.success":
                        {
                            var notify = await _client.CertificateExecuteAsync<AlipayTradeSettleSuccessNotify>(Request, _optionsAccessor.Value);
                            return AlipayNotifyResult.Success;
                        }
                    // 身份认证记录消息
                    case "alipay.user.certify.open.notify.completed":
                        {
                            var notify = await _client.CertificateExecuteAsync<AlipayUserCertifyOpenNotifyCompletedNotify>(Request, _optionsAccessor.Value);
                            return AlipayNotifyResult.Success;
                        }
                    default:
                        return AlipayNotifyResult.Failure;
                }
            }
            catch (AlipayException ex)
            {
                Assistant.Logger.Error("出现异常: " + ex.Message);
                await EmailKitHelper.SendEMailToDevMsgAsync("支付消息回调异常："+ex.Message);
                return AlipayNotifyResult.Failure;
            }
        }

        /// <summary>
        /// 扫码支付异步通知
        /// </summary>
        [Route("precreate")]
        [HttpPost]
        public async Task<IActionResult> Precreate()
        {
            try
            {
                var notify = await _client.CertificateExecuteAsync<AlipayTradePrecreateNotify>(Request, _optionsAccessor.Value);
                switch (notify.TradeStatus)
                {
                    case AlipayTradeStatus.Wait: // 等待付款
                        Logger.Warning("扫码支付异步通知-等待买家付款 => OutTradeNo: " + notify.OutTradeNo);
                        return AlipayNotifyResult.Success;
                    case AlipayTradeStatus.Success: // 支付成功
                        Logger.Info("扫码支付异步通知-交易支付成功 => OutTradeNo: " + notify.OutTradeNo);
                        return AlipayNotifyResult.Success;
                    default:
                        return AlipayNotifyResult.Failure;
                }
            }
            catch (AlipayException ex)
            {
                Logger.Error("出现异常: " + ex.Message);
                return AlipayNotifyResult.Failure;
            }
        }

        /// <summary>
        /// APP支付异步通知
        /// </summary>
        [Route("apppay")]
        [HttpPost]
        public async Task<IActionResult> AppPay()
        {
            try
            {
                var notify = await _client.CertificateExecuteAsync<AlipayTradeAppPayNotify>(Request, _optionsAccessor.Value);
                switch (notify.TradeStatus)
                {
                    case AlipayTradeStatus.Success: // 支付成功
                        Logger.Info("APP支付异步通知-交易支付成功 => OutTradeNo: " + notify.OutTradeNo);
                        return AlipayNotifyResult.Success;
                    case AlipayTradeStatus.Closed: // 交易关闭
                       Logger.Warning("APP支付异步通知-未付款交易超时关闭，或支付完成后全额退款 => OutTradeNo: " + notify.OutTradeNo);
                        return AlipayNotifyResult.Success;
                    case AlipayTradeStatus.Finished: // 交易完结
                        Logger.Warning("APP支付异步通知-交易结束，不可退款 => OutTradeNo: " + notify.OutTradeNo);
                        return AlipayNotifyResult.Success;
                    default:
                        return AlipayNotifyResult.Failure;
                }
            }
            catch (AlipayException ex)
            {
                Logger.Error("出现异常: " + ex.Message);
                return AlipayNotifyResult.Failure;
            }
        }

        /// <summary>
        /// 电脑网站支付异步通知
        /// </summary>
        [Route("pagepay")]
        [HttpPost]
        public async Task<IActionResult> PagePay()
        {
            try
            {
                var notify = await _client.CertificateExecuteAsync<AlipayTradePagePayNotify>(Request, _optionsAccessor.Value);
                switch (notify.TradeStatus)
                {
                    case AlipayTradeStatus.Success: // 支付成功
                        Logger.Info("电脑网站支付异步通知-交易支付成功 => OutTradeNo: " + notify.OutTradeNo);
                        return AlipayNotifyResult.Success;
                    case AlipayTradeStatus.Closed: // 交易关闭
                        Logger.Warning("电脑网站支付异步通知-未付款交易超时关闭，或支付完成后全额退款 => OutTradeNo: " + notify.OutTradeNo);
                        return AlipayNotifyResult.Success;
                    case AlipayTradeStatus.Finished: // 交易完结
                        Logger.Warning("电脑网站支付异步通知-交易结束，不可退款 => OutTradeNo: " + notify.OutTradeNo);
                        return AlipayNotifyResult.Success;
                    default:
                        return AlipayNotifyResult.Failure;
                }
            }
            catch (AlipayException ex)
            {
                Logger.Error("出现异常: " + ex.Message);
                return AlipayNotifyResult.Failure;
            }
        }

        /// <summary>
        /// 手机网站支付异步通知
        /// </summary>
        [Route("wappay")]
        [HttpPost]
        public async Task<IActionResult> WapPay()
        {
            try
            {
                var notify = await _client.CertificateExecuteAsync<AlipayTradeWapPayNotify>(Request, _optionsAccessor.Value);
                switch (notify.TradeStatus)
                {
                    case AlipayTradeStatus.Success: // 支付成功
                        Logger.Info("手机网站支付异步通知-交易支付成功 => OutTradeNo: " + notify.OutTradeNo);
                        return AlipayNotifyResult.Success;
                    case AlipayTradeStatus.Closed: // 交易关闭
                        Logger.Warning("手机网站支付异步通知-未付款交易超时关闭，或支付完成后全额退款 => OutTradeNo: " + notify.OutTradeNo);
                        return AlipayNotifyResult.Success;
                    case AlipayTradeStatus.Finished: // 交易完结
                        Logger.Warning("手机网站支付异步通知-交易结束，不可退款 => OutTradeNo: " + notify.OutTradeNo);
                        return AlipayNotifyResult.Success;
                    default:
                        return AlipayNotifyResult.Failure;
                }
            }
            catch (AlipayException ex)
            {
                Logger.Error("出现异常: " + ex.Message);
                return AlipayNotifyResult.Failure;
            }
        }

        /// <summary>
        /// 交易关闭异步通知
        /// </summary>
        [Route("close")]
        [HttpPost]
        public async Task<IActionResult> Close()
        {
            try
            {
                var notify = await _client.CertificateExecuteAsync<AlipayTradeCloseNotify>(Request, _optionsAccessor.Value);
                if (notify.TradeStatus == AlipayTradeStatus.Closed)
                {
                    Logger.Warning("交易关闭异步通知 => OutTradeNo: " + notify.OutTradeNo);

                    return AlipayNotifyResult.Success;
                }

                return AlipayNotifyResult.Failure;
            }
            catch (AlipayException ex)
            {
                Logger.Error("出现异常: " + ex.Message);
                return AlipayNotifyResult.Failure;
            }
        }
    }
}
