using System;
using System.Collections.Generic;
using System.Text;

namespace Magic.Guangdong.Assistant.Lib
{
    public class YoungSvCertModel
    {
        public string certTempUrl { get; set; }

        public byte[] certTempData { get; set; }

        public List<YoungSvCertContentModel> contentList { get; set; }

        public List<YoungSvCertImgModel> imgList { get; set; }
    }

    public class YoungSvCertContentModel
    {
        /// <summary>
        /// 文字内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 横坐标点（pdf标准，距离最左侧的距离）
        /// </summary>
        public float location_x { get; set; }
        /// <summary>
        /// 纵坐标点（pdf标准，距离最底侧的距离，注意是从底部开始计算）
        /// </summary>
        public float location_y { get; set; }
        /// <summary>
        /// 文字内容所在文档中的宽度（超出后会向上折行）
        /// </summary>
        public float width { get; set; }
        /// <summary>
        /// 文字大小
        /// </summary>
        public float fontSize { get; set; }
        //public double height { get; set; }
    }

    public class YoungSvCertImgModel
    {
        /// <summary>
        /// 图片链接
        /// </summary>
        public string imgUrl { get; set; }

        public byte[] imgData { get; set; }
        //public byte[] imgData { 
        //    get 
        //    {
        //        string resourceHost = ConfigurationHelper.GetSectionValue("resourceHost");
        //        if (imgUrl.StartsWith(resourceHost))
        //        {
        //            string remotePath = FileHelper.getFilePath(imgUrl.Replace(resourceHost, "").Replace("/", "\\"));
        //            return FileHelper.getFileByte(remotePath);
        //        }
        //        return null;
        //    }
        //    set { }
        //}

        public float location_x { get; set; }

        public float location_y { get; set; }

        public float width { get; set; }

        public float height { get; set; }

        public float opacity { get; set; } = 0.8f;
    }
}
