using Magic.Guangdong.Assistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Teacher
{

    public class SaveSubjectiveScoreDto
    {
        public Guid teacherId { get; set; }

        public long recordId { get; set; }

        public string itemScoreStr { get; set; }

        public List<SubjectiveScoreItem> itemScores
        {
            get
            {
                if (!string.IsNullOrEmpty(itemScoreStr))
                    return JsonHelper.JsonDeserialize<List<SubjectiveScoreItem>>(itemScoreStr);
                return null;
            }
            set { }
        }
    }


    public class SubjectiveScoreItem
    {
        public long SubmitRecordId { get; set; }
        public int Score { get; set; }

        public long QuestionId { get; set; }

        public string Remark { get; set; }
    }
}
