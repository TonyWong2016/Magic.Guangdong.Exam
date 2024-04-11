using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using NPOI.SS.Formula.Functions;
using System.Text;

namespace Magic.Guangdong.Assistant
{
    public static class EmailKitHelper
    {
        private static bool UseSsl { get; set; } = true;
        /// <summary>
        /// 发送邮件的账号名称
        /// </summary>
        private static string UserName { get; set; } = ConfigurationHelper.GetSectionValue("smsSign");
        private static EmailConfig _tconfig;

        /// <summary>
        /// 发送电子邮件，默认发送方为<see cref="UserAddress"/>
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="content">邮件内容主题</param>
        /// <param name="toAddress">接收方信息</param>
        /// <param name="textFormat">内容主题模式，默认TextFormat.Text</param>
        /// <param name="attachments">附件</param>
        /// <param name="dispose">是否自动释放附件所用Stream</param>
        /// <returns></returns>
        public static async Task<bool> SendEMailAsync(string subject, string content, IEnumerable<MailboxAddress> toAddress, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null, bool dispose = true)
        {
            IConfigurationSection[] emailCfgs = ConfigurationHelper.GetSections("emailPool");
            int rand = new Random().Next(0, emailCfgs.Length - 1);
            IConfigurationSection emailCfg = emailCfgs[rand];
            _tconfig = new EmailConfig()
            {
                Server = emailCfg.GetSection("Server").Value,
                Email = emailCfg.GetSection("Email").Value,
                Auth = emailCfg.GetSection("Auth").Value,
                Port = Convert.ToInt32(emailCfg.GetSection("port").Value)
            };
            if (string.IsNullOrEmpty(UserName))
                //UserName = UserAddress;
                UserName = _tconfig.Email;


            //await SendEMailAsync(subject, content, new MailboxAddress[] { new MailboxAddress(UserName, UserAddress) }, toAddress, textFormat, attachments, dispose).ConfigureAwait(false);
            return await SendEMailAsync(subject, content, new MailboxAddress[] { new MailboxAddress(UserName, _tconfig.Email) }, toAddress, textFormat, attachments, dispose).ConfigureAwait(false);

        }


        public static async Task<bool> SendVerificationCodeEmailAsync(string subject, string content, string toAddress,string toName, TextFormat textFormat = TextFormat.Html)
        {
            IConfigurationSection[] emailCfgs = ConfigurationHelper.GetSections("emailPool");
            int rand = new Random().Next(0, emailCfgs.Length - 1);
            IConfigurationSection emailCfg = emailCfgs[rand];
            _tconfig = new EmailConfig()
            {
                Server = emailCfg.GetSection("Server").Value,
                Email = emailCfg.GetSection("Email").Value,
                Auth = emailCfg.GetSection("Auth").Value,
                Port = Convert.ToInt32(emailCfg.GetSection("port").Value)
            };
            if (string.IsNullOrEmpty(UserName))
                //UserName = UserAddress;
                UserName = _tconfig.Email;
            if (string.IsNullOrEmpty(toName))
                toName = toAddress;
            
            //await SendEMailAsync(subject, content, new MailboxAddress[] { new MailboxAddress(UserName, UserAddress) }, toAddress, textFormat, attachments, dispose).ConfigureAwait(false);
            return await SendEMailAsync(subject, content, [new MailboxAddress(UserName, _tconfig.Email)], [new MailboxAddress(toName, toAddress)], textFormat, null, true).ConfigureAwait(false);

        }

