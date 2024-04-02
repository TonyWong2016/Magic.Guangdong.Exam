using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using NPOI.XWPF.UserModel;
using System.Text.RegularExpressions;

namespace Magic.Guangdong.Exam.Areas.Exam.Utils
{
    public class WordUtils
    {
        private readonly ISubjectRepo _subjectRepo;
        private readonly IQuestionTypeRepo _questionTypeRepo;
        public WordUtils(IQuestionTypeRepo questionTypeRepo, ISubjectRepo subjectRepo)
        {
            _questionTypeRepo = questionTypeRepo;
            _subjectRepo = subjectRepo;
        }

        /// <summary>
        /// 解析导入的word文档
        /// 暂时比较傻瓜，主要是正则表达式的高阶用法我都忘了。。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public async Task<dynamic> ParseWord(string path, Guid? subjectId, string columnId = "0")
        {
            FileStream fis = new FileStream(path, FileMode.Open, FileAccess.Read);
            XWPFDocument doc = new XWPFDocument(fis);

            if (doc.Paragraphs.Any(u => u.NumLevelText != null) || doc.AllPictures.Any() || doc.Tables.Any())
            {
                Console.WriteLine("暂时不支持导入word里的序列,图片和表格");
                return false;
            }
            var types = await _questionTypeRepo.getListAsync(u => u.IsDeleted == 0);
            var subjects = await _subjectRepo.getListAsync(u => u.IsDeleted == 0);
            //var typeGroups = types.Select(u => u.Caption);
            List<ImportQuestionFromWord> questionlist = new List<ImportQuestionFromWord>();
            List<ImportQuestionItem> optionList = new List<ImportQuestionItem>();

            var questionModel = new ImportQuestionFromWord();
            QuestionType type = new QuestionType();
            int index = 0;
            //bool nextFlag = false;
            foreach (var para in doc.Paragraphs)
            {

                if (types.Any(u => u.Caption == para.Text))
                {
                    type = types.Find(u => u.Caption == para.Text);
                    continue;
                }
                questionModel.typeId = type.Id;
                questionModel.columnId = columnId;
                //匹配标题
                Regex titleRegex = new Regex(@"^\d、|^\d\.");
                if (titleRegex.IsMatch(para.Text))
                {
                    questionModel.title = para.Text;
                    Console.WriteLine("标题：" + para.Text);
                    continue;
                }

                //匹配选项
                Regex optionRegex = new Regex(@"^[A-Za-z]、|^[A-Za-z]\.");
                if (optionRegex.IsMatch(para.Text))
                {
                    optionList.Add(new ImportQuestionItem()
                    {
                        code = para.Text.Substring(0, 1),
                        description = para.Text.Substring(2),
                        index = index,
                    });
                    Console.WriteLine("选项：" + para.Text);
                    continue;
                }

                //匹配答案
                Regex answerRegex = new Regex(@"^(答案：)");
                if (answerRegex.IsMatch(para.Text))
                {
                    string[] parts = para.Text.Replace("答案：", "").Split("|");
                    if (type.Objective == 0)
                    {
                        foreach (string part in parts)
                        {
                            optionList.Add(new ImportQuestionItem()
                            {
                                description = part,
                                isAnswer = 1,
                                code = "主观题",

                            });
                        }
                    }
                    else
                    {
                        optionList.ForEach(u =>
                        {
                            if (parts.Contains(u.code))
                            {
                                u.isAnswer = 1;
                            }
                        });
                    }
                    Console.WriteLine("答案：" + para.Text);
                    continue;
                }

                //匹配分值
                Regex scoreRegex = new Regex(@"^(分值：)");
                if (scoreRegex.IsMatch(para.Text))
                {
                    string s = para.Text.Replace("分值：", "");
                    questionModel.score = Convert.ToDouble(s);
                    Console.WriteLine("分值：" + para.Text);
                    continue;
                }

                //匹配难度
                Regex degreeRegex = new Regex(@"^(难度：)");
                if (titleRegex.IsMatch(para.Text))
                {
                    questionModel.degree = para.Text;
                    Console.WriteLine("难度：" + para.Text);
                    continue;
                }

                //匹配科目（如果导入中包含科目，则取科目，如果不包含则取传入值）
                Regex subjectRegex = new Regex(@"^(科目：)");
                if (scoreRegex.IsMatch(para.Text))
                {
                    string subject = para.Text.Replace("科目:", "");
                    if (subjects.Any(u => u.Caption == subject))
                    {
                        questionModel.subjectId = subjects.Find(u => u.Caption == subject).Id;
                    }
                    Console.WriteLine("科目：" + para.Text);
                    continue;
                }
                else
                {
                    questionModel.subjectId = (Guid)subjectId;
                }

                //匹配解析
                Regex analyiseRegex = new Regex(@"^(解析：)");
                if (analyiseRegex.IsMatch(para.Text))
                {
                    questionModel.analysis = para.Text.Replace("解析:", "");
                    //d.option = para.Text;
                    Console.WriteLine("解析：" + para.Text);

                    Console.WriteLine("一道题结束");
                    //questionModel.subjectId = subjectId;
                    questionModel.items = optionList;
                    questionlist.Add(questionModel);
                    index++;
                    //type = new QuestionType();
                    optionList = new List<ImportQuestionItem>();
                    questionModel = new ImportQuestionFromWord();
                    continue;
                }
            }
            return questionlist;
        }

        public async Task<List<QuestionType>> GetQuestionTypes()
        {
            if (await RedisHelper.ExistsAsync("questionType"))
            {
                string typeStr = await RedisHelper.GetAsync("questionType");
                return JsonHelper.JsonDeserialize<List<QuestionType>>(typeStr);
            }
            var types = await _questionTypeRepo.getListAsync(u => u.IsDeleted == 0);
            await RedisHelper.SetAsync("questionType", JsonHelper.JsonSerialize(types));
            return types;
        }
    }
}
