using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dto.Menus
{
    public class MenuDto
    {
        public long  Id { get; set; }

        public string Name { get; set; }

        public int Depth { get; set; }

        public long ParentId { get; set; }

        public string Description { get; set; }

        public long PermissionId { get; set; }

        public string Router { get; set; }

        public int IsLeef { get; set; }
    }
}
