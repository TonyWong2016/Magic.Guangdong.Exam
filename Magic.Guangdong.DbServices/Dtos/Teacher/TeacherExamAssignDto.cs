using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Teacher
{
    public class TeacherExamAssignDto
    {
        public Guid[] examIds { get; set; }

        public Guid[] teacherIds { get; set; }

    }

    public class TeacherExamAssignStringDto
    {
        public string examIds { get; set; }

        public string teacherIds { get; set; }

    }

    public class TeacherExamAssignSingleTeacherDto
    {
        public Guid[] examIds { get; set; }

        public Guid teacherId { get; set; }

    }

    public class TeacherExamAssignSingleExamDto
    {
        public Guid examId { get; set; }

        public Guid[] teacherIds { get; set; }

    }

    public class TeacherExamAssignSingleDto
    {
        public Guid examId { get; set; }

        public Guid teacherId { get; set; }

    }
}
