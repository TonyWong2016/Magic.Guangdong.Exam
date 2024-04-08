using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Magic.Guangdong.Exam.Client.Pages.Activity
{
    public class DetailModel : PageModel
    {
        private readonly IActivityRepo _activityRepo;
        public DetailModel(IActivityRepo activityRepo)
        {
            _activityRepo = activityRepo;
        }
        [BindProperty]
        public string? Id { get; set; }


        [BindProperty]
        public string? Deacription { get; set; } = "нч";
        public void OnGet(string? id="")
        {
            if (!string.IsNullOrEmpty(id) && _activityRepo.getAnyAsync(u => u.Id == Convert.ToInt64(id)).Result)
            {
                Id = id;
                var activity = _activityRepo.getOneAsync(u => u.Id == Convert.ToInt64(id)).Result;
                Deacription = activity.Description;
                Console.WriteLine(Deacription);
            }
            
        }
    }
}
