using DotNetCore.CAP;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Order;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Mvc;

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
                return Json(_resp.success("同步成功"));
            return Json(_resp.error("同步失败"));
        }
    }
}
