using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Exam.Examinations
{
    public class ScoreSchemeDto
    {
        /// <summary>
         /// 答对后评分动作，正数代表得分基础上正向相乘，比如这里给1，题目分数是2分，那答对后就得2分（1*2），给2就是得4分（2*2），以此类推
         /// </summary>
        public double CorrectAction { get; set; } = 1d;

        public string Description { get; set; }

        /// <summary>
        /// 不答得评分动作，正数代表得分基础上正向相乘，比如这里给0，题目分数是2分，那不答题后就得0分（0*2），给-1就是扣2分（-1*2），以此类推
        /// </summary>
        public double EmptyAction { get; set; } = 0d;

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; } = "system";

        public string UpdatedBy { get; set; } = "system";

        /// <summary>
        /// 答错后评分动作，正数代表得分基础上正向相乘，比如这里给0，题目分数是2分，那答错后就得0分（0*2），给-1就是扣2分（-1*2），以此类推
        /// </summary>
        [JsonProperty]
        public double WrongAction { get; set; } = 0d;
    }
}
