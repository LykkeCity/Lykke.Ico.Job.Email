using Lykke.Job.IcoEmailSender.Core.Services;
using System;
using Common.Log;
using System.Threading.Tasks;
using Lykke.Ico.Core;
using System.Net;
using Lykke.Ico.Core.Queues.Emails;
using Lykke.Ico.Core.Helpers;
using System.Collections.Generic;
using Lykke.Ico.Core.Repositories.EmailHistory;

namespace Lykke.Job.IcoEmailSender.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILog _log;
        private readonly ISmtpService _smtpService;
        private readonly IEmailHistoryRepository _emailHistoryRepository;
        private readonly int _maxAttempts = 3;
        private readonly string _contentUrl;
        private readonly string _icoSiteUrl;
        private readonly string _bodyInvestorConfirmation;
        private readonly string _bodyInvestorSummary;
        private readonly string _bodyInvestorSummaryRefundBtcSection;
        private readonly string _bodyInvestorSummaryRefundEthSection;
        private readonly string _bodyInvestorKycRequest;
        private readonly string _bodyInvestorNewTransaction;

        public EmailService(ILog log, ISmtpService smtpService, IEmailHistoryRepository emailHistoryRepository,
            string contentUrl, string icoSiteUrl)
        {
            _log = log;
            _smtpService = smtpService;
            _emailHistoryRepository = emailHistoryRepository;
            _contentUrl = contentUrl;
            _icoSiteUrl = icoSiteUrl;

            _bodyInvestorConfirmation = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorConfirmation);
            _bodyInvestorSummary = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorSummary);
            _bodyInvestorSummaryRefundBtcSection = GetEmailBodyTemplate("investor-summary-refund-btc-section.html");
            _bodyInvestorSummaryRefundEthSection = GetEmailBodyTemplate("investor-summary-refund-eth-section.html");
            //_bodyInvestorKycRequest = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorKycRequest);
            //_bodyInvestorNewTransaction = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorNewTransaction);
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
            var body = _bodyInvestorConfirmation
                .Replace("{ConfirmationLink}", $"{_icoSiteUrl}register/{message.ConfirmationToken}");

            await SendInvestorEmail(message, subject, body);
        }

        public async Task SendEmail(InvestorSummaryMessage message)
        {
            var attachments = new Dictionary<string, byte[]>();
            attachments.Add("PayInBtcAddressQRCode.png", QRCodeHelper.GenerateQRPng(message.PayInBtcAddress));
            attachments.Add("PayInEthAddressQRCode.png", QRCodeHelper.GenerateQRPng(message.PayInBtcAddress));

            var bodyInvestorSummaryRefundBtcSection = _bodyInvestorSummaryRefundBtcSection;
            if (string.IsNullOrEmpty(message.RefundBtcAddress))
            {
                bodyInvestorSummaryRefundBtcSection = "";
            }

            var bodyInvestorSummaryRefundEthSection = _bodyInvestorSummaryRefundEthSection;
            if (string.IsNullOrEmpty(message.RefundEthAddress))
            {
                bodyInvestorSummaryRefundEthSection = "";
            }

            var subject = Consts.Emails.Subjects.InvestorSummary;

            var body = _bodyInvestorSummary
                .Replace("{PayInBtcAddress}", message.PayInBtcAddress)
                .Replace("{PayInEthAddress}", message.PayInEthAddress)
                .Replace("{RefundBtcSection}", bodyInvestorSummaryRefundBtcSection)
                .Replace("{RefundBtcAddress}", message.RefundBtcAddress)
                .Replace("{RefundEthSection}", bodyInvestorSummaryRefundEthSection)
                .Replace("{RefundEthAddress}", message.RefundEthAddress)
                .Replace("{TokenAddress}", message.TokenAddress);

            await SendInvestorEmail(message, subject, body, attachments);
        }

        public async Task SendEmail(InvestorKycRequestMessage message)
        {
            var body = _bodyInvestorKycRequest
                .Replace("{KycId}", message.KycId);

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorKycRequest, body);
        }

        public async Task SendEmail(InvestorNewTransactionMessage message)
        {
            var body = _bodyInvestorNewTransaction
                .Replace("{KycId}", message.Amount)
                .Replace("{CurrencyType}", Enum.GetName(typeof(CurrencyType), message.CurrencyType))
                .Replace("{Amount}", message.Amount);

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorNewTransaction, body);
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

            await _emailHistoryRepository.SaveAsync(typeName, message.EmailTo, subject, body);
        }
    }
}
