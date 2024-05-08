using Magic.Guangdong.DbServices.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Exam.UserAnswerRecord
{
    public class UserAnswerSubmitRecordDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string UserName
        {
            get
            {
                return Name;
            }
            set { }
        }
        public string IdNumber { get; set; }
        public string ReportId { get; set; }

        public string PaperTitle {  get; set; }

        public string ExamTitle { get; set; }

        public string AssociationTitle { get; set; }

        public double Score { get; set; }

        public double ObjectiveScore { get; set; }

        public Guid PaperId { get; set; }

        public double UsedTime { get; set; }

        public string PaperDegree {  get; set; }

        public int ExamType {  get; set; }

        public int IsDeleted { get; set; }

        public int Marked { get; set; }

        public string MarkedStr
        {
            get
            {
                if (Marked == (int)ExamMarked.All)
                {
                    return "主观题判分完成";
                }
                if (Marked == (int)ExamMarked.Part)
                {
                    return "客观题已判分，主观题未判分";
                }
                return "未判分";
            }
        }
    }


    public class TeacherPapersDto
    {
        public long Id { get; set; }

        //public string Name { get; set; }

        //public string UserName
        //{
        //    get
        //    {
        //        return Name;
        //    }
        //    set { }
        //}

        //public string IdNumber { get; set; }

        public string ReportId { get; set; }

        public string PaperTitle { get; set; }

        public string ExamTitle { get; set; }

        public string AssociationTitle { get; set; }

        public double Score { get; set; }

        public double ObjectiveScore { get; set; }

        public Guid PaperId { get; set; }

        //public double UsedTime { get; set; }

        public string PaperDegree { get; set; }

        public int ExamType { get; set; }

        public int Marked { get; set; }

        public string MarkedStr
        {
            get
            {
                if(Marked == (int)ExamMarked.All)
                {
                    return "主观题判分完成";
                }
                if(Marked == (int)ExamMarked.Part)
                {
                    return "客观题已判分，主观题未判分";
                }
                return "未判分";
            }
        }

        //public int IsDeleted { get; set; }
    }
}
