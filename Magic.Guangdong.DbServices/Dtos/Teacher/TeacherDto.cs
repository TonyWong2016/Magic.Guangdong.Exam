using MassTransit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Dtos.Teacher
{
    public class TeacherDto
    {

        public Guid Id { get; set; } = NewId.NextGuid();

        [Required(AllowEmptyStrings = false, ErrorMessage = "姓名不可为空")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "最大不可以超过500个字")]
        public string Intro { get; set; }

        //public string TeachNo {  get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "电话不可为空")]
        public string Mobile { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "邮箱不可为空")]
        public string Email { get; set; }


        public string AuthToken { get; set; }
        //public string KeyId { get; set; }
        //public string KeySecret { get; set; }

    }

    public class TeacherPwdChangedDto
    {
        public Guid Id { get; set; }

        public string AuthToken { get; set; }

        public int Notice { get; set; } = 0;
    }
}
