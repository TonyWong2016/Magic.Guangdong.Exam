﻿namespace Magic.Guangdong.Assistant.CloudModels
{
    /// <summary>
    /// 人脸检测请求模型
    /// </summary>
    public class FaceDetect
    {
        /// <summary>
        /// 图片信息(总数据大小应小于10M)，图片上传方式根据image_type来判断
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// 图片类型
        /// BASE64:图片的base64值，base64编码后的图片数据，编码后的图片大小不超过2M.需要注意的是，图片的base64编码是不包含图片头的，如data:image/jpg;base64；
        /// URL:图片的 URL地址(可能由于网络等原因导致下载图片时间过长)；
        /// FACE_TOKEN: 人脸图片的唯一标识，调用人脸检测接口时，会为每个人脸图片赋予一个唯一的FACE_TOKEN，同一张图片多次检测得到的FACE_TOKEN是同一个。
        /// </summary>
        public string image_type { get; set; }
        /// <summary>
        /// 包括age,beauty,expression,face_shape,gender,glasses,landmark,landmark150,
        /// quality,eye_status,emotion,face_type,mask,spoofing信息
        /// 逗号分隔.默认只返回face_token、人脸框、概率和旋转角度
        /// 这里额外增加性别，是否戴眼镜，图片质量三个参数
        /// </summary>
        public string face_field { get; set; } = "face_token,gender,expression,glasses,mask,quality,age";

        /// <summary>
        /// 最多处理人脸的数目，默认值为1，根据人脸检测排序类型检测图片中排序第一的人脸（默认为人脸面积最大的人脸），最大值120
        /// </summary>
        public uint max_face_num { get; set; } = 1;
        /// <summary>
        /// 人脸的类型
        /// LIVE表示生活照：通常为手机、相机拍摄的人像图片、或从网络获取的人像图片等
        /// IDCARD表示身份证芯片照：二代身份证内置芯片中的人像照片
        /// WATERMARK表示带水印证件照：一般为带水印的小图，如公安网小图
        /// CERT表示证件照片：如拍摄的身份证、工卡、护照、学生证等证件图片
        /// 默认LIVE
        /// </summary>
        public string face_type { get; set; } = "LIVE";
        /// <summary>
        /// 活体控制 检测结果中不符合要求的人脸会被过滤
        /// NONE: 不进行控制
        /// LOW:较低的活体要求(高通过率 低攻击拒绝率)
        /// NORMAL: 一般的活体要求(平衡的攻击拒绝率, 通过率)
        /// HIGH: 较高的活体要求(高攻击拒绝率 低通过率)
        /// 默认NONE
        /// </summary>
        public string liveness_control { get; set; } = "NORMAL";
        /// <summary>
        /// 人脸检测排序类型
        /// 0:代表检测出的人脸按照人脸面积从大到小排列
        /// 1:代表检测出的人脸按照距离图片中心从近到远排列
        /// 默认为0
        /// </summary>
        public int face_sort_type { get; set; } = 0;
    }

   
}
