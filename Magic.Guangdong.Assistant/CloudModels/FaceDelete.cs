namespace Magic.Guangdong.Assistant.CloudModels
{
    public class FaceDelete
    {
        /// <summary>
        /// 请求标识码，随机数，唯一
        /// </summary>
        public uint log_id { get; set; } = Utils.GetTimeStampUint(DateTime.Now);

        /// <summary>
        /// 用户id（由数字、字母、下划线组成），长度限制128B
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// 用户组id（由数字、字母、下划线组成） 长度限制48B，删除指定group_id中的user_id信息
        /// </summary>
        public string group_id { get; set; }
        
        /// <summary>
        /// 需要删除的人脸图片token，（由数字、字母、下划线组成）长度限制64B
        /// </summary>
        public string face_token { get; set; }
    }
}
