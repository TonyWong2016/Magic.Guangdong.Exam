using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.IService
{
    public interface IBaiduFaceHelper
    {
        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> faceDetect(CloudModels.FaceDetect model);
        /// <summary>
        /// 人脸注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> faceAdd(CloudModels.FaceAdd model);
        /// <summary>
        /// 人脸更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> faceUpdate(CloudModels.FaceUpdate model);
        /// <summary>
        /// 人脸删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> faceDelete(CloudModels.FaceDelete model);
        /// <summary>
        /// 人脸检索（1：N）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> faceQuery(CloudModels.FaceQuery model);
        /// <summary>
        /// 人脸比对
        /// 人证合一业务使用，比如场馆签到，先扫码确认信息，在扫脸确认人员
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        Task<string> faceMatch(CloudModels.FaceMatch[] models);
    }
}
