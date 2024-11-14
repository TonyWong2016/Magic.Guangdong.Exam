using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Material
{
    public class MaterialDto
    {
        public long Id { get; set; }

        public string ConnId { get; set; }  

        public string ConnName { get; set; }

        public string Remark { get; set; }

        public string link { get; set; } = "";
    }
}
