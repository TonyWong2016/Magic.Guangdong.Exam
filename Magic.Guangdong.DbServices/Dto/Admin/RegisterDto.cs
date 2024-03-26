using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dto.Admin
{
    public class RegisterDto
    {
        public string NickName { get; set; }
        public string? Name { get; set; }

        public string Email { get; set; } = "";

        public string Mobile { get; set; } = "";

        public string Password { get; set; }

        public string VerificationCode {  get; set; }
    }
}
