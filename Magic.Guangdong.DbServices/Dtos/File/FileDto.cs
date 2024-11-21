using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Dtos.File
{
    public class FileDto
    {
        public long Id { get; set; } = YitIdHelper.NextId();

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Name { get; set; } = Assistant.Utils.GenerateRandomCodePro(10);

        public string Ext { get; set; } = "";

        public long Size { get; set; } = 0;

        public string Type { get; set; } = "";

        public string AccountId { get; set; } = "";

        public string Path { get; set; } = "";

        public string ShortUrl { get; set; } = "";

        public int IsDeleted { get; set; } = 0;

        public string ConnId { get; set; } = "";

        public string ConnName { get; set; } = "";

        public string Remark { get; set; } = "";

        public string FileUrl
        {
            get
            {
                if (string.IsNullOrEmpty(ShortUrl))
                    return "";
                if (ShortUrl.StartsWith("http"))
                    return ShortUrl;
                if (Type == "server")
                {
                    return Assistant.ConfigurationHelper.GetSectionValue("resourceHost") + ShortUrl;
                }
                return Assistant.ConfigurationHelper.GetSectionValue("baseHost") + ShortUrl;
            }
        }

        public string FileUrlTitle
        {
            get
            {
                if (string.IsNullOrEmpty(Remark))
                    return Name;
                return Remark;
            }
        }

        public string FileSize
        {
            get
            {
                if(Size==0 && ShortUrl.StartsWith("http"))
                {
                    return "网络资源，未获取实际大小";
                }
                double kbSize = Convert.ToDouble(Size * 1.0 / 1024);
                if (kbSize <= 1024)
                    return Math.Round(kbSize, 2)+"KB";
                double mbSize = kbSize / 1024;
                if (mbSize <= 1024)
                    return Math.Round(mbSize, 2)+"MB";
                return Size + "字节(B)";
            }
        }
    }
}
