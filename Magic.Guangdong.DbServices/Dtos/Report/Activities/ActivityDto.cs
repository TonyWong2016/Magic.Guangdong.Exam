using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Dtos.Report.Activities
{
    public class ActivityDto
    {
        public long Id { get; set; } = YitIdHelper.NextId();

        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Status { get; set; } = 0;


        //public int Quota { get; set; } = 0;

        //public decimal Expenses { get; set; } = 0;

        public string FieldJson { get; set; } = "{}";
    }

    public class ActivityReportDto()
    {
        public long ActivityId { get; set; }

        public string Title { get; set; }

        public int status { get; set; }

        public List<ActivityExamDto> Exams { get; set; }
    }

    public class ActivityExamDto()
    {
        public Guid ExamId { get; set; }

        public string ExamTitle { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        
        public int Quota { get; set; }

        public decimal Expenses { get; set; }

        public string Amount {
            get
            {
                if (Expenses > 0)
                    return Math.Round(Expenses, 2).ToString();
                return "0";
            }
        }
    }
}
