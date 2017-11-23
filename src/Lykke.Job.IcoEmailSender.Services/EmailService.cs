using Lykke.Job.IcoEmailSender.Core.Services;
using System;
using Common.Log;
using System.Threading.Tasks;
using Lykke.Ico.Core;
using System.Net;
using Lykke.Ico.Core.Queues.Emails;
using Lykke.Ico.Core.Helpers;
using System.Collections.Generic;

namespace Lykke.Job.IcoEmailSender.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILog _log;
        private readonly ISmtpService _smtpService;
        private readonly string _contentUrl;
        private readonly string _icoSiteUrl;
        private readonly string _bodyInvestorConfirmation;
        private readonly string _bodyInvestorSummary;
        private readonly string _bodyInvestorSummaryRefundBtcSection;
        private readonly string _bodyInvestorSummaryRefundEthSection;
        private readonly string _bodyInvestorKycRequest;
        private readonly string _bodyInvestorNewTransaction;

        public EmailService(ILog log, ISmtpService smtpService, string contentUrl, string icoSiteUrl)
        {
            _log = log;
            _smtpService = smtpService;
            _contentUrl = contentUrl;
            _icoSiteUrl = icoSiteUrl;

            _bodyInvestorConfirmation = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorConfirmation);
            _bodyInvestorSummary = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorSummary);
            _bodyInvestorSummaryRefundBtcSection = GetEmailBodyTemplate("investor-summary-refund-btc-section.html");
            _bodyInvestorSummaryRefundEthSection = GetEmailBodyTemplate("investor-summary-refund-eth-section.html");
            //_bodyInvestorKycRequest = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorKycRequest);
            _bodyInvestorNewTransaction = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorNewTransaction);
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
            var body = _bodyInvestorConfirmation
                .Replace("{ConfirmationLink}", $"{_icoSiteUrl}register/{message.ConfirmationToken}");

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorConfirmation, body);
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

            var body = _bodyInvestorSummary
                .Replace("{PayInBtcAddress}", message.PayInBtcAddress)
                .Replace("{PayInEthAddress}", message.PayInEthAddress)
                .Replace("{RefundBtcSection}", bodyInvestorSummaryRefundBtcSection)
                .Replace("{RefundBtcAddress}", message.RefundBtcAddress)
                .Replace("{RefundEthSection}", bodyInvestorSummaryRefundEthSection)
                .Replace("{RefundEthAddress}", message.RefundEthAddress)
                .Replace("{TokenAddress}", message.TokenAddress);

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorSummary, body, attachments);
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
                .Replace("{TransactionLink}", message.TransactionLink)
                .Replace("{Payment}", message.Payment);

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorNewTransaction, body);
        }
    }
}
