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

    public class AdminPermissionDto
    {
        public long RoleId { get; set; }


        public Guid AdminId { get; set; }

        public long PermissionId { get; set; }

        public string area { get; set; } = "";

        public string controller { get; set; } = "";  

        public string action { get; set; } = "";

        public string router
        {
            get
            {
                if(string.IsNullOrEmpty(area))
                    return $"/{controller}/{action}";
                return $"/{area}/{controller}/{action}";
            }
        }
        public string dataFilterJson { get; set; } = "";
    }

}
