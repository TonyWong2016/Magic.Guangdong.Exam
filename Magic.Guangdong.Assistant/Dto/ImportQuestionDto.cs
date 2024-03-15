using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Magic.Guangdong.Assistant.Dto
{
    [ExcelImporter(IsLabelingError = true)]
    public class ImportQuestionDto
    {
        [ImporterHeader(Name = "科目", Format = "@", Description = "只能填入系统中包含的科目", AutoTrim = true, FixAllSpace = true)]
        [Required(ErrorMessage = "科目不能为空")]
        public string QuestionSubject { get; set; }

        //没有做成筛选，现阶段就靠自觉吧
        [ImporterHeader(Name = "题型", Description = "只能填入单选题，判断题，多选题，填空题，简答题", AutoTrim = true, FixAllSpace = true)]
        [Required(ErrorMessage = "题型不能为空")]
        public string QuestionType { get; set; }

        [ImporterHeader(Name = "难度", Description = "只能填入容易，普通，困难三种类型", AutoTrim = true, FixAllSpace = true)]
        [Required(ErrorMessage = "题型不能为空")]
        [ValueMapping("容易", "easy")]
        [ValueMapping("普通", "normal")]
        [ValueMapping("困难", "difficult")]
        [ValueMapping("低", "easy")]
        [ValueMapping("中", "normal")]
        [ValueMapping("高", "difficult")]
        public string Degree { get; set; } = "normal";


        [ImporterHeader(Name = "题目", Format = "@", Description = "题目内容", AutoTrim = true, IsAllowRepeat = false, FixAllSpace = true)]
        [Required(ErrorMessage = "题目不能为空")]
        public string Title { get; set; }

        [ImporterHeader(Name = "选项内容1", Format = "@", Description = "选项内容，由字目A-Z开头，并包含具体内容，若是判断题，则只能再选项1和2种出现‘对’，‘错’，‘正确’，‘错误’等字眼")]
        [MaxLength(500, ErrorMessage = "字数超出最大限制,请修改!")]
        public string Option1 { get; set; }

        [ImporterHeader(Name = "选项内容2", Format = "@", Description = "选项内容，由字目A-Z开头，并包含具体内容，若是判断题，则只能再选项1和2种出现‘对’，‘错’，‘正确’，‘错误’等字眼")]
        [MaxLength(500, ErrorMessage = "字数超出最大限制,请修改!")]
        public string Option2 { get; set; }

        [ImporterHeader(Name = "选项内容3", Format = "@", Description = "选项内容，由字目A-Z开头，并包含具体内容")]
        [MaxLength(500, ErrorMessage = "字数超出最大限制,请修改!")]
        public string Option3 { get; set; }

        [ImporterHeader(Name = "选项内容4", Format = "@", Description = "选项内容，由字目A-Z开头，并包含具体内容")]
        [MaxLength(500, ErrorMessage = "字数超出最大限制,请修改!")]
        public string Option4 { get; set; }

        [ImporterHeader(Name = "选项内容5", Format = "@", Description = "选项内容，由字目A-Z开头，并包含具体内容")]
        [MaxLength(500, ErrorMessage = "字数超出最大限制,请修改!")]
        public string Option5 { get; set; }

        [ImporterHeader(Name = "选项内容6", Format = "@", Description = "选项内容，由字目A-Z开头，并包含具体内容")]
        [MaxLength(500, ErrorMessage = "字数超出最大限制,请修改!")]
        public string Option6 { get; set; }

        [ImporterHeader(Name = "答案", Format = "@", Description = "答案内容推荐使用选项标识1-6，直接输入选项数字即可，或者A-Z，多个答案请用管道符'|'分开")]
        [MaxLength(500, ErrorMessage = "字数超出最大限制,请修改!")]
        [Required(ErrorMessage = "答案不能为空")]
        public string Answer { get; set; }

        [ImporterHeader(Name = "解析", Format = "@", Description = "解析内容")]
        [MaxLength(500, ErrorMessage = "字数超出最大限制,请修改!")]
        //[Required(ErrorMessage = "解析不能为空")]
        public string Analysis { get; set; }

        [ImporterHeader(Name = "分值", Description = "题目分值")]
        [Required(ErrorMessage = "分值不能为空")]
        public double Score { get; set; }

        [ImporterHeader(Name = "备注", Format = "@", Description = "备注")]
        public string Remark { get; set; }


        [ImporterHeader(IsIgnore = true)]
        public Guid SubjectId { get; set; }

        [ImporterHeader(IsIgnore = true)]
        public Guid QuestionTypeId { get; set; }

        [ImporterHeader(IsIgnore = true)]
        public string CreateBy { get; set; }

        [ImporterHeader(IsIgnore = true)]
        public string ColumnId { get; set; }
    }
}
