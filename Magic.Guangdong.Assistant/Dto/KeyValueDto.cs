using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.Dto
{
    public record KeyValueDto
    {
        public string key { get; set; }

        public string value { get; set; }
    }

    public class KeyContentDto
    {
        public string key { get; set; }

        public string content { get; set; }
    }
}
