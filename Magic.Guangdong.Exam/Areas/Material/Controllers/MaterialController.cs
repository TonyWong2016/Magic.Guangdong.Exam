using DotNetCore.CAP;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.File;
using Magic.Guangdong.DbServices.Dtos.Material;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Areas.Material.Controllers
{
    [Area("material")]
    public class MaterialController : Controller
    {
        private readonly IFileRepo _fileRepo;
        private readonly IResponseHelper _resp;
        private readonly IQuestionItemRepo _questionItemRepo;
        private readonly IHttpContextAccessor _contextAccessor;

        private string adminId = "system";
        public MaterialController(IFileRepo fileRepo, IResponseHelper resp,IQuestionItemRepo questionItemRepo, IHttpContextAccessor httpContextAccessor)
        {
            _fileRepo = fileRepo;
            _resp = resp;
            _contextAccessor = httpContextAccessor;
            _questionItemRepo = questionItemRepo;
            adminId = (_contextAccessor.HttpContext != null && _contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").Any()) ?
               Assistant.Utils.FromBase64Str(_contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").First().Value) : "system";

        }
        [RouteMark("素材管理")]
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
            var items = _fileRepo.getList(dto, out total).Adapt<List<FileDto>>();
            return Json(_resp.success(new { items , total }));
        }

        /// <summary>
        /// 获取指定素材
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="connName"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 100, VaryByQueryKeys = ["connId", "connName", "rd"])]
        public async Task<IActionResult> GetMaterials(string connId, string connName)
        {
            return Json(_resp.success(await _fileRepo.GetMaterials(connId, connName)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMaterial(long id, int realDel = 0)
        {
            bool del = realDel == 1;
            string delPath = "";
            try
            {
                var file = await _fileRepo.getOneAsync(u =>u.Id==id);
                file.Remark += "fake deleted";
                file.IsDeleted = 1;
                file.updatedAt = DateTime.Now;
                file.AccountId = adminId;
                if (del)
                {
                    delPath = file.Path;
                    file.Remark += "real deleted";
                }
               
                
                return Json(_resp.success(await _fileRepo.updateItemAsync(file)));
            }
            catch (Exception ex)
            {
                Assistant.Logger.Error(ex);
                return Json(_resp.error(ex.Message));
            }
            finally
            {
                if (del && !string.IsNullOrEmpty(delPath))
                {
                    FileHelper.FileRemove(delPath);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBatch(long[] ids,int realDel=0)
        {
            bool del = realDel == 1;
            List<string> delPaths = new List<string>();
            try
            {
                var files = await _fileRepo.getListAsync(u => ids.Contains(u.Id));
                
                List<DbServices.Entities.File> list = new List<DbServices.Entities.File>();
                foreach (var file in files)
                {
                    file.Remark += "fake deleted";                    
                    file.IsDeleted = 1;
                    file.updatedAt = DateTime.Now;
                    file.AccountId = adminId;
                    if (del)
                    {
                        delPaths.Add(file.Path);
                        file.Remark += "real deleted";
                    }
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BindMaterial(MaterialDto dto)
        {
            Assistant.Logger.Warning($"{DateTime.Now},开始绑定资源,{dto.ConnName}");
            if (!string.IsNullOrEmpty(dto.Link) && dto.Id==0)
            {
                var file = new DbServices.Entities.File()
                {
                    ConnId = dto.ConnId,
                    ConnName = dto.ConnName,
                    updatedAt = DateTime.Now,
                    Remark = dto.Remark,
                    ShortUrl = dto.Link,
                    Path = dto.Link,
                    Ext = Path.GetExtension(dto.Link),
                    Type = "net"
                };
                await _fileRepo.insertOrUpdateAsync(file);
                return Json(_resp.success("绑定成功"));

            }
            if (await _fileRepo.getAnyAsync(u => u.Id == dto.Id))
            {
                var file = await _fileRepo.getOneAsync(u => u.Id == dto.Id);
                file.ConnId = dto.ConnId;
                file.ConnName = dto.ConnName;
                file.updatedAt = DateTime.Now;
                file.Remark = dto.Remark;
                await _fileRepo.updateItemAsync(file);
                return Json(_resp.success("绑定成功"));

            }
            return Json(_resp.error("绑定失败"));
        }
    }
}
