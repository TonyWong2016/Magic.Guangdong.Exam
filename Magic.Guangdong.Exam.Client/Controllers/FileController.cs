using DotNetCore.CAP;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.Identity.Client;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class FileController : Controller
    {
        private IWebHostEnvironment en;
        private IFileRepo _fileRepo;
        private readonly ICapPublisher _capPublisher;
        private readonly IResponseHelper resp;
        /// <summary>
        /// 文件存放的根路径
        /// </summary>
        private readonly string _baseUploadDir;

        public FileController(IWebHostEnvironment en, ICapPublisher capPublisher, IFileRepo fileRepo, IResponseHelper resp)
        {
            this.en = en;
            _fileRepo = fileRepo;
            this.resp = resp;
            _capPublisher = capPublisher;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="ismult">该值表示可能前端是一次性提交多个文件也可能是循环提交多次</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int ismult = 0, string userId = "system", int del = 0)
        {
            try
            {
                var files = Request.Form.Files;

                if (files.Count() > 1 || ismult == 1)
                {
                    List<DbServices.Entities.File> list = await UploadMultFiles(files, userId);
                    return Json(resp.success(new { total = list.Count, items = list }));
                }
                else if (files.Count == 1)
                {
                    bool isDel = del == 0 ? false : true;
                    string path = await UploadFile(files[0], userId, isDel);

                    return Json(resp.success(path));
                }
            }
            catch (Exception ex)
            {
                return Json(resp.ret(-1, $"上传失败：error:{ex.Message}"));
            }
            return Json(resp);
        }

        /// <summary>
        /// 直接上传文件至服务器
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadFile()
        {
            var data = Request.Form.Files[0];
            string lastModified = Request.Form["lastModified"].ToString();
            var total = Request.Form["total"];
            var fileName = Request.Form["fileName"];
            var index = Request.Form["index"];
            var connId = Request.Form["connId"];
            var is_rewrite = Request.Form["rewrite"];
            var accountId = Request.Form["accountId"];
            string temporary = Path.Combine($"{Directory.GetCurrentDirectory()}\\wwwroot\\upfile", lastModified);//临时保存分块的目录
            if (await RedisHelper.HExistsAsync("upload_list", lastModified))
            {
                IList<string> list = new List<string> { lastModified };
                //resp.code = 3;
                //resp.message = "已上传";
                if (string.IsNullOrEmpty(is_rewrite) || is_rewrite.ToString() == "n")
                {
                    return Json(resp.ret(3, "已上传"));
                }
                else if (!string.IsNullOrEmpty(is_rewrite) && is_rewrite.ToString() == "y")
                {
                    FileHelper.DelectDir(temporary);
                    await RedisHelper.HDelAsync("upload_list", list.ToArray());
                }
            }
            try
            {
                int code = 0;
                string msg = "";
                bool mergeOk = false;
                if (!Directory.Exists(temporary))
                    Directory.CreateDirectory(temporary);

                string filePath = Path.Combine(temporary, index.ToString());
                //如果该文件上传过，且没有上传完成，执行如下续传逻辑
                //如果只有一个文件,直接删掉，如果大于一个文件，为了保证文件完整性，把最后一个文件删掉，继续上传；
                if (index.ToString() == "0" && System.IO.File.Exists(filePath) && is_rewrite.ToString() != "y")
                {
                    code = 2;
                    msg = $"{fileName}已上传，开始续传";
                    dynamic retdata = null;
                    var files = Directory.GetFiles(temporary);
                    if (files.Length > 1)
                    {
                        //把标号最大的文件删掉；
                        string last_file = files.OrderByDescending(u => u.Length).ThenByDescending(u => u).First();
                        System.IO.File.Delete(last_file);
                        retdata = new { index = files.Length - 2, total, mergeOk, fileName };
                    }
                    else
                        System.IO.File.Delete(filePath);
                    return Json(resp.ret(code, msg, retdata));
                }

                if (!Convert.IsDBNull(data))
                {
                    await Task.Run(() =>
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            data.CopyTo(fs);
                        }
                    });
                }

                if (total == index)
                {
                    mergeOk = await FileMerge(lastModified, fileName, accountId, connId);
                    if (mergeOk)
                        await RedisHelper.HSetAsync("upload_list", lastModified, "success");
                }

                return Json(resp.success(new { index, total, mergeOk, fileName }, $"{index}/{total}上传完成"));

            }
            catch (Exception ex)
            {
                Directory.Delete(temporary);//删除文件夹
                throw;
            }
        }

        /// <summary>
        /// 分片上传文件
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Route("upload")]
        public async Task<ActionResult<FileResponseDto>> UploadChunk()
        {
            var fileName = Request.Form["file_name"];
            //当前分块序号
            var index = Convert.ToInt32(Request.Form["file_index"]);
            //所有块数
            var maxChunk = Convert.ToInt32(Request.Form["file_total"]);
            var adminId = Request.Form["adminId"];

            var connId = Request.Form["connId"];
            //前端传来的md5值
            var md5 = Request.Form["file_md5"];
            //临时保存分块的目录
            var dir = Path.Combine(_baseUploadDir, md5);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            //分块文件名为索引名，更严谨一些可以加上是否存在的判断，防止多线程时并发冲突
            var filePath = Path.Combine(dir, index.ToString());
            //表单中取得分块文件
            var file = Request.Form.Files["file_data"];
            //var suffix = Request.Form.Files["file_suffix"];
            //获取文件扩展名
            //var suffix = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1, (file.FileName.Length - file.FileName.LastIndexOf(".") - 1));
            var filePathWithFileName = string.Concat(filePath, fileName);
            using (var stream = new FileStream(filePathWithFileName, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                LogicLog.WriteLogicLog(index.ToString(), _baseUploadDir + md5 + ".txt", false);
            }

            //如果是最后一个分块， 则合并文件
            var fileResponseDto = new FileResponseDto();
            fileResponseDto.fileIndex = index;
            if (index == maxChunk)
            {
                fileResponseDto = await MergeFileAsync(fileName, md5, adminId, connId);
                fileResponseDto.Completed = true;
            }
            return Ok(fileResponseDto);
        }

        /// <summary>
        /// 验证文件上传情况
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckFile()
        {
            //前端传来的md5值
            var file_name = Request.Form["file_name"];
            var md5 = Request.Form["file_md5"];
            var maxChunk = Convert.ToInt32(Request.Form["file_total"]);
            string logPath = _baseUploadDir + md5 + ".txt";
            var logs = LogicLog.ReadLogicLog(logPath);
            if (logs != null)
            {
                int shards = logs.Count();
                if (shards >= Convert.ToInt32(maxChunk))
                {
                    string path = $"/{DateTime.Now.ToString("yyyyMM")}/{md5}/{file_name}";
                    var file = await _fileRepo.getOneAsync(u => u.Path == path);
                    return Json(resp.ret(2, "该文件已上传", new { path, fileId = file.Id }));
                }
                else
                {

                    return Json(resp.ret(0, $"该文件已上传步长{shards}/{maxChunk}，继续上传", new { file_index = shards - 1, percent = Convert.ToDouble(shards) / Convert.ToDouble(maxChunk) * 100 }));
                }
            }
            return Json(resp);
        }

        /// <summary>
        /// 合并分片的文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="md5">文件md5</param>
        /// <param name="adminId">用户id</param>
        private async Task<FileResponseDto> MergeFileAsync(string fileName, string md5, string adminId, string connId)
        {
            var fileResponseDto = new FileResponseDto();
            //临时文件夹
            var dir = Path.Combine(_baseUploadDir, md5);
            //获得下面的所有文件
            var files = Directory.GetFiles(dir);
            var yearMonth = DateTime.Now.ToString("yyyyMM");
            //最终的文件名
            var finalDir = Path.Combine(_baseUploadDir, yearMonth, md5);
            if (!Directory.Exists(finalDir))
                Directory.CreateDirectory(finalDir);
            var finalPath = Path.Combine(finalDir, fileName);
            int fileSize = 0;
            using (var fs = new FileStream(finalPath, FileMode.Create))
            {
                //排一下序，保证从0-N Write
                var fileParts = files.OrderBy(x => x.Length).ThenBy(x => x);
                foreach (var part in fileParts)
                {
                    var bytes = System.IO.File.ReadAllBytes(part);
                    fileSize += bytes.Length;
                    await fs.WriteAsync(bytes, 0, bytes.Length);
                    bytes = null;
                    //删除分块
                    System.IO.File.Delete(part);
                }
                await fs.FlushAsync();
                fs.Close();
                //删除临时文件夹和分片文件
                Directory.Delete(dir);
                var suffix = Path.GetExtension(fileName);

                fileResponseDto.path = $"/{yearMonth}/{md5}/{fileName}";

                var file = new DbServices.Entities.File()
                {
                    Name = fileName,
                    Ext = suffix,
                    Size = fileSize,
                    ShortUrl = fileResponseDto.path,
                    AccountId = adminId,
                    ConnId = connId,
                    Type = "client"
                };



                fileResponseDto.fileId = Convert.ToInt32(_fileRepo.addItemsIdentity(file));
                #region 上传到远程附件服务器（或本地服务器，注释的代码是本地）

                try
                {
                    string storageType = ConfigurationHelper.GetSectionValue("storageType");
                    if (storageType == "local")
                    {
                        FileInfo fi = new FileInfo(finalPath);
                        string target = ConfigurationHelper.GetSectionValue("targetBase") + $"\\{yearMonth}\\{md5}\\";
                        if (!Directory.Exists(target))
                            Directory.CreateDirectory(target);
                        string target_file = target + fileName;
                        fi.CopyTo(target_file, true);
                        file.Path = target_file;
                    }
                    else if (storageType == "server")
                    {
                        string remoteBase = ConfigurationHelper.GetSectionValue("remoteBase");
                        string remoteUser = ConfigurationHelper.GetSectionValue("remoteUser");
                        string remotePwd = ConfigurationHelper.GetSectionValue("remotePwd");
                        bool flag = FileHelper.connectState(remoteBase, remoteUser, remotePwd);
                        if (flag)
                        {
                            //共享文件夹的目录
                            DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
                            //获取保存文件的路径
                            string PathName = theFolder.ToString() + $"{yearMonth}\\{md5}\\";
                            //执行方法
                            //await Task.Run(() => FileHelper.Transport(finalPath, PathName, fileName));
                            //上传文件到附件服务器，同时删掉本地文件节省服务器空间
                            await FileHelper.Transport(finalPath, PathName, fileName, true);
                            file.Path = Path.Combine(PathName, fileName);
                        }
                        else
                        {
                            Assistant.Logger.Error("远程连接建立失败");
                        }
                    }

                    await _fileRepo.addItemAsync(file);
                }
                catch
                {
                    throw;
                }

                #endregion

            }
            return fileResponseDto;
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="isDel">是否删除原文件</param>
        /// <returns></returns>
        public async Task<string> UploadFile(IFormFile file, string userId, bool isDel = false)
        {
            string uploadFileName = file.FileName.Replace(".", $"-{DateTime.Now.ToString("yyyyMMddHHmmss")}.");//a.jpg-->a-20220216161309.jpg
            string basepath = en.WebRootPath;
            //string path_server = basepath + "\\upfile\\" + DateTime.Now.ToString("yyyyMM") + "\\"+Utils.GetCurrentWeekOfMonth(DateTime.Now) + "\\";
            string path_server = $"{basepath}\\upfile\\{DateTime.Now.ToString("yyyyMM")}\\{Utils.GetCurrentWeekOfMonth(DateTime.Now)}\\";
            if (!Directory.Exists(path_server))
                Directory.CreateDirectory(path_server);
            string save_file = path_server + uploadFileName;
            if (System.IO.File.Exists(save_file))
            {
                System.IO.File.Delete(save_file);
                //return $"/{DateTime.Now.ToString("yyyyMM")}/" + path;
            }
            using (FileStream fstream = new FileStream(save_file, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await file.CopyToAsync(fstream);

            }
            FileInfo fi = new FileInfo(save_file);
            string fileMd5 = await Security.GetFileMD5(save_file);
            string returnPath = await FileHelper.SyncFile(save_file, uploadFileName, true);
            await _fileRepo.addItemAsync(new DbServices.Entities.File()
            {
                AccountId = userId,
                ShortUrl = returnPath,
                Path = fi.FullName,
                Size = fi.Length,
                Ext = fi.Extension,
                Name = fi.Name,
                Md5 = fileMd5,
                Type = "client"
            });

            return returnPath;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<List<DbServices.Entities.File>> UploadMultFiles(IFormFileCollection files, string userId)
        {
            int cnt = files.Count();
            //List<string> list = new List<string>(cnt);
            List<DbServices.Entities.File> attachMains = new List<DbServices.Entities.File>(cnt);
            foreach (var file in files)
            {

                string filename = file.FileName;//--"20191119113847612.jpg"

                string fileExtention = System.IO.Path.GetExtension(file.FileName);//--.jpg
                                                                                  //string path = Guid.NewGuid().ToString() + fileExtention;

                string path = Security.GetMD5HashFromStream(file.OpenReadStream()) + fileExtention;
                string basepath = en.WebRootPath;
                //en.WebRootPath-》wwwroot的目录; .ContentRootPath到达WebApplication的项目目录
                //string path_server = basepath + "\\upfile\\" + DateTime.Now.ToString("yyyyMM") + "\\";
                string path_server = $"{basepath}\\upfile\\{DateTime.Now.ToString("yyyyMM")}\\{Utils.GetCurrentWeekOfMonth(DateTime.Now)}\\";
                //if (isLessonResource)//这里可以结合具体的业务
                //{
                //    var attach = AddAttach(filename, $"/{DateTime.Now.ToString("yyyyMM")}/" + path, Path.GetExtension(filename));
                //    attachMains.Add(attach);
                //}
                if (!Directory.Exists(path_server))
                    Directory.CreateDirectory(path_server);
                string save_file = path_server + path;
                if (System.IO.File.Exists(save_file))
                {
                    continue;
                }
                using (FileStream fstream = new FileStream(save_file, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(fstream);
                }
                string returnPath = await FileHelper.SyncFile(save_file, path, true);
                FileInfo fileInfo = new FileInfo(save_file);
                string fileMd5 = await Security.GetFileMD5(save_file);
                attachMains.Add(new DbServices.Entities.File()
                {
                    AccountId = userId,
                    ShortUrl = returnPath,
                    Path = save_file,
                    Size = fileInfo.Length,
                    Ext = fileInfo.Extension,
                    Name = fileInfo.Name,
                    Md5 = fileMd5,
                    Type = "client"
                });
                //await CopyFileAsync(save_file, path);

            }
            await _fileRepo.addItemsAsync(attachMains);
            return attachMains;
        }

        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="lastModified"></param>
        /// <param name="fileName"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<bool> FileMerge(string lastModified, string fileName, string accountId = "", string connId = "")
        {
            bool ok = false;
            try
            {
                var temporary = Path.Combine($"{Directory.GetCurrentDirectory()}\\wwwroot\\upfile", lastModified);//临时文件夹
                fileName = Request.Form["fileName"];//文件名
                string fileExt = Path.GetExtension(fileName);//获取文件后缀
                var files = Directory.GetFiles(temporary);//获得下面的所有分片文件
                string tmp_filepath = Guid.NewGuid().ToString("N");
                string finalPath = $"{en.WebRootPath}\\upfile\\{DateTime.Now.ToString("yyyyMM")}\\{lastModified}";
                if (!Directory.Exists(finalPath))
                    Directory.CreateDirectory(finalPath);
                finalPath += $"\\{fileName}";
                int fileSize = 0;
                using (var fs = new FileStream(finalPath, FileMode.Create))
                {
                    foreach (var part in files.OrderBy(x => x.Length).ThenBy(x => x))//排一下序，保证从0-N Write
                    {
                        var bytes = System.IO.File.ReadAllBytes(part);
                        fileSize += bytes.Length;
                        await fs.WriteAsync(bytes, 0, bytes.Length);
                        bytes = null;
                        System.IO.File.Delete(part);//删除分块
                    }
                    //fs.Close();
                }
                Directory.Delete(temporary);//删除文件夹                
                ok = true;
                string fileMd5 = await Security.GetFileMD5(finalPath);
                //复制到附件服务器
                await CopyFileAsync(finalPath, lastModified + "\\" + fileName, true);
                Directory.Delete(Path.GetDirectoryName(finalPath));

                #region 写入数据
                string path = $"/{DateTime.Now.ToString("yyyyMM")}/{lastModified}/{fileName}";
                Guid att_id = Guid.NewGuid();
                var file = new DbServices.Entities.File()
                {
                    Name = fileName,
                    Ext = fileExt,
                    Size = fileSize,
                    Type = "client",
                    Path = path,
                    AccountId = accountId,
                    ConnId = connId,
                    Md5 = fileMd5
                };
                await _fileRepo.addItemAsync(file);
                #endregion
            }
            catch (Exception ex)
            {
                throw;
            }
            return ok;
        }

        private async Task CopyFileAsync(string save_file, string fileName, bool isDel = false)
        {
            string remoteBase = ConfigurationHelper.GetSectionValue("remoteBase");
            bool flag = FileHelper.connectState();
            if (flag)
            {
                //共享文件夹的目录
                DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
                //获取保存文件的路径
                string PathName = theFolder.ToString() + $"{DateTime.Now.ToString("yyyyMM")}\\";
                //string PathName =$"{theFolder.ToString()}{ConfigurationHelper.GetSectionValue("remoteFolder")}{DateTime.Now.ToString("yyyyMM")}\\";
                if (!string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
                {
                    PathName += $"{Path.GetDirectoryName(fileName)}\\";
                    //上传到附件服务器
                    await FileHelper.Transport(save_file, PathName, Path.GetFileName(fileName), isDel);
                }
                else
                    await FileHelper.Transport(save_file, PathName, fileName, isDel);
            }
            else
            {
                Assistant.Logger.Error("远程连接建立失败");
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="encodeUri"></param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadFile(string encodeUri)
        {
            if (encodeUri == null || encodeUri.Length == 0)
                return Content("没文件名下个什么玩意儿.");
            var memoryStream = new MemoryStream();
            if (ConfigurationHelper.GetSectionValue("storage") == "server")
            {
                string remoteBase = ConfigurationHelper.GetSectionValue("remoteBase");
                bool flag = FileHelper.connectState();
                if (!flag)
                {
                    return Content("服务器链接失败");
                }
                string decodeUri = System.Web.HttpUtility.UrlDecode(encodeUri).Replace(ConfigurationHelper.GetSectionValue("resourcehost"), "").Replace("/", "\\");
                string filePath = remoteBase + decodeUri;
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memoryStream);
                }

            }
            else if (encodeUri.StartsWith("http"))
            {
                HttpClient client = new HttpClient();
                byte[] fileData = await client.GetByteArrayAsync(encodeUri);
                client.Dispose();
                memoryStream = new MemoryStream(fileData);

            }
            memoryStream.Position = 0;

            return File(memoryStream, "text/plain", Path.GetFileName(encodeUri));
        }

        //public async Task<IActionResult> BuildConnWithFile(long fileId, string connId, string connName)
        //{
        //    if (!await _fileRepo.getAnyAsync(u => u.Id == fileId))
        //    {
        //        return Json(resp.error("文件不存在"));
        //    }
        //    var file = await _fileRepo.getOneAsync(u => u.Id == fileId);
        //    file.ConnId = connId;
        //    file.ConnName = connName;
        //    //await _capPublisher.PublishAsync("InsertOrUpdateFileModel", file);
        //    return Json(resp.success("修改请求提交成功"));
        //}

        /// <summary>
        /// 修改文件
        /// </summary>
        /// <param name="paperIds"></param>
        /// <returns></returns>
        //[NonAction]
        //[CapSubscribe(CapConsts.PREFIX + "BuildFileConnection")]
        //public async Task UpdateFileModel(BuildFileConnectionDto dto)
        //{
        //    if (!await _fileRepo.getAnyAsync(u => u.Id == dto.fileId))
        //    {
        //        return;
        //    }
        //    var file = await _fileRepo.getOneAsync(u => u.Id == dto.fileId);
        //    file.ConnId = dto.connId;
        //    file.ConnName = dto.connName;
        //    await _fileRepo.updateItemAsync(file);
        //}

    }


    public class FileResponseDto
    {
        public bool Completed { get; set; }

        public string path { get; set; }

        public int fileId { get; set; }

        public int fileIndex { get; set; }
    }

    public class LogicLog
    {
        /// <summary>
        /// 逻辑日志
        /// </summary>
        /// <param name="info"></param>
        /// <param name="logPath"></param>
        public static void WriteLogicLog(string info, string logPath, bool is_overwrite = false)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            if (string.IsNullOrEmpty(logPath))
                logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            if (is_overwrite && System.IO.File.Exists(logPath))
                System.IO.File.Delete(logPath);
            try
            {
                using (StreamWriter sw = System.IO.File.AppendText(logPath))
                {
                    sw.WriteLine(info);
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (IOException e)
            {
                using (StreamWriter sw = System.IO.File.AppendText(logPath))
                {
                    sw.WriteLine("异常：" + e.Message);
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"));
                    sw.WriteLine("**************************************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
        /// <summary>
        /// 读取日志
        /// </summary>
        /// <param name="logPath"></param>
        /// <returns></returns>
        public static List<string> ReadLogicLog(string logPath)
        {
            if (!System.IO.File.Exists(logPath))
                return null;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //读取路径文件
            var file_path = System.IO.File.Open(logPath, FileMode.Open);
            List<string> list_path = new List<string>();
            using (var stream = new StreamReader(file_path, Encoding.GetEncoding("GB2312")))
            {
                while (!stream.EndOfStream)
                {
                    list_path.Add(stream.ReadLine());
                }
            }
            return list_path;
        }
    }

    public class BuildFileConnectionDto
    {
        public long fileId { get; set; }
        public string connId { get; set; }
        public string connName { get; set; }
    }
}
