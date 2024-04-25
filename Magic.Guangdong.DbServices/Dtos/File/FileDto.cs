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
    }
}
