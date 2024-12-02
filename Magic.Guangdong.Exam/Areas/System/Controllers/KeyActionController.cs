using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("system")]
    public class KeyActionController : Controller
    {
        private readonly IKeyActionRepo _keyActionRepo;
        private readonly IRedisCachingProvider _redisCachingProvider;

        private readonly IResponseHelper _resp;
        public KeyActionController(IKeyActionRepo keyActionRepo,IResponseHelper responseHelper,IRedisCachingProvider redisCachingProvider)
        {
            _keyActionRepo = keyActionRepo;
            _resp = responseHelper;
            _redisCachingProvider = redisCachingProvider;
        }
        public IActionResult Index()
        {
            return View();
        }

        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "AddKeyAction")]
        public async Task AddKeyAction(KeyAction keyAction, [FromCap] CapHeader header)
        {
            try
            {
                string msgId = header["cap-msg-id"] ?? "";
                if(!string.IsNullOrEmpty(msgId) && await _redisCachingProvider.HExistsAsync(CapConsts.MsgIdCacheOaName, msgId))
                {
                    Assistant.Logger.Warning("动作已被记录");
                    return;
                }
                keyAction.CapInstance= header["cap-exec-instance-id"] ?? "";
                keyAction.CapMsgId = msgId;
                keyAction.CapSenttime= header["cap-senttime"] ?? "";
                //keyAction.ExpiredAt = 14;
                if (await _keyActionRepo.getAnyAsync(u => u.Id == keyAction.Id || u.CapMsgId == msgId))
                    return;
                await _keyActionRepo.addItemAsync(keyAction);
            }
            catch (Exception ex)
            {
                Assistant.Logger.Error("记录关键动作失败"+ex.Message);
            }
        }

        [ResponseCache(Duration = 60)]
        public IActionResult GetKeyActions(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _keyActionRepo.getList(dto, out total),total }));
        }
    }
}
