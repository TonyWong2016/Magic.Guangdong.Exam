using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Teacher
{
    public class TeacherSummaryDto
    {
        public long ExamCnt { get; set; } = 0;
        
        public long MarkedCnt { get; set; } = 0;

        public long UnMarkedCnt { get; set; }

        //public List<TeacherPapersCntDto> PapersCntList { get; set; }
    }

    public class TeacherPapersCntDto
    {
        public long PapersCnt { get; set; } = 0;

        public Guid ExamId { get; set; }

        public string ExamTitle { get; set; }

        //public int Marked { get; set; }

        public long value
        {
            get { return PapersCnt;  }
        }

        public string name
        {
            get
            {
                return ExamTitle;
            }
            }

        //public long MarkedCnt { get; set; }

        //public long UnMarkedCnt
        //{
        //    get
        //    {
        //        return PapersCnt - MarkedCnt;
        //    }
        //}
    }

    public class TeacherPapersMarkedCntLast7DaysDto
    {
        //public Guid examId { get; set;}

        //public string examTitle { get; set;}

        public DateTime MarkedDate { get; set; }

        public string MarkedDateFormat {
            get
            {
                return MarkedDate.ToString("yyyy/MM/dd");
            }
        }

        public int MarkedCnt { get; set; }
    }

}
