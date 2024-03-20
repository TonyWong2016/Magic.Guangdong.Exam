using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Dto.Permissions
{
    public class PermissionDto
    {
        public long Id { get; set; } = YitIdHelper.NextId();
        public string Controller { get; set; }

        public string Action { get; set; }

        public string Area { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Router { get; set; }
    }
}
