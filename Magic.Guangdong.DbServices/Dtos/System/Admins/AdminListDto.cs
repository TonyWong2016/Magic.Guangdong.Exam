using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.System.Admins
{
    public class AdminListDto
    {
        public Guid Id { get; set; }
        public string NickName { get; set; }
        public string? Name { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public int Status { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<AdminRoleList> adminRoles { get; set; }
    }

    public class AdminRoleList
    {
        public Guid AdminId { get; set; }
        public long RoleId { get; set; }

        /// <summary>
        /// 所有角色的组合，拼接起来
        /// </summary>
        public string RoleName { get; set; }

        public int RoleType { get; set; }
    }
}
