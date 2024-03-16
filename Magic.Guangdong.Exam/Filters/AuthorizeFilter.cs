using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Magic.Guangdong.Exam.Filters
{
    /// <summary>
    /// 实现自定义授权
    /// </summary>
    public class AuthorizeFilter : IAuthorizationFilter
    {
        /// <summary>
        /// 请求验证，当前验证部分不要抛出异常，ExceptionFilter不会处理
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //这里可以做复杂的权限控制操作
            //if (context.HttpContext.User.Identity.Name != "1") //简单的做一个示范
            //{            　　　　　　　　
            //    //未通过验证则跳转到无权限提示页 
            //    RedirectToActionResult content = new RedirectToActionResult("NoAuth", "Exception", null);            　　　　　　　　//    context.Result = content;            　　　　　　　　//

            if (context.RouteData.Values.ContainsKey("area"))
            {
                string area = context.RouteData.Values["area"].ToString();
                string controller = context.RouteData.Values["controller"].ToString().ToLower();
                string action = context.RouteData.Values["action"].ToString().ToLower();
                var cookies = context.HttpContext.Request.Cookies;
                if (!cookies.Any(u => u.Key == "adminid") && action.ToLower() != "login")
                {
                    //var item = new ContentResult();
                    //item.Content = "您无权访问当前地址或执行该操作，可能是登录超时造成的，请尝试重新登陆";
                    //item.StatusCode = 200;                    
                    //context.Result = item;
                    //if (controller != "face" && controller != "trtc")
                    //{
                    //    var item2 = new RedirectToActionResult("Login", "Admin", new { msg = "timeout" });
                    //    context.Result = item2;
                    //}

                }
                else
                {
                    string adminid = cookies["adminId"];
                    if (controller == "admin" && action == "scan")
                    {

                    }
                }
            }
        }
    }
}
