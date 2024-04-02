using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Magic.Guangdong.Assistant.Lib
{
    class ExportModelForYongSV
    {
    }
    /// <summary>
    /// 申报汇总导出模型
    /// 注意，字段属性的顺序很重要，设计导出的单元格合并
    /// </summary>
    public class ApplySummaryForExcelModel
    {
        /// <summary>
        /// 赛队编号
        /// </summary>
        [Description("赛队编号")]
        public string projectNo { get; set; }

        /// <summary>
        /// 类别领域
        /// </summary>
        [Description("类别领域")]
        public string fieldName { get; set; }

        /// <summary>
        /// 赛项
        /// </summary>
        [Description("赛项")]
        public string eventName { get; set; }

        

        /// <summary>
        /// 地区
        /// </summary>
        [Description("赛队地区")]
        public string province { get; set; }

        /// <summary>
        /// 组别
        /// </summary>
        [Description("组别")]
        public string team { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Description("年级")]
        public string gradeCaption { get; set; }

        /// <summary>
        /// 赛队
        /// </summary>
        [Description("赛队名称")]
        public string projectTitle { get; set; }

        /// <summary>
        /// 作品名
        /// </summary>
        [Description("作品名称")]
        public string projectWorkCaption { get; set; }

        /// <summary>
        /// 作品简介
        /// </summary>
        [Description("作品简介")]
        public string projectIntro { get; set; }
       
        /// <summary>
        /// 主题
        /// </summary>
        //[Description("主题")]
        public string topicName { get; set; }
        /// <summary>
        /// 选手姓名
        /// </summary>
        [Description("选手姓名")]
        public string memberName { get; set; }
        /// <summary>
        /// 选手地市
        /// </summary>
        [Description("选手地市")]
        public string memberAddr { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [Description("身份证号")]
        public string memberIdNo { get; set; }

        /// <summary>
        /// 学校
        /// </summary>
        [Description("学校")]
        public string memberSchool { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [Description("电话")]
        public string memberMobile { get; set; }
        /// <summary>
        /// email
        /// </summary>
        [Description("邮箱")]
        public string memberEmail { get; set; }
        /// <summary>
        /// 监护人姓名
        /// </summary>
        [Description("监护人姓名")]
        public string memberGuardianName { get; set; }
        /// <summary>
        /// 辅导教师姓名（逗号隔开）
        /// </summary>
        [Description("辅导教师姓名")]
        public string teacherNames { get; set; }
        /// <summary>
        /// 辅导教师身份证号（逗号隔开）
        /// </summary>
        [Description("辅导教师身份证号")]
        public string teacherIdNos { get; set; }
        /// <summary>
        /// 辅导教师单位（逗号隔开）
        /// </summary>
        [Description("辅导教师单位")]
        public string teacherUnits { get; set; }
        /// <summary>
        /// 辅导教师电话（逗号隔开）
        /// </summary>
        [Description("辅导教师电话")]
        public string teacherMobiles { get; set; }
        /// <summary>
        /// 辅导教师email（逗号隔开）
        /// </summary>
        [Description("辅导教师邮箱")]        
        public string teacherEmails { get; set; }
        
        /// <summary>
        /// 申报id
        /// </summary>
        public int ApplyID { get; set; }

        /// <summary>
        /// 关系
        /// </summary>
        //[Description("关系")]
        //public string memberGuardianRelation { get; set; }
    }
    /// <summary>
    /// 汇总导出pdf模型
    /// </summary>
    public class ApplySummaryForPdfModel
    {
        /// <summary>
        /// 赛项
        /// </summary>
        // [Description("赛项")]
        public string eventName { get; set; }

        public string fieldName { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        [Description("地区")]
        public string province { get; set; }

        /// <summary>
        /// 组别
        /// </summary>
        [Description("组别")]
        public string team { get; set; }

        /// <summary>
        /// 赛队
        /// </summary>
        [Description("赛队名称")]
        public string projectTitle { get; set; }
        /// <summary>
        /// 辅导教师姓名（逗号隔开）
        /// </summary>
        [Description("教师")]
        public string teacherNames { get; set; }
        /// <summary>
        /// 主题（以此分类作为单元格合并依据）
        /// </summary>
        //[Description("主题")]
        public string topicName { get; set; }
        /// <summary>
        /// 选手姓名
        /// </summary>
        [Description("姓名")]
        public string memberName { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        [Description("电话")]
        public string memberMobile { get; set; }
        /// <summary>
        /// 学校
        /// </summary>
        [Description("学校")]
        public string memberSchool { get; set; }

        /// <summary>
        /// 申报id
        /// </summary>
        public int ApplyID { get; set; }

    }

    /// <summary>
    /// 汇总导出pdf模型
    /// </summary>
    public class ApplySummaryForPdfSpaceModel
    {
        /// <summary>
        /// 赛队编号
        /// </summary>
        [Description("赛队编号")]
        public string projectNo { get; set; }                

        /// <summary>
        /// 组别
        /// </summary>
        [Description("参赛组别")]
        public string team { get; set; }


        [Description("年级")]
        public string gradeCaption { get; set; }

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
        /// 选手姓名
        /// </summary>
        [Description("选手姓名")]
        public string memberName { get; set; }

        /// <summary>
        /// 学校
        /// </summary>
        [Description("学校")]
        public string memberSchool { get; set; }

        /// <summary>
        /// 辅导教师姓名（逗号隔开）
        /// </summary>
        [Description("辅导教师")]
        public string teacherNames { get; set; }
        
        /// <summary>
        /// 电话
        /// </summary>
        [Description("联系电话")]
        public string memberMobile { get; set; }
        

        
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
    /// <summary>
    /// 确认参赛导出Pdf模型（回执表）
    /// </summary>
    public class ApplyFinalBackForPdfModel
    {
        [Description("赛队编号")]
        public string ProjectNo { get; set; }
        [Description("参赛赛队")]
        public string ProjectTitle { get; set; }
        [Description("参赛选手")]
        public string MemberNames { get; set; }
        [Description("辅导教师")]
        public string TeacherNames { get; set; }

        public int ApplyID { get; set; }

        public int ProgramTypeID { get; set; }

    }
    public class ApplyMemberInfo
    {
        /// <summary>
        /// 选手姓名
        /// </summary>
        [Description("选手姓名")]
        public string name { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [Description("身份证号")]
        public string idNo { get; set; }

        /// <summary>
        /// 学校
        /// </summary>
        [Description("学校")]
        public string school { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [Description("电话")]
        public string mobile { get; set; }
        /// <summary>
        /// email
        /// </summary>
        [Description("邮箱")]
        public string email { get; set; }

        /// <summary>
        /// 监护人姓名
        /// </summary>
        [Description("监护人姓名")]
        public string guardianName { get; set; }
        /// <summary>
        /// 关系
        /// </summary>
        [Description("关系")]
        public string relation { get; set; }

    }

    public class ApplyTeacherInfo { }
}
