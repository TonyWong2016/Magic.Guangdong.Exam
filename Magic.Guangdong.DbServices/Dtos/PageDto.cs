using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos
{
    public class PageDto
    {
        public string whereJsonStr { get; set; }

        public int pageindex { get; set; } = 1;

        public int pagesize { get; set; } = 10;

        public bool isAsc { get; set; } = true;

        public string orderby { get; set; } = "id";

        public long tagId { get; set; } = 0;
    }

    public class AdminListPageDto : PageDto
    {
        public long[] roleIds { get; set; }
    }

    public class PageResponse
    {
        public long total { get; set; }

        public dynamic items { get; set; }
    }
}
