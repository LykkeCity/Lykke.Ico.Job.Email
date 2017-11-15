using Lykke.Job.IcoJobEmail.Core.Services;
using System;
using Lykke.Ico.Core.Contracts.Emails;
using Common.Log;
using System.Threading.Tasks;
using Lykke.Ico.Core;
using Lykke.Ico.Core.Contracts;
using System.Net;

namespace Lykke.Job.IcoJobEmail.Services
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
        private readonly string _bodyAdminNewTransaction;


        private readonly string _templatesUrl = "";

        public EmailService(ILog log, ISmtpService smtpService, string contentUrl, string icoSiteUrl)
        {
            _log = log;
            _smtpService = smtpService;
            _contentUrl = contentUrl;
            _icoSiteUrl = icoSiteUrl;

            _bodyInvestorConfirmation = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorConfirmation);
            //_bodyInvestorSummary = GetEmailBodyTemplate(Consts.Emails.BodyTemplates.InvestorSummary);
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

        public async Task SendEmail(InvestorConfirmation message)
        {
            var body = _bodyInvestorConfirmation
                .Replace("{ConfirmationLink}", $"{_icoSiteUrl}register/{message.ConfirmationToken}");

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorConfirmation, body);
        }

        public async void SendEmail(InvestorSummary message)
        {
            var body = _bodyInvestorConfirmation
                .Replace("{BtcAddress}", message.BtcAddress)
                .Replace("{EthAddress}", message.EthAddress);

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorSummary, body);
        }

        public async void SendEmail(InvestorKycRequest message)
        {
            var body = _bodyInvestorConfirmation
                .Replace("{KycId}", message.KycId);

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorKycRequest, body);
        }

        public async void SendEmail(InvestorNewTransaction message)
        {
            var body = _bodyInvestorConfirmation
                .Replace("{KycId}", message.Amount)
                .Replace("{CurrencyType}", Enum.GetName(typeof(CurrencyType), message.CurrencyType))
                .Replace("{TokensAmount}", message.TokensAmount);

            await _smtpService.Send(message.EmailTo, Consts.Emails.Subjects.InvestorNewTransaction, body);
        }
    }
}
