using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Magic.Guangdong.Exam.Extensions
{
    public class RouteMark : Attribute
    {
        public string Permission { get; set; }
        public string Module { get; set; }

        public RouteMark()
        {

        }

        public RouteMark(string permission, string module)
        {
            Permission = permission;
            Module = module;
        }

        public RouteMark(string module)
        {
            Module = module;
        }

    }
}
