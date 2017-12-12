using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Ico.Core;
using Lykke.Ico.Core.Helpers;
using Lykke.Ico.Core.Queues.Emails;
using Lykke.Ico.Core.Repositories.InvestorEmail;
using Lykke.Job.IcoEmailSender.Core;
using Lykke.Job.IcoEmailSender.Core.Services;

namespace Lykke.Job.IcoEmailSender.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILog _log;
        private readonly ISmtpService _smtpService;
        private readonly IInvestorEmailRepository _investorEmailRepository;
        private readonly IRazorRenderService _razorRenderService;

        public EmailService(ILog log, ISmtpService smtpService, IInvestorEmailRepository 
            investorEmailRepository, IRazorRenderService razorRenderService)
        {
            _log = log;
            _smtpService = smtpService;
            _investorEmailRepository = investorEmailRepository;
            _razorRenderService = razorRenderService;
        }

        public async Task SendEmail(InvestorConfirmationMessage message)
        {
            var subject = Consts.Emails.Subjects.InvestorConfirmation;
            var body = await _razorRenderService.Render(Consts.Emails.BodyTemplates.InvestorConfirmation, message);

            await SendInvestorEmail(message, subject, body);
        }

        public async Task SendEmail(InvestorSummaryMessage message)
        {
            var subject = Consts.Emails.Subjects.InvestorSummary;
            var body = await _razorRenderService.Render(Consts.Emails.BodyTemplates.InvestorSummary, message);
            var attachments = new Dictionary<string, byte[]>
            {
                { "PayInBtcAddressQRCode.png", QRCodeHelper.GenerateQRPng(message.PayInBtcAddress) },
                { "PayInEthAddressQRCode.png", QRCodeHelper.GenerateQRPng(message.PayInBtcAddress) }
            };

            await SendInvestorEmail(message, subject, body, attachments);
        }

        public async Task SendEmail(InvestorNewTransactionMessage message)
        {
            var subject = Consts.Emails.Subjects.InvestorNewTransaction;
            var body = await _razorRenderService.Render(Consts.Emails.BodyTemplates.InvestorNewTransaction, message);

            await SendInvestorEmail(message, subject, body);
        }

        public async Task SendEmail(InvestorNeedMoreInvestmentMessage message)
        {
            var subject = Consts.Emails.Subjects.InvestorNeedMoreInvestment;
            var body = await _razorRenderService.Render(Consts.Emails.BodyTemplates.InvestorNeedMoreInvestment, message);

            await SendInvestorEmail(message, subject, body);
        }

        public async Task SendEmail(InvestorKycRequestMessage message)
        {
            var subject = Consts.Emails.Subjects.InvestorKycRequest;
            var body = await _razorRenderService.Render(Consts.Emails.BodyTemplates.InvestorKycRequest, message);

            await SendInvestorEmail(message, subject, body);
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
    }
}
