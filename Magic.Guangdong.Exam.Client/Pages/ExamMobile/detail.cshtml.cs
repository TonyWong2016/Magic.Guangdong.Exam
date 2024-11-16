using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos.Exam.Papers;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Magic.Guangdong.Exam.Client.Pages.ExamMobile
{
    public class DetailModel : PageModel
    {
        private readonly IPaperRepo _paperRepo;
        private readonly IUserAnswerRecordClientRepo _userAnswerRecordClientRepo;
        private readonly IRedisCachingProvider _redisCachingProvider;
        public DetailModel(IPaperRepo paperRepo, IUserAnswerRecordClientRepo userAnswerRecordClientRepo,IRedisCachingProvider redisCachingProvider)
        {
            _paperRepo = paperRepo;
            _userAnswerRecordClientRepo = userAnswerRecordClientRepo;
            _redisCachingProvider = redisCachingProvider;
        }

        [BindProperty]
        public FinalPaperDto paperDto { get; set; }

        [BindProperty]
        public UserAnswerRecordView record { get; set; }

        public async Task<IActionResult> OnGet(long urid)
        {
            if (!await _userAnswerRecordClientRepo.getAnyAsync(u=>u.Id==urid))
            {
                return Redirect("/Error?msg=" + Utils.EncodeUrlParam("答题记录不存在"));
            }

            if (!await _redisCachingProvider.KeyExistsAsync($"userRecord_" + urid))
            {
                record = await _userAnswerRecordClientRepo.GetMyRecord(urid);
                await _redisCachingProvider.StringSetAsync($"userRecord_" + urid, 
                    JsonHelper.JsonSerialize(record),
                    DateTime.Now.AddMinutes(10) - DateTime.Now);
            }
            else
            {
                record = JsonHelper.JsonDeserialize<UserAnswerRecordView>(
                    await _redisCachingProvider.StringGetAsync($"userRecord_" + urid));
            }
            
            if(!await _redisCachingProvider.KeyExistsAsync($"userPaper_" + urid))
            {
                paperDto = await _paperRepo.PreviewPaper(record.PaperId);
                await _redisCachingProvider.StringSetAsync($"userPaper_" + urid, 
                    JsonHelper.JsonSerialize(paperDto), 
                    DateTime.Now.AddMinutes(10) - DateTime.Now);
            }
            else
            {
                paperDto = JsonHelper.JsonDeserialize<FinalPaperDto>(
                    await _redisCachingProvider.StringGetAsync($"userPaper_" + urid));
            }
            if (paperDto.Questions==null || !paperDto.Questions.Any())
            {
                return Redirect("/Error?msg=" + Utils.EncodeUrlParam("没有有效的答题记录"));

            }
            return Page();
        }
    }
}
