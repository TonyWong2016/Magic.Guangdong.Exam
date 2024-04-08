using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Exam.Examinations
{
    public class ExamValidateExpressionDto
    {
        public Guid ExamId { get; set; }

        public Guid[] PaperIds { get; set; }

        public long ActivityId { get; set; }

        public string Expression { get; set; }

        public string AdminId { get; set; } = "";
    }
}
