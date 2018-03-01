using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
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

        public EmailService(
            ILog log, 
            ISmtpService smtpService, 
            IInvestorEmailRepository investorEmailRepository,
            IRazorRenderService razorRenderService)
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

            await SendInvestorEmail(message, subject, body);
        }

        public async Task SendEmail(InvestorNewTransactionMessage message)
        {
            var subject = Consts.Emails.Subjects.InvestorNewTransaction;
            var body = await _razorRenderService.Render(Consts.Emails.BodyTemplates.InvestorNewTransaction, message);

            await SendInvestorEmail(message, subject, body);
        }

        public async Task SendEmail(InvestorKycReminderMessage message)
        {
            var subject = Consts.Emails.Subjects.InvestorKycReminder;
            var body = await _razorRenderService.Render(Consts.Emails.BodyTemplates.InvestorKycReminder, message);

            await SendInvestorEmail(message, subject, body);
        }

        public async Task SendEmail(InvestorReferralCodeMessage message)
        {
            var subject = Consts.Emails.Subjects.InvestorReferralCode;
            var body = await _razorRenderService.Render(Consts.Emails.BodyTemplates.InvestorReferralCode, message);

            await SendInvestorEmail(message, subject, body);
        }

        public async Task SendInvestorEmail<T>(T message, string subject, string body, Dictionary<string, byte[]> attachments = null)
            where T : IInvestorMessage
        {
            var typeName = message.GetType().Name;

            try
            {
                await _smtpService.Send(message.EmailTo, subject, body, attachments);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(SendEmail),
                    $"Type: {typeName}, Message: '{message.ToJson()}'",
                    ex);

                throw ex;
            }

            await _investorEmailRepository.SaveAsync(typeName, message.EmailTo, subject, body);
        }
    }
}
