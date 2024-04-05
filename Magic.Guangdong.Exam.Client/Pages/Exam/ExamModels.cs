namespace Magic.Guangdong.Exam.Client.Pages.Exam
{
    public class ExamModels
    {
    }

    public class ExamListDto
    {
        public string associationId { get; set; } = "";

        public Guid examId { get; set; } = Guid.Empty;

        public string groupCode { get; set; } = "";
    }
}
