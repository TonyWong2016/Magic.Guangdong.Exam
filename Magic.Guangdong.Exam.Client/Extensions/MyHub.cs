using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;

namespace Magic.Guangdong.Exam.Client.Extensions
{
    public class MyHub : Hub
    {
        //private Common.RedisExchange redis = new Common.RedisExchange();
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected->User:" + this.Context.User);
            Console.WriteLine("Connected->UserIdentifier:" + this.Context.UserIdentifier);
            Console.WriteLine("Connected->ConnectionId:" + this.Context.ConnectionId);
            //var tmp = redis.HashKeys("chathash");
            //int user_cnt = redis.HashKeys("chathash").Count();
            int user_cnt = RedisHelper.HKeys("chathash").Length;
            base.Clients.All.SendAsync("OnlineCount", user_cnt.ToString());//推送全局，也可以推送给指定用户
            if (!string.IsNullOrEmpty(this.Context.UserIdentifier))
                base.Clients.All.SendAsync("OnlineMsg", this.Context.UserIdentifier + "|" + this.Context.User.Identity.Name);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("Disconnected->ConnectionId:" + this.Context.UserIdentifier);
            var path = this.Context.GetHttpContext().Request;
            if (!string.IsNullOrEmpty(this.Context.UserIdentifier))
            {
                base.Clients.All.SendAsync("Offline", this.Context.UserIdentifier + "|" + this.Context.User.Identity.Name);//推送全局，也可以推送给指定用户
                //redis.HashDelete("chathash", this.Context.UserIdentifier);
                //int user_cnt = redis.HashKeys("chathash").Count();

                int user_cnt = RedisHelper.HKeys("chathash").Length;
                base.Clients.All.SendAsync("OnlineCount", user_cnt.ToString());//推送全局，也可以推送给指定用户
            }
            return base.OnDisconnectedAsync(exception);
        }

        //发送消息--发送给所有连接的客户端
        public Task SendMessage(string msg)
        {
            return Clients.All.SendAsync("ReceiveMessage", msg);
        }

        //发送消息--发送给所有连接的客户端
        public async Task SendMessage2(string userId, string msg)
        {
            Console.WriteLine($"user:{userId},msg:{msg},{DateTime.Now}");
            await Clients.All.SendAsync("ReceiveMessage", userId, msg);
        }

        //发送消息--发送给指定用户
        //默认情况下，使用 SignalRClaimTypes.NameIdentifier从ClaimsPrincipal与作为用户标识符连接相关联
        //所以使用自带授权Authorize登陆时，可以把用户id保存在NameIdentifier中
        public Task SendPrivateMessage(string userId, string message)
        {
            Console.WriteLine($"user:{userId},msg:{message},{DateTime.Now}");
            return Clients.User(userId).SendAsync("ReceiveMessage", message);
        }
    }
}
