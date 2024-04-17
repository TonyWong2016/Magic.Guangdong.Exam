using Magic.Guangdong.Assistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Exam.Papers
{
    public class PaperQuestionDto
    {

        public long Id { get; set; }

        public string Title { get; set; }

        public string TitleRaw
        {
            get
            {
                //string filterTitle= Regex.Replace(Title, @"^\d+(\.|、|,|\w)", "");
                //@"^\d+(\.|、|,|\w)"加上^标识匹配开头，但富文本开头不一定是序号，可能有html字符
                return Regex.Replace(Title, @"^\d+(\.|、|,|\w)", "");
            }
            set { }
        }

        public string Degree { get; set; }

        public int IsOpen { get; set; }

        public string SubjectName { get; set; }

        public string TypeName { get; set; }

        public int? IsObjective { get; set; } = 1;

        public int? SingleAnswer { get; set; } = 1;

        public double ItemScore { get; set; }

        public string Analysis { get; set; }

        public string AnalysisTxt
        {
            get
            {
                if (!string.IsNullOrEmpty(Analysis))
                    return Utils.StripHTML(Analysis);
                return "无";
            }
        }

        public List<PaperQuestionItemDto> Items { get; set; }
    }

    public class PaperQuestionItemDto
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public int IsAnswer { get; set; }

        public string DescriptionTxt { get; set; }

        public int OrderIndex { get; set; }
    }

    

}
