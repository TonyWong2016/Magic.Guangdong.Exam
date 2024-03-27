using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Admins
{
    public class AdminDto
    {
        public Guid Id { get; set; } = NewId.NextGuid();
        public string NickName { get; set; }
        public string? Name { get; set; }

        public string Email { get; set; } = "";

        public string Mobile { get; set; } = "";

        public string Password { get; set; }

        public int Status { get; set; } = 1;

        public string Description { get; set; } = "管理员";
    }
}
