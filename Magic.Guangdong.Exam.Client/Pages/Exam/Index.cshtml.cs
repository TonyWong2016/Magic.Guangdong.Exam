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
        private readonly IResponseHelper _resp;
        private readonly IExaminationRepo _examRepo;
        public string _examId { get; set; } = "";
        public string _groupCode { get; set; } = "";
        public IndexModel(IResponseHelper responseHelper,IExaminationRepo examRepo)
        {
            _resp = responseHelper;
            _examRepo = examRepo;
        }

        public IActionResult OnGet(string examId,string groupCode)
        {
            if(string.IsNullOrEmpty(examId) && string.IsNullOrEmpty(groupCode))           
            {
                Assistant.Logger.Error("非法请求");
                return Redirect("/Error?msg=" + Assistant.Utils.EncodeUrlParam("非法请求"));

            }
            _examId = examId;
            _groupCode = groupCode;
            return Page();
        }

    }
}
