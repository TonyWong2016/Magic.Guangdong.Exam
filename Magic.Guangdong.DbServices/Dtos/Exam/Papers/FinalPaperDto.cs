using Magic.Guangdong.DbServices.Dtos.Exam.Papers;

namespace Magic.Guangdong.DbServices.Dtos.Exam.Papers
{
    public class FinalPaperDto
    {
        public Guid PaperId { get; set; }

        public Guid ExamId { get; set; }
        public string ExamTitle { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string PaperTitle { get; set; }

        public string AssociationId { get; set; }

        public string AssociationTitle { get; set; }

        public double PaperScore { get; set; }

        public double Duration { get; set; }

        public int PaperType { get; set; }

        public string PaperTypeStr
        {
            get
            {
                if (PaperType == 0) return "机器组卷";
                if (PaperType == 1) return "人工组卷";
                if (PaperType == 2) return "随机抽题";
                return "";
            }
        }

        public int Status { get; set; }


        public int OpenResult { get; set; }


        public List<PaperQuestionDto> Questions { get; set; }

        public DateTime CreatedAt { get; set; }

    }


}
