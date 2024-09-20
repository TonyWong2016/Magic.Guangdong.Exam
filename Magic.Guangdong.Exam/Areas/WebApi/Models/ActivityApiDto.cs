using Yitter.IdGenerator;

namespace Magic.Guangdong.Exam.Areas.WebApi.Models
{
    public class ActivityApiDto
    {
        public long Id { get; set; } = YitIdHelper.NextId();

        public string Title { get; set; }

        public string? Description { get; set; }

        public long St { get; set; }

        public long Et { get; set; }

        public DateTime StartTime
        {
            get
            {
                return Assistant.Utils.TimeStampToDateTime(St);
            }
        }

        public DateTime EndTime
        {
            get
            {
                return Assistant.Utils.TimeStampToDateTime(Et);
            }
        }

        public int Status { get; set; } = 0;
        public string FieldJson { get; set; } = "{}";

        public int CreatedExam { get; set; } = 0;

        public ExamApiDto ExamDto { get; set; }
    }
}
