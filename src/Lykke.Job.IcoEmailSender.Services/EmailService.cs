using Lykke.Job.IcoEmailSender.Core.Services;
using System;
using Common.Log;
using System.Threading.Tasks;
using Lykke.Ico.Core;
using System.Net;
using Lykke.Ico.Core.Queues.Emails;
using Lykke.Ico.Core.Helpers;

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
            var body = _bodyInvestorConfirmation
                .Replace("{ConfirmationLink}", $"{_icoSiteUrl}register/{message.ConfirmationToken}");

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorConfirmation, body);
        }

        public async Task SendEmail(InvestorSummaryMessage message)
        {
            var payInBtcAddressQRCode = Convert.ToBase64String(QRCodeHelper.GenerateQRPng(message.PayInBtcAddress));
            var payInEthAddressQRCode = Convert.ToBase64String(QRCodeHelper.GenerateQRPng(message.PayInBtcAddress));

            var body = _bodyInvestorConfirmation
                .Replace("{PayInBtcAddress}", message.PayInBtcAddress)
                .Replace("{PayInBtcAddressQRCode}", payInBtcAddressQRCode)
                .Replace("{PayInEthAddress}", message.PayInEthAddress)
                .Replace("{PayInEthAddressQRCode}", payInEthAddressQRCode)
                .Replace("{RefundBtcAddress}", message.RefundBtcAddress)
                .Replace("{RefundEthAddress}", message.RefundEthAddress)
                .Replace("{TokenAddress}", message.TokenAddress);

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorSummary, body);
        }

        public async Task SendEmail(InvestorKycRequestMessage message)
        {
            var body = _bodyInvestorConfirmation
                .Replace("{KycId}", message.KycId);

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorKycRequest, body);
        }

        public async Task SendEmail(InvestorNewTransactionMessage message)
        {
            var body = _bodyInvestorConfirmation
                .Replace("{KycId}", message.Amount)
                .Replace("{CurrencyType}", Enum.GetName(typeof(CurrencyType), message.CurrencyType))
                .Replace("{Amount}", message.Amount);

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorNewTransaction, body);
        }
    }
}
