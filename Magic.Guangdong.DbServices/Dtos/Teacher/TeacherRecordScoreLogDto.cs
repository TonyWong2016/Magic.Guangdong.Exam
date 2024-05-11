using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Teacher
{
    public class TeacherRecordScoreLogDto
    {
        public long Id { get; set; }

        public Guid TeacherId { get; set; }

        public string TeacherName { get; set;}

        public double SubjectiveScore { get; set; }

        public DateTime ScoreTime { get; set; }
    }
}
