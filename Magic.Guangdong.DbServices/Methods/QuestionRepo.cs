using FreeSql.Internal.Model;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Newtonsoft.Json;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class QuestionRepo : ExaminationRepository<Question>, IQuestionRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public QuestionRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public dynamic GetQuestions(PageDto dto, out long total)
        {
            if (string.IsNullOrWhiteSpace(dto.whereJsonStr))
            {
                //return fsql.Get(conn_str).Select<Question, QuestionType, Subject>()
                //        .LeftJoin((a, b, c) => a.TypeId == b.Id)
                //        .LeftJoin((a, b, c) => a.SubjectId == c.Id)
                //        .Where((a, b, c) => a.IsDeleted == 0)
                //        .Count(out total)
                //        .Page(dto.pageindex, dto.pagesize)
                //        .ToList((a, b, c) => new
                //        {
                //            a.Id,
                //            a.TitleText,
                //            //a.Analysis,
                //            a.Remark,
                //            a.Author,
                //            a.CreatedBy,
                //            a.CreatedAt,
                //            a.TypeId,
                //            a.SubjectId,
                //            a.Score,
                //            type = b.Caption,
                //            subject = c.Caption
                //        });
                return fsql.Get(conn_str).Select<QuestionView>()
                        .Where(a => a.IsDeleted == 0)
                        .Count(out total)
                        .Page(dto.pageindex, dto.pagesize)
                        .ToList(a => new
                        {
                            a.Id,
                            a.TitleText,
                            //a.Analysis,
                            a.Remark,
                            a.Author,
                            a.CreatedBy,
                            a.CreatedAt,
                            a.TypeId,
                            a.SubjectId,
                            a.Score,
                            type = a.TypeName,
                            subject = a.SubjectName
                        });
            }
            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(dto.whereJsonStr);
            //return fsql.Get(conn_str).Select<Question, QuestionType, Subject>()
            //            .LeftJoin((a, b, c) => a.TypeId == b.Id)
            //            .LeftJoin((a, b, c) => a.SubjectId == c.Id)
            //            .Where((a, b, c) => a.IsDeleted == 0)
            //            .WhereDynamicFilter(dyfilter)
            //            .Count(out total)
            //            .Page(dto.pageindex, dto.pagesize)
            //            .ToList((a, b, c) => new
            //            {
            //                a.Id,
            //                a.TitleText,
            //                //a.Analysis,
            //                a.Remark,
            //                a.Author,
            //                a.CreatedBy,
            //                a.CreatedAt,
            //                a.TypeId,
            //                a.SubjectId,
            //                a.Score,
            //                type = b.Caption,
            //                subject = c.Caption
            //            });
            return fsql.Get(conn_str).Select<QuestionView>()
                        .Where(a => a.IsDeleted == 0)
                        .WhereDynamicFilter(dyfilter)
                        .Count(out total)
                        .Page(dto.pageindex, dto.pagesize)
                        .ToList(a => new
                        {
                            a.Id,
                            a.TitleText,
                            //a.Analysis,
                            a.Remark,
                            a.Author,
                            a.CreatedBy,
                            a.CreatedAt,
                            a.TypeId,
                            a.SubjectId,
                            a.Score,
                            a.Degree,
                            type = a.TypeName,
                            subject = a.SubjectName
                        });
        }


        /// <summary>
        /// 新增或更新单条题目
        /// </summary>
        /// <param name="question"></param>
        /// <param name="questionItems"></param>
        /// <returns></returns>
        public async Task<int> AddOrUpdateSingleQuestion(Question question, List<QuestionItem> questionItems)
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var questionRepo = fsql.Get(conn_str).GetRepository<Question>();
                    
                    await questionRepo.InsertOrUpdateAsync(question);

                    var questionItemRepo = fsql.Get(conn_str).GetRepository<QuestionItem>();
                    if (questionItemRepo.Where(u => u.QuestionId == question.Id).Any())
                    {
                        var existItems = questionItemRepo.Where(u => u.QuestionId == question.Id).ToList();
                        foreach (var item in existItems)
                        {
                            item.Remark = "重新绑定";
                            item.UpdatedBy = question.UpdatedBy;
                            item.UpdatedAt = DateTime.Now;
                            item.IsDeleted = 1;
                            await questionItemRepo.UpdateAsync(item);
                        }
                    }

                    foreach (var item in questionItems)
                    {
                        item.QuestionId = question.Id;
                        item.UpdatedBy = question.UpdatedBy;
                        item.CreatedBy = question.CreatedBy;
                        item.Remark = "创建选项";

                        item.DescriptionText = Utils.StripHTML(item.Description);
                    }
                    await questionItemRepo.InsertAsync(questionItems);
                    uow.Commit();
                    return 1;
                }
                catch
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 从excel里导入题库
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task<int> ImportQuestionFromExcel(List<ImportQuestionDto> items)
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var questionRepo = fsql.Get(conn_str).GetRepository<Question>();
                    var questionItemRepo = fsql.Get(conn_str).GetRepository<QuestionItem>();

                    List<Question> listQuestion = new List<Question>();
                    List<QuestionItem> questionItems = new List<QuestionItem>();
                    foreach (var item in items)
                    {
                        var question = new Question()
                        {
                            Title = item.Title,
                            TypeId = item.QuestionTypeId,
                            SubjectId = item.SubjectId,
                            Author = "excel",
                            Remark = item.Remark,
                            Analysis = string.IsNullOrEmpty(item.Analysis) ? "无" : item.Analysis,
                            CreatedBy = item.CreateBy,
                            Score = item.Score,
                            Degree = item.Degree,
                            ColumnId = item.ColumnId
                        };
                        questionItems.AddRange(MakeQuestionItems(item, question.Id));
                        if (questionItems.Count > 1000)
                        {
                            await questionItemRepo.InsertAsync(questionItems);
                            questionItems = new List<QuestionItem>();
                        }
                        listQuestion.Add(question);
                        if (listQuestion.Count > 1000)
                        {
                            await questionRepo.InsertAsync(listQuestion);
                        }
                    }
                    await questionItemRepo.InsertAsync(questionItems);
                    await questionRepo.InsertAsync(listQuestion);
                    uow.Commit();
                    return 1;
                }
                catch
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        private List<QuestionItem> MakeQuestionItems(ImportQuestionDto dto, long QuestionId)
        {
            List<QuestionItem> questionItems = new List<QuestionItem>();
            if (!string.IsNullOrEmpty(dto.Option1))
            {
                questionItems.Add(new QuestionItem()
                {
                    Code = dto.QuestionType.Contains("判断") ? "对" : "A",
                    Description = dto.QuestionType.Contains("判断") ? " " : dto.Option1,
                    CreatedBy = dto.CreateBy,
                    IsAnswer = (dto.Answer.Contains("1") || dto.Answer.Contains("对") || dto.Answer.ToUpper().Contains("A")) ? 1 : (dto.QuestionType.Contains("填空") || dto.QuestionType.Contains("问答")) ? 1 : 0,
                    QuestionId = QuestionId,
                    OrderIndex = 0
                });
            }
            if (!string.IsNullOrEmpty(dto.Option2))
            {
                questionItems.Add(new QuestionItem()
                {
                    Code = dto.QuestionType.Contains("判断") ? "错" : "B",
                    Description = dto.QuestionType.Contains("判断") ? " " : dto.Option2,
                    CreatedBy = dto.CreateBy,
                    IsAnswer = (dto.Answer.Contains("2") || dto.Answer.Contains("错") || dto.Answer.ToUpper().Contains("B")) ? 1 : (dto.QuestionType.Contains("填空") || dto.QuestionType.Contains("问答")) ? 1 : 0,
                    QuestionId = QuestionId,
                    OrderIndex = 1
                });
            }
            if (!string.IsNullOrEmpty(dto.Option3))
            {
                questionItems.Add(new QuestionItem()
                {
                    Code = "C",
                    Description = dto.Option3,
                    CreatedBy = dto.CreateBy,
                    IsAnswer = (dto.Answer.Contains("3") || dto.Answer.ToUpper().Contains("C")) ? 1 : (dto.QuestionType.Contains("填空") || dto.QuestionType.Contains("问答")) ? 1 : 0,
                    QuestionId = QuestionId,
                    OrderIndex = 2
                });
            }
            if (!string.IsNullOrEmpty(dto.Option4))
            {
                questionItems.Add(new QuestionItem()
                {
                    Code = "D",
                    Description = dto.Option4,
                    CreatedBy = dto.CreateBy,
                    IsAnswer = (dto.Answer.Contains("4") || dto.Answer.ToUpper().Contains("D")) ? 1 : (dto.QuestionType.Contains("填空") || dto.QuestionType.Contains("问答")) ? 1 : 0,
                    QuestionId = QuestionId,
                    OrderIndex = 3
                });
            }
            if (!string.IsNullOrEmpty(dto.Option5))
            {
                questionItems.Add(new QuestionItem()
                {
                    Code = "E",
                    Description = dto.Option5,
                    CreatedBy = dto.CreateBy,
                    IsAnswer = (dto.Answer.Contains("5") || dto.Answer.ToUpper().Contains("E")) ? 1 : (dto.QuestionType.Contains("填空") || dto.QuestionType.Contains("问答")) ? 1 : 0,
                    QuestionId = QuestionId,
                    OrderIndex = 4
                });
            }
            if (!string.IsNullOrEmpty(dto.Option6))
            {
                questionItems.Add(new QuestionItem()
                {
                    Code = "F",
                    Description = dto.Option6,
                    CreatedBy = dto.CreateBy,
                    IsAnswer = (dto.Answer.Contains("6") || dto.Answer.ToUpper().Contains("F")) ? 1 : (dto.QuestionType.Contains("填空") || dto.QuestionType.Contains("问答")) ? 1 : 0,
                    QuestionId = QuestionId,
                    OrderIndex = 5
                });
            }
            return questionItems;
        }

        /// <summary>
        /// 从word导入题目
        /// </summary>
        /// <param name="Items"></param>
        /// <returns></returns>
        public async Task<int> ImportQuestionsFromWord(List<ImportQuestionFromWord> Items)
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var questionRepo = fsql.Get(conn_str).GetRepository<Question>();
                    var questionItemRepo = fsql.Get(conn_str).GetRepository<QuestionItem>();
                    //await questionRepo.InsertOrUpdateAsync(question);
                    List<Question> listQuestion = new List<Question>();
                    foreach (var item in Items)
                    {
                        if (item.degree == "容易")
                            item.degree = "easy";
                        else if (item.degree == "困难")
                            item.degree = "difficult";
                        else
                            item.degree = "normal";
                        var question = new Question()
                        {
                            Title = item.title,
                            TypeId = item.typeId,
                            SubjectId = item.subjectId,
                            Author = "word",
                            Remark = "word导入",
                            Analysis = item.analysis,
                            CreatedBy = item.createdby,
                            Score = item.score,
                            Degree = item.degree,
                        };

                        List<QuestionItem> questionItems = new List<QuestionItem>();
                        int i = 0;
                        foreach (var subItem in item.items)
                        {
                            questionItems.Add(new QuestionItem()
                            {
                                QuestionId = question.Id,
                                Description = subItem.description,
                                Code = subItem.code,
                                IsAnswer = subItem.isAnswer,
                                CreatedBy = question.CreatedBy,
                                Remark = "word导入",
                                OrderIndex = subItem.index == 0 ? i : subItem.index,
                                IsOption = subItem.code == "主观题" ? 0 : 1
                            });
                            i++;
                        }
                        listQuestion.Add(question);

                        await questionItemRepo.InsertAsync(questionItems);
                    }
                    await questionRepo.InsertAsync(listQuestion);


                    uow.Commit();
                    return 1;
                }
                catch
                {
                    uow.Rollback();
                    throw;
                }
            }

        }


    }

}
