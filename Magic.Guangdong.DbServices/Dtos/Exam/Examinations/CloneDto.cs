using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Exam.Examinations
{
    public class CloneDto
    {
        public Guid examId { get; set; }
        public string cloneExamName { get; set; } = "克隆";
        public string clonePaperTitle { get; set; } = "";
        public string adminId { get; set; }
    }
}
