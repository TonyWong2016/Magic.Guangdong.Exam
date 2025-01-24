using Magic.Guangdong.Assistant.CloudModels;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.Exam.Controllers;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Magic.Guangdong.Exam.LLM.Plugins.Checking
{
    public class GetReportInfo(IServiceProvider serviceProvider)
    {
        private IServiceProvider _serviceProvider = serviceProvider;

        [KernelFunction("get_apply_by_number")]
        [Description("通过赛队编号获取赛队信息")]
        [return: Description("如果编号存在，返回编号所属赛队详情，反之返回空")]
        public ApplyModel? GetApplyInfoByNumber(string number)
        {
            Assistant.Logger.Warning("找人插件正确执行");
            var _en = _serviceProvider.GetService<IWebHostEnvironment>();
            List<ApplyModel> list = new List<ApplyModel>() 
            {
                 new ApplyModel()
                {
                    ProjectType = "航天科技创新赛",
                    Area = "北京东城区",
                    TeamNumber = "AA24030004",
                    ApplyMembers = [
                        new ApplyMember {
                            Group="初中三年级",
                            Name="郭欣诺",
                            IdCard="110103200903260943",
                            School = "北京市第十一中学",
                            Role = "参赛选手"
                        },
                        new ApplyMember {
                            Group="初中三年级",
                            Name="周云清",
                            IdCard="110111200903037047",
                            School = "北京市第十一中学",
                            Role = "参赛选手"
                        },
                        new ApplyMember {
                            Group="初中三年级",
                            Name="宋英伽",
                            IdCard="110111200911170315",
                            School = "北京市第十一中学",
                            Role = "参赛选手"
                        },
                        new ApplyMember {
                            Group="",
                            Name="孟颖",
                            IdCard="410724198901242042",
                            School = "北京市第十一中学",
                            Role = "指导老师"
                        },
                        new ApplyMember {
                            Group="",
                            Name="蔡葆元",
                            IdCard="110103195410290673",
                            School = "北京市第十一中学",
                            Role = "指导老师"
                        }
                    ],
                    Schools = "北京市第十一中学",
                    MemberSign = "周云清，郭欣诺，宋英伽",
                    FileName =  Path.Combine($"{_en.WebRootPath}", "upfile", "test", "申报表1.pdf"),
                    Email="67101342@qq.com"
                },
                new ApplyModel()
                {
                    ProjectType = "航天科技创新赛",
                    Area = "浙江杭州市",
                    TeamNumber = "BA24030035",
                    ApplyMembers = [
                        new ApplyMember {
                            Group="初中三年级",
                            Name="宋宇宁",
                            IdCard="330103200811111044",
                            School = "杭州外国语学校",
                            Role = "参赛选手"
                        },
                        new ApplyMember {
                            Group="初中三年级",
                            Name="曾昱希",
                            IdCard="330106200810124029",
                            School = "杭州外国语学校",
                            Role = "参赛选手"
                        },
                        new ApplyMember {
                            Group="初中三年级",
                            Name="范欣航",
                            IdCard="330102200901201223",
                            School = "杭州外国语学校",
                            Role = "参赛选手"
                        },
                        new ApplyMember {
                            Group="",
                            Name="刘勇",
                            IdCard="421004198111133035",
                            School = "杭州外国语学校",
                            Role = "指导老师"
                        }
                    ],
                    Schools = "杭州外国语学校",
                    MemberSign = "宋宇宁，曾昱希，范欣航",
                    FileName = Path.Combine($"{_en.WebRootPath}", "upfile", "test", "申报表2.pdf"),
                    Email="wangteng@xxt.org.cn"
                },
               
                new ApplyModel()
                {
                    ProjectType = "航天科技创新赛",
                    Area = "北京海淀区",
                    TeamNumber = "BA24030004",
                    ApplyMembers = [
                        new ApplyMember {
                            Group="初中一年级",
                            Name="郭纳睿",
                            IdCard="110108201101261422",
                            School = "北京市八一学校",
                            Role = "参赛选手"
                        },new ApplyMember {
                            Group="初中三年级",
                            Name="史佳凝",
                            IdCard="110108201106076824",
                            School = "北京市八一学校",
                            Role = "参赛选手"
                        },new ApplyMember {
                            Group="",
                            Name="刘婧宇",
                            IdCard="370102199412184129",
                            School = "北京市八一学校",
                            Role = "指导老师"
                        }
                    ],
                    Schools = "北京市八一学校",
                    MemberSign = "郭纳睿，史佳凝",
                    FileName = Path.Combine($"{_en.WebRootPath}", "upfile", "test", "申报表3.pdf"),
                }
            };
            if (list.Where(u => u.TeamNumber == number).Any())
                return list.Where(u => u.TeamNumber == number).FirstOrDefault();
            return null;
        }

        [KernelFunction("get_document_info")]
        [Description("识别文档内容并返回识别后的信息")]
        [return: Description("当文件路径并存在或者识别失败时，返回空，反之则返回识别后的信息")]
        public async Task<OcrResponseDto?> GetDocumentInfo(string pdfpath)
        {
            Assistant.Logger.Warning("文档识别插件正确执行");
            var _baiduOcrHelper = _serviceProvider.GetRequiredService<IBaiduOcrHelper>();
            return await _baiduOcrHelper.DocumentRecognition(pdfpath);
        }
    }
}
