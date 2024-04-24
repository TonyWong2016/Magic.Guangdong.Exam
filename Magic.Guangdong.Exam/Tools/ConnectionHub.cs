using Microsoft.AspNetCore.SignalR;

namespace Magic.Guangdong.Exam.Tools
{
    public class ConnectionHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ConnectionHub(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// 连接建立时
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            string userId = "system";
            if (!string.IsNullOrEmpty(GetCookieValue("userId")))
            {
                userId = GetCookieValue("userId");
            }
            
            Console.WriteLine("Connected->User:" + userId);
            //RedisHelper.HSetAsync("adminConnections", userId, userId);
            int user_cnt = RedisHelper.HKeys("onLineForOA").Count();
            base.Clients.All.SendAsync("OnlineCount", user_cnt.ToString());//推送全局，也可以推送给指定用户
            if (userId!= "system")
                base.Clients.All.SendAsync("OnlineMsg", userId);
            return base.OnConnectedAsync();
        }
        /// <summary>
        /// 连接丢失时
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            string userId = "";
            if (!string.IsNullOrEmpty(GetCookieValue("userId")))
            {
                userId = GetCookieValue("userId");
            }
            if (!string.IsNullOrEmpty(userId))
            {
                base.Clients.All.SendAsync("Offline", userId);//推送全局，也可以推送给指定用户
                
            }
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 发送消息--发送给所有连接的客户端
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task BroadcastMessage(string userId, string msg)
        {
            Console.WriteLine($"user:{userId},msg:{msg},{DateTime.Now}");
            return Clients.All.SendAsync("ReceiveMessage", msg);
        }

        /// <summary>
        /// 发送消息--发送给指定用户
        /// 默认情况下，使用 SignalRClaimTypes.NameIdentifier从ClaimsPrincipal与作为用户标识符连接相关联
        /// 所以使用自带授权Authorize登陆时，可以把用户id保存在NameIdentifier中
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendPrivateMessage(string userId, string message)
        {
            return Clients.User(userId).SendAsync("ReceiveMessage", message);
        }

        public string GetCookieValue(string cookieName)
        {
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(cookieName, out string cookieValue))
            {
                return cookieValue;
            }

            return ""; // 或者返回默认值，表示未找到对应的cookie
        }

    }
}
