using Essensoft.Paylink.Alipay.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Material
{
    public class MaterialDto
    {
        public long Id { get; set; } = 0;

        public string ConnId { get; set; }  

        public string ConnName { get; set; }

        public string Remark { get; set; }

        public string Link { get; set; } = "";
    }

    public class MaterialResponseDto
    {
        public long Id { get; set; }

        public string ConnId { get; set; }

        public string ConnName { get; set; }

        public string Remark { get; set; }

        public string ShortUrl { get; set; }

        public string Md5 { get;set;}

        public string Name {  get; set; }

        public string Ext { get; set; }

        public long Size { get; set; }


        public string imgUrl { 
            get {
                if (string.IsNullOrEmpty(ShortUrl))
                    return "";
                if(ShortUrl.StartsWith("http"))
                    return ShortUrl;
                return Assistant.ConfigurationHelper.GetSectionValue("baseHost")+ShortUrl;
            } 
        }

        public string imgTitle
        {
            get
            {
                if (string.IsNullOrEmpty(Remark))
                    return Name;
                return Remark;
            }
        }
    }

    
}
