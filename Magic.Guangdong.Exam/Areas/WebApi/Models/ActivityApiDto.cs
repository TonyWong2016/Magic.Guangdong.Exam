using Yitter.IdGenerator;

namespace Magic.Guangdong.Exam.Areas.WebApi.Models
{
    public class ActivityApiDto
    {
        public long Id { get; set; } = YitIdHelper.NextId();

        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Status { get; set; } = 0;
        public string FieldJson { get; set; } = "{}";

        public int CreatedExam { get; set; } = 0;

        public ExamApiDto ExamDto { get; set; }
    }
}
