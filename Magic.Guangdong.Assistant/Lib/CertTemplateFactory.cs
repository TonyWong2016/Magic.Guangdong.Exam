using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.Lib
{
    public class CertTemplateFactory
    {

        /// <summary>
        /// 生成模板参数(适用于航天创新MatchHT,如果再有其他活动，需要重写该方法，调整为适应新模板的形式)  
        /// </summary>
        /// <param name="TemplateConfigJsonStrForPdf"></param>
        /// <param name="certModel"></param>
        /// <param name="templateJsonStr"></param>
        /// <returns></returns>
        public static YoungSvCertModel makeYoungSvCertTempConfig(string TemplateConfigJsonStrForPdf,MatchAICertModel certModel,out string templateJsonStr)
        {
            var model = new YoungSvCertModel();
            if (!string.IsNullOrEmpty(TemplateConfigJsonStrForPdf))
            {
                model = JsonHelper.JsonDeserialize<YoungSvCertModel>(TemplateConfigJsonStrForPdf);
                templateJsonStr = TemplateConfigJsonStrForPdf;
            }
            else
            {
                if (certModel.certType < 4)//等级奖
                    model = makeYoungSvCertLevelConfig(certModel);
                else if (certModel.certType == 4)//展示奖
                    model = makeYoungSvCertZsConfig(certModel);
                else if (certModel.certType == 5)//优秀展示奖
                    model = makeYoungSvCertYszsConfig(certModel);
                else
                {
                    templateJsonStr = "";
                    return null;
                }                
            }
            templateJsonStr = JsonHelper.JsonSerialize(model);//一定要在赋值之前生成

            //这样处理看似复杂，实际是为了以后如果要调整内容的位置，便不再需要调整代码，只需要修改库里对应的
            //参数即可，直接修改对应模板的config字段，而程序只根据初始模板，设定一次即可。
            //但有一点不够优雅的地方就是，除位置，宽度等固定内容之外的动态内容，需要每次在程序中设定，而这里
            //的设定方法只能是根据序列来赋值，因此每个工厂类里的参数设定方法都只适用于指定的活动，如这里就是
            //试用与航天创新，如果下次还有新的活动，就再写一个方法，修改参数即可。
            model.contentList[0].content = certModel.name.TrimEnd(',');
            #region 处理下学校展示逻辑，相同学校只展示1所
            string school = "";
            if (certModel.school.Contains(","))
            {
                string[] parts = certModel.school.Split(',');
                foreach (var part in parts)
                {
                    if (school.Contains(part))
                        continue;
                    school += part + ",";
                }
            }
            else
            {
                school = certModel.school;
            }
            #endregion
            model.contentList[1].content = school.TrimEnd(',');
            model.contentList[2].content = certModel.teacher.TrimEnd(',');
            if (certModel.certType < 4)
            {
                model.contentList[3].content = certModel.eventName.Replace("-", "\r\n");
                model.contentList[4].content = certModel.number;
            }
            else
                model.contentList[3].content = certModel.number;
            
            return model;
        }
        /// <summary>
        /// 等级奖参数
        /// </summary>
        /// <param name="certModel"></param>
        /// <returns></returns>
        static YoungSvCertModel makeYoungSvCertLevelConfig(MatchAICertModel certModel)
        {
            var model = new YoungSvCertModel();
            model.certTempUrl = certModel.certTemplateUrlFinal;
            model.contentList = new List<YoungSvCertContentModel>();
            model.contentList.Add(new YoungSvCertContentModel()
            {
                content = "{姓名}",
                location_x = 270,
                location_y = 325,//注意，这个纵坐标的取值起点是图片的底部，也就是从下向上的距离
                width = 200,
                fontSize = 14
            });
            model.contentList.Add(new YoungSvCertContentModel()
            {
                content = "{学校}",
                location_x = 270,
                location_y = 299,
                width = 300,
                fontSize = 14
            });
            model.contentList.Add(new YoungSvCertContentModel()
            {
                content = "{老师}",
                location_x = 270,
                location_y = 275,
                width = 200,
                fontSize = 14
            });

            model.contentList.Add(new YoungSvCertContentModel()
            {
                content = "{赛项}",
                location_x = 270,
                location_y = 226,
                width = 300,
                fontSize = 14
            });
            model.contentList.Add(new Magic.Guangdong.Assistant.Lib.YoungSvCertContentModel()
            {
                content = "{编号}",
                location_x = 270,
                location_y = 202,
                width = 200,
                fontSize = 14
            });
            if (certModel.imgList != null && certModel.imgList.Length > 0)
            {
                model.imgList = new List<YoungSvCertImgModel>();
                foreach (var img in certModel.imgList)
                {
                    model.imgList.Add(new YoungSvCertImgModel()
                    {
                        imgUrl = img,
                        location_x = 390,
                        location_y = 60,
                        width = 116,
                        height = 116,
                        opacity = 0.85f
                    });
                }
            }
            return model;
        }
        /// <summary>
        /// 生成展示奖参数
        /// </summary>
        /// <param name="certModel"></param>
        /// <returns></returns>
        static YoungSvCertModel makeYoungSvCertZsConfig(MatchAICertModel certModel)
        {
            var model = new YoungSvCertModel();
            model.certTempUrl = certModel.certTemplateUrlFinal;
            model.contentList = new List<YoungSvCertContentModel>();
            model.contentList.Add(new YoungSvCertContentModel()
            {
                content = "{姓名}",
                location_x = 258,
                location_y = 328,//注意，这个纵坐标的取值起点是图片的底部，也就是从下向上的距离，其余同理
                width = 200,
                fontSize = 14
            });
            model.contentList.Add(new YoungSvCertContentModel()
            {
                content = "{学校}",
                location_x = 258,
                location_y = 300,
                width = 350,
                fontSize = 14
            });
            model.contentList.Add(new YoungSvCertContentModel()
            {
                content = "{老师}",
                location_x = 258,
                location_y = 270,
                width = 200,
                fontSize = 14
            });

            model.contentList.Add(new Magic.Guangdong.Assistant.Lib.YoungSvCertContentModel()
            {
                content = "{编号}",
                location_x = 258,
                location_y = 236,
                width = 200,
                fontSize = 14
            });
            if (certModel.imgList != null && certModel.imgList.Length > 0)
            {
                model.imgList = new List<YoungSvCertImgModel>();
                foreach (var img in certModel.imgList)
                {
                    model.imgList.Add(new YoungSvCertImgModel()
                    {
                        imgUrl = img,
                        location_x = 356,
                        location_y = 75,
                        width = 116,
                        height = 116,
                        opacity = 0.82f
                    });
                }
            }
            return model;
        }
        /// <summary>
        /// 生成优秀展示将参数
        /// </summary>
        /// <param name="certModel"></param>
        /// <returns></returns>
        static YoungSvCertModel makeYoungSvCertYszsConfig(MatchAICertModel certModel)
        {
            var model = new YoungSvCertModel();
            model.certTempUrl = certModel.certTemplateUrlFinal;
            model.contentList = new List<YoungSvCertContentModel>();
            model.contentList.Add(new YoungSvCertContentModel()
            {
                content = "{姓名}",
                location_x = 258,
                location_y = 328,//注意，这个纵坐标的取值起点是图片的底部，也就是从下向上的距离，其余同理
                width = 200,
                fontSize = 14
            });
            model.contentList.Add(new YoungSvCertContentModel()
            {
                content = "{学校}",
                location_x = 258,
                location_y = 300,
                width = 300,
                fontSize = 14
            });
            model.contentList.Add(new YoungSvCertContentModel()
            {
                content = "{老师}",
                location_x = 258,
                location_y = 270,
                width = 200,
                fontSize = 14
            });
            model.contentList.Add(new Magic.Guangdong.Assistant.Lib.YoungSvCertContentModel()
            {
                content = "{编号}",
                location_x = 258,
                location_y = 236,
                width = 200,
                fontSize = 14
            });
            if (certModel.imgList != null && certModel.imgList.Length > 0)
            {
                model.imgList = new List<YoungSvCertImgModel>();
                foreach (var img in certModel.imgList)
                {
                    model.imgList.Add(new YoungSvCertImgModel()
                    {
                        imgUrl = img,
                        location_x = 356,
                        location_y = 75,
                        width = 116,
                        height = 116,
                        opacity = 0.82f
                    });
                }
            }
            return model;
        }
    }

    public class MatchAICertModel
    {
        /// <summary>
        /// 获奖人员
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 学校
        /// </summary>
        public string school { get; set; }
        /// <summary>
        /// 获奖教师
        /// </summary>
        public string teacher { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string number { get; set; }
        /// <summary>
        /// 赛项
        /// </summary>
        public string eventName { get; set; }
        /// <summary>
        /// 获奖类型
        /// 1对应 等级奖一等奖
        /// 2对应 等级奖二等奖
        /// 3对应 等级奖三等奖
        /// 4对应 展示奖
        /// 5对应 优秀展示奖
        /// </summary>
        public int certType { get; set; }

        /// <summary>
        /// 模板地址
        /// </summary>
        public string certTemplateUrl { get; set; }
        /// <summary>
        /// 模板地址（最终使用的地址）
        /// 给一个统一的网络路径，http开头，可以传如外部地址，也可以是内部地址，内部地址会自动转换成网络地址
        /// </summary>
        public string certTemplateUrlFinal
        {
            get
            {
                if (string.IsNullOrEmpty(certTemplateUrl))
                    return "";
                if (certTemplateUrl.StartsWith("http"))
                {
                    return certTemplateUrl;
                }
                return ConfigurationHelper.GetSectionValue("resourceHost")+certTemplateUrl.Replace("\\", "/");
            }
        }
        
        /// <summary>
        /// 要合成的图片列表（这个参数较为特殊，需要手动先设定好图片样式）
        /// </summary>
        public string[] imgList { get; set; }
               
    }
}
