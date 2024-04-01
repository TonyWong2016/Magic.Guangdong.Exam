using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.System.Admins
{
    public class AdminDto
    {
        public Guid Id { get; set; } = NewId.NextGuid();
        public string NickName { get; set; } = "";
        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public string Mobile { get; set; } = "";

        public string Password { get; set; } = "";

        public int Status { get; set; } = 1;

        public string Description { get; set; } = "管理员";

        public string AccountAvailable { get; set; } = "username";

        public long[] RoleIds
        {
            get; set;

        }

        public string? RoleIdStr
        {
            get
            {
                if (RoleIds != null && RoleIds.Any())
                {
                    return string.Join(",", RoleIds);
                }
                return "";
            }
        }
    }

    public class AfterLoginDto
    {
        public Guid adminId { get; set; }

        public DateTime exp { get; set; }
    }
}
