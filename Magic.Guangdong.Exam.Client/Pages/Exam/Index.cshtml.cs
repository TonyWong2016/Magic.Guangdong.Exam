using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace Magic.Guangdong.Exam.Client.Pages.Exam
{
    public class IndexModel : PageModel
    {
        private readonly IResponseHelper _resp;
        //private readonly IExaminationClientRepo _examRepo;
        public string examId { get; set; } = "";
        public string groupCode { get; set; } = "";

        public long reportId { get; set; }

        public string reportNumber { get; set; }
        public IndexModel(IResponseHelper responseHelper)
        {
            _resp = responseHelper;
        }

        public IActionResult OnGet(string examId,string groupCode,long? reportId)
        {
            if(string.IsNullOrEmpty(examId) && reportId==null)
            {
                Assistant.Logger.Error("非法请求");
                return Redirect("/Error?msg=" + Assistant.Utils.EncodeUrlParam("非法请求"));

            }
            
            this.examId = examId;
            this.groupCode = groupCode;
            if (reportId != null)
                this.reportId = (long)reportId;
            
            return Page();
        }

    }
}
