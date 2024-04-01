using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.System.Admins
{
    public class RegisterDto
    {
        public string NickName { get; set; }
        public string? Name { get; set; }

        public string Email { get; set; } = "";

        public string Mobile { get; set; } = "";

        public string Password { get; set; }

        public string Description { get; set; } = "管理员";

        public string VerificationCode { get; set; }
    }


}
