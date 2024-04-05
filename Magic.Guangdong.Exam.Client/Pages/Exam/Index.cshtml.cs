using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq.Expressions;

namespace Magic.Guangdong.Exam.Client.Pages.Exam
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IResponseHelper _resp;
        private readonly IExaminationRepo _examRepo;
        public IndexModel(ILogger<IndexModel> logger,IResponseHelper responseHelper,IExaminationRepo examRepo)
        {
            _logger = logger;
            _resp = responseHelper;
            _examRepo = examRepo;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostGetExams(PageDto dto)
        {            
            return new JsonResult(_resp.success(
                await _examRepo.getListAsync(dto)));
            
        }
    }
}
