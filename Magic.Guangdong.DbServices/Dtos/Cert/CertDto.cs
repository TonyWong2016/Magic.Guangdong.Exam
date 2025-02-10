using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Cert
{
    public class CertDto
    {
        [Description("证书编号")]
        public string CertNo { get; set; }

        [Description("获奖人员")]
        public string AwardName { get; set; }

        [Description("识别码")]
        public string IdNumber { get; set; }

        [Description("证书标题")]
        public string Title { get; set; }

        [Description("生成时间")]
        public string CreatedAt { get; set; }



        public string CertContent { get; set; }

        [Description("自定义内容1（请根据实际内容修改列头）")]
        public string CustomField1 
        {
            get
            {
                if(CertContents.Any(u=>u.key== "customField1"))
                {
                    return CertContents.First(u => u.key == "customField1").content;
                }
                return "未导入任何数据，请删除该列";
            }
            
        } 

        [Description("自定义内容2（请根据实际内容修改列头）")]
        public string CustomField2
        {
            get
            {
                if (CertContents.Any(u => u.key == "customField2"))
                {
                    return CertContents.First(u => u.key == "customField2").content;
                }
                return "未导入任何数据，请删除该列";
            }

        }


        [Description("自定义内容3（请根据实际内容修改列头）")]
        public string CustomField3
        {
            get
            {
                if (CertContents.Any(u => u.key == "customField3"))
                {
                    return CertContents.First(u => u.key == "customField3").content;
                }
                return "未导入任何数据，请删除该列";
            }

        }


        [Description("自定义内容4（请根据实际内容修改列头）")]
        public string CustomField4
        {
            get
            {
                if (CertContents.Any(u => u.key == "customField4"))
                {
                    return CertContents.First(u => u.key == "customField4").content;
                }
                return "未导入任何数据，请删除该列";
            }

        }

        [Description("自定义内容5（请根据实际内容修改列头）")]
        public string CustomField5
        {
            get
            {
                if (CertContents.Any(u => u.key == "customField5"))
                {
                    return CertContents.First(u => u.key == "customField5").content;
                }
                return "未导入任何数据，请删除该列";
            }

        }

        [Description("自定义内容6（请根据实际内容修改列头）")]
        public string CustomField6
        {
            get
            {
                if (CertContents.Any(u => u.key == "customField6"))
                {
                    return CertContents.First(u => u.key == "customField6").content;
                }
                return "未导入任何数据，请删除该列";
            }

        }

        [Description("自定义内容7（请根据实际内容修改列头）")]
        public string CustomField7
        {
            get
            {
                if (CertContents.Any(u => u.key == "customField7"))
                {
                    return CertContents.First(u => u.key == "customField7").content;
                }
                return "未导入任何数据，请删除该列";
            }

        }

        [Description("自定义内容8（请根据实际内容修改列头）")]
        public string CustomField8
        {
            get
            {
                if (CertContents.Any(u => u.key == "customField8"))
                {
                    return CertContents.First(u => u.key == "customField8").content;
                }
                return "未导入任何数据，请删除该列";
            }

        }
        public string Url { get; set; }
        [Description("查看链接")]
        public string DownloadUrl 
        {
            get 
            { 
                if(!string.IsNullOrEmpty(Url) && !Url.StartsWith("http"))
                {
                    return ConfigurationHelper.GetSectionValue("resourceHost") + Url;
                }
                return Url;
            }
        }

        public List<KeyContentDto> CertContents
        {
            get
            {
                if (!string.IsNullOrEmpty(CertContent))
                {
                    return JsonHelper.JsonDeserialize<List<KeyContentDto>>(CertContent);
                }
                return null;
            }
        }

    }

    public class CertRequestDto
    {
        public string CertNo { get; set; }

        public string CertTitle { get; set; }

        public string AwardName { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int IsDesc { get; set;} = 0;
    }


    public class CertDtoForApi
    {
        public long total { get; set; }

        public List<CertDto> items { get; set; }

    }
}
