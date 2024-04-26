using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.Dto
{

    public class CertTemplateDto
    {
        public byte[] certTempData { get; set; } = null;
        public string certTempUrl { get; set; }

        public List<TemplateContentModel> contentList { get; set; }

        public List<TemplateImgModel> imgList { get; set; }
    }

    public class TemplateContentModel
    {
        /// <summary>
        /// 文字内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 横坐标点（距离最左侧的距离）
        /// </summary>
        public float location_x { get; set; }
        /// <summary>
        /// 纵坐标点（当转换模型为pdf，这里距离最底侧的距离，注意是从底部开始计算，为图片是是从顶部开始算）
        /// </summary>
        public float location_y { get; set; }
        /// <summary>
        /// 文字内容所在文档中的宽度（pdf时超出后会向上折行，图片会向下）
        /// 该字段也用于设定二维码宽度
        /// </summary>
        public float width { get; set; }

        /// <summary>
        /// 行内最大字数，超出后这行显示（图片模板生效）
        /// </summary>
        public int row_maxwords { get; set; } = 0;

        /// <summary>
        /// 行间距
        /// </summary>
        public float row_spacing { get; set; } = 0;

        /// <summary>
        /// 文字大小
        /// </summary>
        public float fontSize { get; set; }
        /// <summary>
        /// 16进制颜色值
        /// </summary>
        public string fontColor { get; set; } = "#000000";
        //public double height { get; set; }
        /// <summary>
        /// 渲染顺序
        /// </summary>
        public int orderIndex { get; set; } = 0;
        /// <summary>
        /// 内容对应的key，用来导入数据时和导入模板的字段做对应
        /// </summary>
        public string key { get; set; }

        public SpecialHandle specialHandle { get; set; } = SpecialHandle.None;
    }

    public class TemplateImgModel
    {
        public byte[] imgData { get; set; }
        /// <summary>
        /// 图片链接或者二维码内容，当为二维码内容时，底层方法处理后返回二维码链接地址
        /// </summary>
        public string imgUrl { get; set; }

        public float location_x { get; set; }

        public float location_y { get; set; }
        /// <summary>
        /// 指定宽度（会等比缩放，一般只需要指定宽度或者高度即可）
        /// </summary>
        public float width { get; set; } = 0;
        /// <summary>
        /// 指定高度
        /// </summary>
        public float height { get; set; } = 0;

        public float opacity { get; set; } = 0.8f;
        /// <summary>
        /// 渲染顺序
        /// </summary>
        public int orderIndex { get; set; } = 0;
        /// <summary>
        /// 图片类型
        /// 0-合成二维码，1-普通图片
        /// </summary>
        public int imgType { get; set; } = 0;

        /// <summary>
        /// 图片对应的key，需要合成多个图片时的对应
        /// </summary>
        public string key { get; set; }
    }

    /// <summary>
    /// 特殊处理
    /// 这里就是标记一些证书特殊的处理情况，很无聊，但又要尽可能不破坏原来的规则
    /// </summary>
    public enum SpecialHandle
    {
        None,
        DoubleKeyBreak,//遇到两个不同的对应属性值时，自动折行
    }
}
