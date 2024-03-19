using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dto.Routers
{
    public class RouterDto
    {
        public string controller { get; set; }

        public string action { get; set; }

        public string area { get; set; }

        public string router
        {
            get
            {
                if (string.IsNullOrEmpty(area))
                    return $"{controller}/{action}";
                else
                    return $"{area}/{controller}/{action}";
            }
        }
    }
}
