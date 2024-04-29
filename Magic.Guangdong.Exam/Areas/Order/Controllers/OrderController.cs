using DotNetCore.CAP;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Order;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace Magic.Guangdong.Exam.Areas.Order.Controllers
{
    [Area("Order")]
    public class OrderController : Controller
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IResponseHelper _resp;
        private readonly IReportInfoRepo _reportInfoRepo;
        private readonly ICapPublisher _capPublisher;
        public OrderController(IOrderRepo orderRepo, IResponseHelper resp, IReportInfoRepo reportInfoRepo, ICapPublisher capPublisher)
        {
            _orderRepo = orderRepo;
            _resp = resp;
            _reportInfoRepo = reportInfoRepo;
            _capPublisher = capPublisher;
        }

        [RouteMark("订单管理")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 100,VaryByQueryKeys = new string[] {"whereJsonStr","pageIndex","pageSize","rd"})]
        public IActionResult GetOrders(PageDto dto)
        {
            long total;
            return Json(_resp.success(
               new { items = _orderRepo.GetOrderList(dto,out total), total }));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SyncRefundOrderInfo(string outTradeNo)
        {
            if (await _orderRepo.SyncRefundOrderInfo(outTradeNo))
            {
                await _capPublisher.PublishDelayAsync(DateTime.Now.AddSeconds(5) - DateTime.Now, CapConsts.PREFIX + "NoticeUserRefund", outTradeNo);
                return Json(_resp.success("同步成功"));
            }
            return Json(_resp.error("同步失败"));
        }

        /// <summary>
        /// 合成试卷的事件
        /// </summary>
        /// <param name="paperIds"></param>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "NoticeUserRefund")]
        public async Task NoticeUserRefund(string outTradeNo)
        {
            Assistant.Logger.Warning($"{DateTime.Now},发送通知邮件");
            var reportOrder = await _orderRepo.getOneAsync(u=>u.OutTradeNo==outTradeNo);
            var reportInfo = await _reportInfoRepo.getOneAsync(u => u.Id == reportOrder.ReportId);
            if (reportInfo.ConnAvailable == ConnAvailable.Email)
            {

                List<MailboxAddress> noticeTo = new List<MailboxAddress>()
                {
                     new MailboxAddress(reportInfo.Name, reportInfo.Email)
                };
                await EmailKitHelper.SendEMailAsync("退单通知", $"您在广东教育协会提交的申报信息已退单，订单号【{outTradeNo}】，退单号【{reportOrder.RefundNo}】,款项会原路退回到您的支付账号下，感谢您的参与。",
                    noticeTo);
            }
        }
    }
}
