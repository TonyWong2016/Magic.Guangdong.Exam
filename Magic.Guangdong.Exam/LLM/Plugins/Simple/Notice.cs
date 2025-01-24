using Magic.Guangdong.Assistant;
using Microsoft.SemanticKernel;
using MimeKit;
using System.ComponentModel;

namespace Magic.Guangdong.Exam.LLM.Plugins.Simple
{
    public class Notice(IServiceProvider serviceProvider)
    {
        private IServiceProvider _serviceProvider = serviceProvider;

        [KernelFunction("send_notice_by_email")]
        [Description("发送通知到指定邮箱")]
        [return: Description("如果发送任务执行成功直接返回true，反之返回false")]
        public async Task<bool> SendNoticeByEmail(string email, string content)
        {
            Assistant.Logger.Warning("发送邮件插件正确执行");
            try
            {
                var address = new List<MailboxAddress>
                {
                    new MailboxAddress(email,email)
                };
                await EmailKitHelper.SendEMailAsync("通知", content, address);
                return true;
            }
            catch (Exception ex)
            {
                Assistant.Logger.Error($"邮件发送失败，收件人{email}，" + ex);
                return false;
            }
        }
    }
}
