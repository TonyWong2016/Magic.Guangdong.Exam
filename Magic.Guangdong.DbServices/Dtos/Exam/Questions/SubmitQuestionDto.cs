using FreeSql.Internal;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Exam.Questions
{
    public class SubmitQuestionDto
    {
        public string questionStr { get; set; }

        public string itemsStr { get; set; }

        public string Analysis { get; set;}

        public Question question
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(questionStr))
                {
                    return Assistant.JsonHelper.JsonDeserialize<Question>(questionStr);
                }
                return null;
            }
        }
        public List<QuestionItem> items
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(itemsStr))
                {
                    return Assistant.JsonHelper.JsonDeserialize<List<QuestionItem>>(itemsStr);
                }
                return null;
            }
        }
    }
}