        /// <summary>
        /// 给开发人员发送系统异常邮件
        /// </summary>
        /// <param name="msg1"></param>
        /// <param name="msg2"></param>
        /// <returns></returns>
        public static async Task SendEMailToDevExceptionAsync(Exception ex)
        {
            IConfigurationSection[] emailCfgs = ConfigurationHelper.GetSections("emailPool");
            int rand = new Random().Next(0, emailCfgs.Length - 1);
            IConfigurationSection emailCfg = emailCfgs[rand];
            _tconfig = new EmailConfig()
            {
                Server = emailCfg.GetSection("Server").Value,
                Email = emailCfg.GetSection("Email").Value,
                Auth = emailCfg.GetSection("Auth").Value,
                Port = Convert.ToInt32(emailCfg.GetSection("port").Value)
            };
            if (string.IsNullOrEmpty(UserName))
                UserName = _tconfig.Email;
            var address = new List<MailboxAddress>
            {
                new MailboxAddress("tony","wtlemon@126.com")
                };
            string templateFilePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "web", "error.html");
            //string content = templateFilePath.Replace("?error1?", ex.Message).Replace("?error2?", ex.StackTrace);
            string content = ex.Message + ex.StackTrace;
            using (StreamReader reader = new StreamReader(templateFilePath))
            {
                content = reader.ReadToEnd().Replace("?error1?", ex.Message).Replace("?error2?", ex.StackTrace);

            }
            var message = new MimeMessage
            {
                Subject = "系统异常",
                Priority = MessagePriority.Urgent,
                Date = DateTime.Now,
               
                Body = new BodyBuilder
                {
                    HtmlBody = content
                }.ToMessageBody()
            };
            message.From.Add(new MailboxAddress(UserName, _tconfig.Email));
            message.To.AddRange(address);
            using (var client = new SmtpClient())
            {
                
                await client.ConnectAsync(_tconfig.Server, _tconfig.Port, UseSsl);
                //await client.AuthenticateAsync(UserAddress, Password);
                await client.AuthenticateAsync(_tconfig.Email, _tconfig.Auth);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public static async Task SendEMailToDevMsgAsync(string msg)
        {
            IConfigurationSection[] emailCfgs = ConfigurationHelper.GetSections("emailPool");
            int rand = new Random().Next(0, emailCfgs.Length - 1);
            IConfigurationSection emailCfg = emailCfgs[rand];
            _tconfig = new EmailConfig()
            {
                Server = emailCfg.GetSection("Server").Value,
                Email = emailCfg.GetSection("Email").Value,
                Auth = emailCfg.GetSection("Auth").Value,
                Port = Convert.ToInt32(emailCfg.GetSection("port").Value)
            };
            if (string.IsNullOrEmpty(UserName))
                UserName = _tconfig.Email;
            var address = new List<MailboxAddress>
            {
                new MailboxAddress("tony","wtlemon@126.com")
                };
            string templateFilePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "web", "error.html");
            string content = "系统故障";
            
            using (StreamReader reader = new StreamReader(templateFilePath))
            {
                content = reader.ReadToEnd().Replace("?error1?", "发生故障").Replace("?error2?", msg);

            }
            var message = new MimeMessage
            {
                Subject = "系统异常",
                Priority = MessagePriority.Urgent,
                Date = DateTime.Now,

                Body = new BodyBuilder
                {
                    HtmlBody = content
                }.ToMessageBody()
            };
            message.From.Add(new MailboxAddress(UserName, _tconfig.Email));
            message.To.AddRange(address);
            using (var client = new SmtpClient())
            {

                await client.ConnectAsync(_tconfig.Server, _tconfig.Port, UseSsl);
                //await client.AuthenticateAsync(UserAddress, Password);
                await client.AuthenticateAsync(_tconfig.Email, _tconfig.Auth);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="content">邮件内容主题</param>
        /// <param name="fromAddress">发送方信息</param>
        /// <param name="toAddress">接收方信息</param>
        /// <param name="textFormat">内容主题模式，默认TextFormat.Text</param>
        /// <param name="attachments">附件</param>
        /// <param name="dispose">是否自动释放附件所用Stream</param>
        /// <returns></returns>
        public static async Task SendEMailAsync(string subject, string content, MailboxAddress fromAddress, IEnumerable<MailboxAddress> toAddress, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null, bool dispose = true)
        {
            await SendEMailAsync(subject, content, new MailboxAddress[] { fromAddress }, toAddress, textFormat, attachments, dispose).ConfigureAwait(false);
        }
        
        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="content">邮件内容主题</param>
        /// <param name="fromAddress">发送方信息</param>
        /// <param name="toAddress">接收方信息</param>
        /// <param name="textFormat">内容主题模式，默认TextFormat.Text</param>
        /// <param name="attachments">附件</param>
        /// <param name="dispose">是否自动释放附件所用Stream</param>
        /// <returns></returns>
        public static async Task<bool> SendEMailAsync(string subject, string content, IEnumerable<MailboxAddress> fromAddress, IEnumerable<MailboxAddress> toAddress, TextFormat textFormat = TextFormat.Html, IEnumerable<AttachmentInfo>? attachments = null, bool dispose = true)
        {
            try
            {
                var message = new MimeMessage();
                message.From.AddRange(fromAddress);
                message.To.AddRange(toAddress);
                message.Subject = subject;
                message.Date = DateTime.Now;
                message.Priority = MessagePriority.Urgent;
                message.Body = new BodyBuilder
                {
                    
                    HtmlBody = content
                }.ToMessageBody();
                //var body = new TextPart(textFormat)
                //{                
                //    Text = content,                
                //};

                MimeEntity entity = message.Body;
                if (attachments != null)
                {
                    var mult = new Multipart("mixed")
                {
                    message.Body
                };
                    foreach (var att in attachments)
                    {
                        var attachment = string.IsNullOrWhiteSpace(att.ContentType) ? new MimePart() : new MimePart(att.ContentType);

                        if (att.Stream != null && att.Stream.CanRead)
                        {
                            attachment.Content = new MimeContent(att.Stream);
                        }
                        else if (att.Data != null)
                        {
                            Stream stream = new MemoryStream(att.Data);
                            stream.Seek(0, SeekOrigin.Begin);
                            attachment.Content = new MimeContent(stream);
                            stream.Close();
                            stream.Dispose();
                        }
                        attachment.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                        attachment.ContentTransferEncoding = att.ContentTransferEncoding;
                        attachment.FileName = att.FileName;//解决附件中文名问题

                        mult.Add(attachment);
                    }
                    entity = mult;
                }
                message.Body = entity;
                message.Date = DateTime.Now;
                using (var client = new SmtpClient())
                {
                    //if (Host == "mail.spacechina.com")
                    if (_tconfig.Server == "mail.spacechina.com")
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        //await client.ConnectAsync(Host);
                        await client.ConnectAsync(_tconfig.Server, _tconfig.Port, false);
                    }
                    else
                    {
                        //创建连接
                        //await client.ConnectAsync(Host, Port, UseSsl);
                        await client.ConnectAsync(_tconfig.Server, _tconfig.Port, UseSsl);
                    }
                    //await client.AuthenticateAsync(UserAddress, Password);
                    await client.AuthenticateAsync(_tconfig.Email, _tconfig.Auth);

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    if (dispose && attachments != null)
                    {
                        foreach (var att in attachments)
                        {
                            att.Dispose();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                if (toAddress.Any())
                {
                    string[] errs = toAddress.Select(u => u.Address).ToArray();
                    await RedisHelper.SetAsync("emailerror", $"邮件发送失败{ex.Message}，发信邮箱：{_tconfig.Email}，收信邮箱：{string.Join(',', errs)}");
                }
                Logger.Error($"邮件发送失败，{ex.Message},\r\n{ex.StackTrace},\r\n 发信配置：{JsonHelper.JsonSerialize(_tconfig)}");

                return false;
            }
            return true;
        }
        private static string ConvertToBase64(string inputStr, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(inputStr));
        }


        public static async Task<AttachmentInfo> GetAttachmentInfo(string file, string storageType)
        {
            try
            {
                if (file.StartsWith("http"))
                {
                    HttpClient hc = new HttpClient();
                    byte[] data = await hc.GetByteArrayAsync(file);
                    AttachmentInfo att = new AttachmentInfo()
                    {
                        Stream = null,
                        Data = data,
                        FileName = Path.GetFileName(file),
                        ContentType = System.Net.Mime.MediaTypeNames.Application.Octet
                    };
                    return att;

                }
                else if (storageType == "local")
                {
                    //file = Path.Combine(System.Environment.CurrentDirectory, "wwwroot", file.Replace(ConfigurationHelper.GetSectionValue("resourcehost"), ""));
                    file = $"{Environment.CurrentDirectory}\\wwwroot{file.Replace("/", "\\")}";

                    AttachmentInfo data = new AttachmentInfo()
                    {
                        Stream = null,
                        Data = File.ReadAllBytes(file),
                        FileName = Path.GetFileName(file),
                        ContentType = System.Net.Mime.MediaTypeNames.Application.Octet
                    };

                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error($"加载附件失败：{ex.Message}");
                return null;
            }
        }

    }

    /// <summary>
    /// 附件信息
    /// </summary>
    public class AttachmentInfo : IDisposable
    {
        /// <summary>
        /// 附件类型，比如application/pdf
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件传输编码方式，默认ContentEncoding.Default
        /// </summary>
        public ContentEncoding ContentTransferEncoding { get; set; } = ContentEncoding.Default;
        /// <summary>
        /// 文件数组
        /// </summary>
        public byte[] Data { get; set; }
        private Stream stream;
        /// <summary>
        /// 文件数据流，获取数据时优先采用此部分
        /// </summary>
        public Stream Stream
        {
            get
            {
                if (this.stream == null && this.Data != null)
                {
                    stream = new MemoryStream(this.Data);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                return this.stream;
            }
            set { this.stream = value; }
        }
        /// <summary>
        /// 释放Stream
        /// </summary>
        public void Dispose()
        {
            if (stream != null || stream.CanRead)
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }

    public class EmailConfig
    {
        public string Email { get; set; }

        public string Auth { get; set; }

        public string Server { get; set; }

        public int Port { get; set; }
    }
}
