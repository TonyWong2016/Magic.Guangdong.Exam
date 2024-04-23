using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Magic.Guangdong.Exam.Client.Pages.Exam
{
    public class HistoryModel : PageModel
    {
        private readonly IUserAnswerRecordClientRepo _userAnswerRecordClientRepo;
        private readonly IRedisCachingProvider _redisCachingProvider;
        public HistoryModel(IUserAnswerRecordClientRepo userAnswerRecordClientRepo,IRedisCachingProvider redisCachingProvider)
        {
            _userAnswerRecordClientRepo = userAnswerRecordClientRepo;
            _redisCachingProvider = redisCachingProvider;
        }
        

        public List<ExamRecordDto> myExamHistories { get; set; }

        public async Task<IActionResult> OnGet(string? reportId,string? accountId)
        {
            try
            {
                //优先使用reportId
                if (!string.IsNullOrEmpty(reportId))
                {
                    if (!await _redisCachingProvider.KeyExistsAsync("myReportExamHistories_" + reportId))
                    {
                        myExamHistories = await _userAnswerRecordClientRepo.GetMyReportExamRecords(reportId);
                        await _redisCachingProvider.StringSetAsync("myReportExamHistories_" + reportId,
                            JsonHelper.JsonSerialize(myExamHistories), TimeSpan.FromMinutes(10));
                    }
                    myExamHistories = JsonHelper.JsonDeserialize<List<ExamRecordDto>>(await _redisCachingProvider.StringGetAsync("myReportExamHistories_" + reportId));
                }
                else if (!string.IsNullOrEmpty(accountId))
                {
                    if (!await _redisCachingProvider.KeyExistsAsync("myAccountExamHistories_" + accountId))
                    {
                        myExamHistories = await _userAnswerRecordClientRepo.GetMyAccountExamRecords(accountId);
                        await _redisCachingProvider.StringSetAsync("myAccountExamHistories_" + accountId,
                            JsonHelper.JsonSerialize(myExamHistories), TimeSpan.FromMinutes(10));
                    }
                    myExamHistories = JsonHelper.JsonDeserialize<List<ExamRecordDto>>(await _redisCachingProvider.StringGetAsync("myAccountExamHistories_" + accountId));

                }
            }
            catch(Exception ex)
            {
                return Redirect("/Error?msg=" + Utils.EncodeUrlParam("获取答题记录异常：" + ex.Message));
            }
            return Page();
        }
    }
}
