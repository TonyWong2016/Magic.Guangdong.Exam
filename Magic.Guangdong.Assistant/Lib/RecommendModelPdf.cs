using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Magic.Guangdong.Assistant.Lib
{
    public class RecommendModelPdf
    {
        

        //public string fieldName { get; set; }
        /// <summary>
        /// 项目、赛队编号
        /// </summary>
        [Description("赛队编号")]
        public string projectNo { get; set; }

        /// <summary>
        /// 赛队
        /// </summary>
        [Description("赛队/作品名称")]
        public string projectTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(MatchTeamName))
                    return MatchTeamName;
                if (!string.IsNullOrEmpty(ProjectWorkCaption))
                    return ProjectWorkCaption;
                return "无";
            }
            set { }
        }

        /// <summary>
        /// 组别
        /// </summary>
        [Description("参赛组别")]
        public string team { get; set; }

        /// <summary>
        /// 选手姓名
        /// </summary>
        [Description("选手姓名")]
        public string memberName { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Description("教师")]
        public string teacherName { get; set; }


        /// <summary>
        /// 地区
        /// </summary>
        //[Description("地区")]
        public string province { get; set; }

        //[Description("赛区")]
        public string areaName { get; set; }

        /// <summary>
        /// 赛项
        /// </summary>
        [Description("赛项")]
        public string eventName { get; set; }

        /// <summary>
        /// 学校
        /// </summary>
        [Description("学校")]
        public string memberSchool { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        //[Description("分数")]
        public string score { get; set; }

        //[Description("测评分数")]
        //public string OnlineScore { get; set; }

        [Description("选拔赛成绩")]
        public double OnlineScoreFirst { get; set; }

        //[Description("决赛在线测评成绩")]
        public double OnlineScoreSecond { get; set; }

        //[Description("在线测评阶段")]
        public double OnlineScoreStage { get; set; }

        /// <summary>
        /// 赛项支持单位
        /// </summary>
        [DisplayName("赛项支持单位")]
        public string UnitCaption { get; set; }


        public int IsRecommended { get; set; }

        /// <summary>
        /// 申报id
        /// </summary>
        public int ApplyID { get; set; }

        /// <summary>
        /// 赛队名
        /// </summary>
        public string MatchTeamName { get; set; }

        /// <summary>
        /// 作品名
        /// </summary>
        public string ProjectWorkCaption { get; set; }
    }
}
