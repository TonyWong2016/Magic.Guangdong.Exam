using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Teacher
{
    public class TeacherSubjectiveMarkDto
    {
        public ExamInfoDto examInfoDto;

        public List<SubjectiveQuestionAndAnswersDto> subjectiveQuestionAndAnswers;
    }
     
    public class SubjectiveQuestionAndAnswersDto
    {
        
        public QuestionView question { get; set; }

        //public List<QuestionItem> questionItems { get; set; }

        public UserAnswerSubmitRecord? userAnswer { get; set; }

    }

    public class ExamInfoDto
    {
        public string IdNumber { get; set; }

        public string PaperTitle { get; set; }

        public string ExamTitle { get; set; }
    }
}
