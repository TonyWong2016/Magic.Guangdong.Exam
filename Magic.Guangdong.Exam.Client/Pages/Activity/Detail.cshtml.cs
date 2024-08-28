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

        public string? Deacription { get; set; } = "��";

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public int Status { get; set; }

        public DateTime EndTime { get; set; }

        public int IsDeleted {  get; set; }

        public int CanIReport { get; set; } = 0;

        public string Msg { get; set; }
        public void OnGet(string? id="")
        {
            if (!string.IsNullOrEmpty(id) && _activityRepo.getAnyAsync(u => u.Id == Convert.ToInt64(id)).Result)
            {
                Id = id;
                var activity = _activityRepo.getOneAsync(u => u.Id == Convert.ToInt64(id)).Result;
                Deacription = activity.Description;
                Title = activity.Title;
                StartTime = activity.StartTime;
                EndTime = activity.EndTime;
                Status=activity.Status;
                IsDeleted = activity.IsDeleted;
                if (StartTime > DateTime.Now)
                {
                    CanIReport = 1;
                    Msg = "δ������ʱ��";
                }
                else if (DateTime.Now > EndTime)
                {
                    CanIReport = 2;
                    Msg = "��ѹ���";
                }
                if (Status != 0)
                {
                    CanIReport= 3;
                    Msg = "��ǰ�δ���ű���";

                }

                Console.WriteLine(Deacription);
            }
            
        }
    }
}
