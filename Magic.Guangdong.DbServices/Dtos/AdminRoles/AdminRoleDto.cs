using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.AdminRoles
{
    public class AdminRoleDto
    {
        public Guid AdminID { get; set; }

        public long[] RoleId { get; set; }

    }
}
