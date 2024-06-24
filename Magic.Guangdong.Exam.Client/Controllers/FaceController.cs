using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.CloudModels;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Magic.Guangdong.Exam.Client.Controllers
{
    public class FaceController : Controller
    {
        private readonly IBaiduFaceHelper _baiduFaceHelper;
        private readonly IResponseHelper _resp;
        private readonly IRedisCachingProvider _redisCachingProvider;
        public FaceController(IBaiduFaceHelper baiduFaceHelper,IResponseHelper responseHelper,IRedisCachingProvider redisCachingProvider)
        {
            _baiduFaceHelper = baiduFaceHelper;
            _resp = responseHelper;
            _redisCachingProvider = redisCachingProvider;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> faceDetect(FaceDetect model)
        {
            return Json(_resp.success(await _baiduFaceHelper.faceDetect(model)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> faceDetectStr(string str)
        {
            var model = JsonHelper.JsonDeserialize<FaceDetect>(str);
            if (model != null)
            {
                return Json(_resp.success(await _baiduFaceHelper.faceDetect(model)));
            }
            return Json(_resp.error("参数错误"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> faceMatch(FaceMatch[] models)
        {
            return Json(_resp.success(await _baiduFaceHelper.faceMatch(models)));
        }

        /// <summary>
        /// 检索人脸（1：N）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> faceQuery(FaceQuery model)
        {
            return Json(_resp.success(await _baiduFaceHelper.faceQuery(model)));
        }
    }
}
