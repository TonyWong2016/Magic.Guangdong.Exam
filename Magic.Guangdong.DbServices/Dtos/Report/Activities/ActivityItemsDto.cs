using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Report.Activities
{
    public class ActivityItemsDto
    {
        public long Id { get; set; }

        public string Cover {  get; set; }

        public string Description { get; set; }

        public string FieldJson { get; set; }

        public int Status { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int CanIReport 
        {
            get
            {
                if (StartTime > DateTime.Now)
                    return 1;
                if(DateTime.Now> EndTime) 
                    return 2;
                if(Status!=0)
                    return 3;
                return 0;
            }
        }
    }
}
