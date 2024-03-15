using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dto
{
    public class PageDto
    {
        public string whereJsonStr { get; set; }

        public int pageindex { get; set; } = 1;

        public int pagesize { get; set; } = 10;
    }
}
