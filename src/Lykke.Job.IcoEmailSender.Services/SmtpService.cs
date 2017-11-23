using Common.Log;
using Lykke.Job.IcoEmailSender.Core.Services;
using Lykke.Job.IcoEmailSender.Core.Settings.JobSettings;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MimeKit.Utils;
using System;

namespace Lykke.Job.IcoEmailSender.Services
{
    public class SmtpService : ISmtpService
    {
        private readonly ILog _log;
        private readonly SmtpSettings _settings;

        public SmtpService(ILog log, SmtpSettings settings)
        {
            _log = log;
            _settings = settings;
        }

        public async Task Send(string to, string subject, string body, Dictionary<string, byte[]> attachments = null)
        {
            MimeEntity messageBody = null;

            if (attachments == null)
            {
                messageBody = new TextPart(TextFormat.Html) { Text = body };
            }
            else
            {
                var builder = new BodyBuilder
                {
                    HtmlBody = body
                };

                foreach (var item in attachments)
                {
                    var image = builder.LinkedResources.Add(item.Key, item.Value);
                    image.ContentId = MimeUtils.GenerateMessageId();
                    image.IsAttachment = false;

                    builder.HtmlBody = builder.HtmlBody.Replace($"{{{item.Key}}}", image.ContentId);
                }

                messageBody = builder.ToMessageBody();
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));
            emailMessage.To.Add(new MailboxAddress(string.Empty, to));
            emailMessage.Subject = subject;
            emailMessage.Body = messageBody;

            using (var client = new SmtpClient())
            {
                client.LocalDomain = _settings.LocalDomain;

                try
                {
                    await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.Auto);
                    await client.AuthenticateAsync(_settings.Login, _settings.Password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    await Task.Delay(500);
                    throw ex;
                }
            }
        }
    }
}
