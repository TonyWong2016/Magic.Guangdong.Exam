using DotNetCore.CAP;
using EasyCaching.Core;
using FreeSql.Internal;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using NPOI.HPSF;
using Org.BouncyCastle.Ocsp;
using System.Security.AccessControl;
using System.Web;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Magic.Guangdong.Exam.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_minioClient"></param>
    /// <param name="resp"></param>
    /// <param name="_fileRepo"></param>
    public class UploadController(IMinioClient _minioClient, IResponseHelper _resp, IFileRepo _fileRepo) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetUrl(string bucketName,string objectName)
        {
            PresignedGetObjectArgs args = new PresignedGetObjectArgs()
                       .WithBucket(bucketName)
                       .WithObject(objectName)
                       .WithExpiry(60 * 60 * 24 * 7);
            return Json(_resp.success(await _minioClient.PresignedGetObjectAsync(args)));
        }

        [HttpPost]
        public async Task<IActionResult> ToMinio()
        {
            FilePartModel model = new FilePartModel();
            try
            {                
                model.file = Request.Form.Files["file_data"];
                model.chunkIndex = Convert.ToInt32(Request.Form["file_index"]);
                model.chunkTotal = Convert.ToInt32(Request.Form["file_total"]);
                model.uploadId = Request.Form["file_md5"];
                model.fileExt = Request.Form["file_suffix"];
                model.fileName = Request.Form["file_name"];
                model.fileSize = Convert.ToInt64(Request.Form["file_size"]);
                
                if (model.file == null || model.file.Length <= 0)
                    return BadRequest("No file found.");
                await CreateBucketIfNotExists(model.bucket);

                using (var stream = model.file.OpenReadStream())
                {
                    //model.uploadId=Assistant.Security.GetMD5HashFromStream(stream);
                    // 上传单个分片到MinIO
                    await _minioClient.PutObjectAsync(new PutObjectArgs()
                        .WithBucket(model.bucket)
                        .WithObject(model.savedFileName)
                        .WithContentType(model.contentType)
                        .WithObjectSize(stream.Length)
                        .WithStreamData(stream));
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{model.savedFileName} is uploaded successfully");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                
                if (model.merged)
                {
                    var listArgs = new ListObjectsArgs()
                    .WithBucket(model.bucket)
                    .WithPrefix(model.uploadId)
                    .WithRecursive(true)
                    .WithVersions(false);

                    using (var mergedStream = new MemoryStream())
                    {
                        await foreach (var obj in _minioClient.ListObjectsEnumAsync(listArgs).ConfigureAwait(false))
                        {
                            var partStream = await _minioClient.GetObjectAsync(new GetObjectArgs()
                                .WithBucket(model.bucket)
                                .WithObject(obj.Key)
                                .WithCallbackStream((stream) =>
                                {
                                    stream.CopyTo(mergedStream);
                                }));

                            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                                .WithBucket(model.bucket)
                                .WithObject(obj.Key));

                        }

                        // 将合并后的流写入到MinIO
                        mergedStream.Seek(0, SeekOrigin.Begin);
                        await _minioClient.PutObjectAsync(new PutObjectArgs()
                            .WithBucket(model.bucket)
                            .WithObject(model.savedFileName)
                            .WithContentType(model.contentType)
                            .WithObjectSize(mergedStream.Length)
                            .WithStreamData(mergedStream));
                    }
                }

                var respData = new FileResponseDto()
                {
                    fileId = 0,
                    fileIndex = model.chunkIndex,
                    Completed = model.completed,
                    path = "未完成",
                };
                if (model.merged || model.chunkTotal == 1)
                {
                    PresignedGetObjectArgs args = new PresignedGetObjectArgs()
                           .WithBucket(model.bucket)
                           .WithObject(model.savedFileName)
                           .WithExpiry(60 * 60 * 24);
                    string temporaryUrl = await _minioClient.PresignedGetObjectAsync(args);
                    Uri uriResult;

                    bool result = Uri.TryCreate(temporaryUrl, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    var file = new DbServices.Entities.File()
                    {
                        Name = model.savedFileName,
                        Ext = model.fileExt,
                        Size = model.fileSize,
                        ShortUrl = result ? uriResult.PathAndQuery : temporaryUrl,
                        AccountId = "system",
                        ConnId = "",
                        Md5 = model.uploadId
                    };
                    await _fileRepo.addItemAsync(file);
                    respData.fileId = file.Id;
                    respData.path = temporaryUrl;
                }                
                return Json(_resp.success(respData, "上传成功"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to upload part {model.chunkIndex} to MinIO: {ex.Message}");
            }
            return Json(_resp.error("上传失败"));
        }

        private async Task CreateBucketIfNotExists(string bucketName)
        {
            bool isExist = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!isExist)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            }
        }
    }

    public class FilePartModel
    {
        public IFormFile? file { get; set; }

        public int chunkIndex { get; set; }

        public int chunkTotal { get; set; }

        public long fileSize { get; set; }

        public string? fileName { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string? contentType
        {
            get
            {
                if(new string[] { "png","jpg","jpeg","tiff","webm" }.Contains(fileExt))
                {
                    return "image/" + fileExt;
                }
                return  "application/octet-stream";
            }
            set
            {

            }
        }

        /// <summary>
        /// 扩展名
        /// </summary>
        public string? fileExt { get; set; }

        public string? bucket { get; set; } = "firstbucket";

        public string? uploadId { get; set; } = Guid.NewGuid().ToString("N");


        public string savedFileName
        {
            get
            {
                string suffix = $"{chunkIndex}.{fileExt}";
                if(chunkTotal>1 && chunkTotal == chunkIndex)
                {
                    suffix = $"final.{fileExt}";
                }
                if (string.IsNullOrEmpty(fileName))
                {
                    return $"{uploadId}_{suffix}";
                }
                return HttpUtility.HtmlEncode($"{uploadId}_{Path.GetFileNameWithoutExtension(fileName)}_{suffix}");
            }
        }

        public string mergedFileName
        {
            get
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    return $"{uploadId}_merged.{fileExt}";
                }
                return $"{uploadId}_{fileName}_merged.{fileExt}";
            }
        }

        public bool merged
        {
            get
            {
                if (chunkTotal > 1 && chunkTotal == chunkIndex)
                {
                    return true;
                }
                return false;
            }
        }

        public bool completed
        {
            get
            {
                if (chunkTotal > 0 && chunkTotal == chunkIndex)
                    return true;
                return false;
            }
            
        }
    }

    public class FileResponseModel
    {
        public bool Completed { get; set; }

        public string path { get; set; }

        public int fileId { get; set; }

        public int fileIndex { get; set; }
    }
}
