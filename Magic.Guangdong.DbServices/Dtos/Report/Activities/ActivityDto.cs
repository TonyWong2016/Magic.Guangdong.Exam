using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Dtos.Report.Activities
{
    public class ActivityDto
    {
        public long Id { get; set; } = YitIdHelper.NextId();

        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Status { get; set; } = 0;

        public int Quota { get; set; } = 0;

        public decimal Expenses { get; set; } = 0;

        public string FieldJson { get; set; } = "{}";
    }
}
