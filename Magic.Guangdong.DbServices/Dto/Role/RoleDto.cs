using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Dto.Role
{
    public class RoleDto
    {
        public long Id { get; set; } = YitIdHelper.NextId();
        public string Name { get; set; }

        public string Description { get; set; }

        //public Guid AdminId { get; set; }

        public long[]? PermissionIds { get; set; }

        public RoleType Type { get; set; } 
    }

    public enum RoleType
    {
        Super = 1,
        Normal = 2,
        Other = 3
    }
}
