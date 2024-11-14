using DotNetCore.CAP;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Material;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Material.Controllers
{
    [Area("material")]
    public class MaterialController : Controller
    {
        private readonly IFileRepo _fileRepo;
        private readonly IResponseHelper _resp;
        private readonly IHttpContextAccessor _contextAccessor;

        private string adminId = "system";
        public MaterialController(IFileRepo fileRepo, IResponseHelper resp, IHttpContextAccessor httpContextAccessor)
        {
            _fileRepo = fileRepo;
            _resp = resp;
            _contextAccessor = httpContextAccessor;
            adminId = (_contextAccessor.HttpContext != null && _contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").Any()) ?
               Assistant.Utils.FromBase64Str(_contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").First().Value) : "system";

        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取素材文件列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult GetList(PageDto dto)
        {
            long total=0;
            return Json(_resp.success(new { items = _fileRepo.getList(dto, out total), total }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(long[] ids,int realDel=0)
        {
            bool del = realDel == 1;
            List<string> delPaths = new List<string>();
            try
            {
                var files = await _fileRepo.getListAsync(u => ids.Contains(u.Id));
                
                List<DbServices.Entities.File> list = new List<DbServices.Entities.File>();
                foreach (var file in files)
                {       
                    if(del)
                        delPaths.Add(file.Path);
                    file.IsDeleted = 1;
                    file.updatedAt = DateTime.Now;
                    file.AccountId = adminId;
                    file.Remark = "fake deleted";
                    list.Add(file);
                }
                return Json(_resp.success(await _fileRepo.updateItemsAsync(list)));
            }
            catch (Exception ex)
            {
                Assistant.Logger.Error(ex);
                return Json(_resp.error(ex.Message));
            }
            finally
            {
                if(del && delPaths.Count > 0)
                {
                    foreach(var path in delPaths)
                        FileHelper.FileRemove(path);
                }
            }
        }

        /// <summary>
        /// 绑定素材
        /// </summary>
        /// <param name="paperIds"></param>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "BindMaterial")]
        public async Task BindMaterial(MaterialDto dto)
        {
            Assistant.Logger.Warning($"{DateTime.Now},开始绑定资源,{dto.ConnName}");
            //await _paperRepo.GeneratePaper(paperIds, adminId);
            if(await _fileRepo.getAnyAsync(u => u.Id == dto.Id))
            {
               var file = await _fileRepo.getOneAsync(u=>u.Id==dto.Id);
                file.ConnId = dto.ConnId;
                file.ConnName = dto.ConnName;
                file.updatedAt= DateTime.Now;
                file.Remark = "绑定资源";
                await _fileRepo.updateItemAsync(file);
                Assistant.Logger.Warning($"{DateTime.Now},绑定成功");
            }
            else
            {
                Assistant.Logger.Warning($"{DateTime.Now},绑定失败");
            }
        }
    }
}
