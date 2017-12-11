using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Ico.Core;
using Lykke.Ico.Core.Queues.Emails;
using Lykke.Ico.Core.Helpers;
using Lykke.Ico.Core.Repositories.InvestorEmail;
using Lykke.Job.IcoEmailSender.Core.Services;
using Lykke.Job.IcoEmailSender.Core;

namespace Lykke.Job.IcoEmailSender.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILog _log;
        private readonly ISmtpService _smtpService;
        private readonly IInvestorEmailRepository _investorEmailRepository;
        private readonly IViewRenderService _viewRenderService;
        private readonly string _contentUrl;
        private readonly string _bodyInvestorConfirmation;
        private readonly string _bodyInvestorSummary;
        private readonly string _bodyInvestorKycRequest;
        private readonly string _bodyInvestorNewTransaction;
        private readonly string _bodyInvestorNeedMoreInvestment;

        public EmailService(ILog log, ISmtpService smtpService, IInvestorEmailRepository 
            investorEmailRepository, string contentUrl, IViewRenderService viewRenderService)
        {
            _log = log;
            _smtpService = smtpService;
            _investorEmailRepository = investorEmailRepository;
            _viewRenderService = viewRenderService;
            _contentUrl = contentUrl;

            _bodyInvestorConfirmation = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorConfirmation);
            _bodyInvestorSummary = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorSummary);
            _bodyInvestorKycRequest = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorKycRequest);
            _bodyInvestorNewTransaction = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorNewTransaction);
            _bodyInvestorNeedMoreInvestment = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorNeedMoreInvestment);
        }

        private string GetEmailBodyTemplate(string templateFileName)
        {
            var url = $"{_contentUrl}{templateFileName}";

            _log.WriteInfoAsync(nameof(EmailService), nameof(GetEmailBodyTemplate), $"Get content from {url}").Wait();

            using (var client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }

        public async Task SendEmail(InvestorConfirmationMessage message)
        {
            var subject = Consts.Emails.Subjects.InvestorConfirmation;

            var body = await _viewRenderService.Render(Consts.Emails.BodyTemplates.InvestorConfirmation.Replace(".html", ""), message);

            await SendInvestorEmail(message, subject, body);
        }

        public async Task SendEmail(InvestorSummaryMessage message)
        {
            var attachments = new Dictionary<string, byte[]>
            {
                { "PayInBtcAddressQRCode.png", QRCodeHelper.GenerateQRPng(message.PayInBtcAddress) },
                { "PayInEthAddressQRCode.png", QRCodeHelper.GenerateQRPng(message.PayInBtcAddress) }
            };

            var body = _bodyInvestorSummary
                .Replace("{LinkBtcAddress}", message.LinkBtcAddress)
                .Replace("{LinkEthAddress}", message.LinkEthAddress)
                .Replace("{PayInBtcAddress}", message.PayInBtcAddress)
                .Replace("{PayInEthAddress}", message.PayInEthAddress)
                .Replace("{RefundBtcAddress}", message.RefundBtcAddress)
                .Replace("{RefundEthAddress}", message.RefundEthAddress)
                .Replace("{TokenAddress}", message.TokenAddress);

            if (string.IsNullOrEmpty(message.RefundBtcAddress))
            {
                body = await RemoveSection(body, "RefundBtcAddress");
            }
            if (string.IsNullOrEmpty(message.RefundEthAddress))
            {
                body = await RemoveSection(body, "RefundBtcAddress");
            }

            await SendInvestorEmail(message, Consts.Emails.Subjects.InvestorSummary, body, attachments);
        }

        public async Task SendEmail(InvestorNewTransactionMessage message)
        {
            var body = _bodyInvestorNewTransaction
                .Replace("{TransactionLink}", message.TransactionLink)
                .Replace("{Payment}", message.Payment);

            if (string.IsNullOrEmpty(message.TransactionLink))
            {
                body = await RemoveSection(body, "TransactionLink");
            }

            await SendInvestorEmail(message, Consts.Emails.Subjects.InvestorNewTransaction, body);
        }

        public async Task SendEmail(InvestorNeedMoreInvestmentMessage message)
        {
            var body = _bodyInvestorNeedMoreInvestment
                .Replace("{InvestedAmount}", message.InvestedAmount.ToString())
                .Replace("{MinAmount}", message.MinAmount.ToString());

            await SendInvestorEmail(message, Consts.Emails.Subjects.InvestorNeedMoreInvestment, body);
        }

        public async Task SendEmail(InvestorKycRequestMessage message)
        {
            var body = _bodyInvestorKycRequest
                .Replace("{KycLink}", message.KycLink);

            await SendInvestorEmail(message, Consts.Emails.Subjects.InvestorKycRequest, body);
        }

        private async Task SendInvestorEmail<T>(T message, string subject, string body, Dictionary<string, byte[]> attachments = null)
            where T : IInvestorMessage
        {
            var typeName = message.GetType().Name;

            try
            {
                await _smtpService.Send(message.EmailTo, subject, body, attachments);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(
                        nameof(EmailService),
                        nameof(SendEmail),
                        $"Failed to send {typeName}: '{message.ToString()}'",
                        ex);

                throw ex;
            }

            await _investorEmailRepository.SaveAsync(typeName, message.EmailTo, subject, body);
        }

        private async Task<string> RemoveSection(string body, string section)
        {
            try
            {
                return Utils.RemoveSection(body, section);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(
                        nameof(EmailService),
                        nameof(RemoveSection),
                        $"Failed to remove section: '{section}'. {Environment.NewLine}Body: {Environment.NewLine}{body}",
                        ex);
            }

            return body;
        }
    }
}
