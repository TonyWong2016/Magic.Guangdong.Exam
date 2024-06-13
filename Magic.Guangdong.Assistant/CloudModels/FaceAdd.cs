namespace Magic.Guangdong.Assistant.CloudModels
{
    public class FaceAdd
    {
        /// <summary>
        /// 图片信息(总数据大小应小于10M)，图片上传方式根据image_type来判断
        /// 必填
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// 图片类型，必填
        /// BASE64:图片的base64值，base64编码后的图片数据，编码后的图片大小不超过2M,需要注意的是，图片的base64编码是不包含图片头的，如data:image/jpg;base64；
        /// URL:图片的 URL地址(可能由于网络等原因导致下载图片时间过长)；
        /// FACE_TOKEN: 人脸图片的唯一标识，调用人脸检测接口时，会为每个人脸图片赋予一个唯一的FACE_TOKEN，同一张图片多次检测得到的FACE_TOKEN是同一个。
        /// </summary>
        public string image_type { get; set; }
        /// <summary>
        /// 必填，这里结合业务，这里可以通过角色+活动类型+年份进行区分，比如人工智能普通用户可以设定的group_id是user_ai_2021
        /// 用户组id，标识一组用户（由数字、字母、下划线组成），长度限制48B。
        /// 产品建议：根据您的业务需求，可以将需要注册的用户，按照业务划分，分配到不同的group下，
        /// 例如按照会员手机尾号作为groupid，用于刷脸支付、会员计费消费等，
        /// 这样可以尽可能控制每个group下的用户数与人脸数，提升检索的准确率
        /// </summary>
        public string group_id { get; set; }
        /// <summary>
        ///用户id（由数字、字母、下划线组成），长度限制128B，必填
        /// </summary>
        public string user_id { get; set; }
        /// <summary>
        ///用户资料，长度限制256B 默认空
        /// </summary>
        public string user_info { get; set; }
        /// <summary>
        /// 图片质量控制
        /// NONE: 不进行控制
        /// LOW:较低的质量要求
        /// NORMAL: 一般的质量要求
        /// HIGH: 较高的质量要求
        /// 默认 NONE
        /// 若图片质量不满足要求，则返回结果中会提示质量检测失败
        /// </summary>
        public string quality_control { get; set; } = "NONE";
        /// <summary>
        /// 活体控制 检测结果中不符合要求的人脸会被过滤
        /// NONE: 不进行控制
        /// LOW:较低的活体要求(高通过率 低攻击拒绝率)
        /// NORMAL: 一般的活体要求(平衡的攻击拒绝率, 通过率)
        /// HIGH: 较高的活体要求(高攻击拒绝率 低通过率)
        /// 默认NONE
        /// </summary>
        public string liveness_control { get; set; } = "NONE";
        /// <summary>
        /// 人脸检测排序类型
        /// 0:代表检测出的人脸按照人脸面积从大到小排列
        /// 1:代表检测出的人脸按照距离图片中心从近到远排列
        /// 默认为0
        /// </summary>
        public int face_sort_type { get; set; } = 0;
    }
}
