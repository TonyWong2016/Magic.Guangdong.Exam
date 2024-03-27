using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Dtos.Routers
{
    public class RouterDto
    {
        public string Controller { get; set; }

        public string Action { get; set; }

        public string Area { get; set; }

        public string Name { get; set; }  



        public string Router
        {
            get
            {
                if (string.IsNullOrEmpty(Area))
                    return $"/{Controller.Replace("Controller", "")}/{Action}".ToLower();
                else
                    return $"/{Area}/{Controller.Replace("Controller", "")}/{Action}".ToLower();
            }
        }
    }

    public class RouterGroupDto
    {
        //public string area { get; set; }

        public string group { get; set; }

        public List<RouterDto> routes { get; set; }
    }
}
