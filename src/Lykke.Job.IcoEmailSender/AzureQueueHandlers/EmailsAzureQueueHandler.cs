using System.Threading.Tasks;
using Lykke.Ico.Core;
using Lykke.Ico.Core.Queues.Emails;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Job.IcoEmailSender.Core.Services;
using Common;
using Common.Log;

namespace Lykke.Job.IcoEmailSender.AzureQueueHandlers
{
    public class EmailsAzureQueueHandler
    {
        private readonly ILog _log;
        private readonly IEmailService _emailService;

        public EmailsAzureQueueHandler(ILog log, IEmailService emailService)
        {
            _log = log;
            _emailService = emailService;
        }

        [QueueTrigger(Consts.Emails.Queues.InvestorConfirmation, 30000)]
        public async Task HandleEmailMessage(InvestorConfirmationMessage message)
        {
            await _log.WriteInfoAsync(nameof(HandleEmailMessage),
                $"Type: {nameof(InvestorConfirmationMessage)}, Message: {message.ToJson()}",
                $"New message");

            await _emailService.SendEmail(message);
        }

        [QueueTrigger(Consts.Emails.Queues.InvestorSummary, 30000)]
        public async Task HandleEmailMessage(InvestorSummaryMessage message)
        {
            await _log.WriteInfoAsync(nameof(HandleEmailMessage),
                $"Type: {nameof(InvestorSummaryMessage)}, Message: {message.ToJson()}",
                $"New message");

            await _emailService.SendEmail(message);
        }

        [QueueTrigger(Consts.Emails.Queues.InvestorNewTransaction, 30000)]
        public async Task HandleEmailMessage(InvestorNewTransactionMessage message)
        {
            await _log.WriteInfoAsync(nameof(HandleEmailMessage),
                $"Type: {nameof(InvestorNewTransactionMessage)}, Message: {message.ToJson()}",
                $"New message");

            await _emailService.SendEmail(message);
        }

        [QueueTrigger(Consts.Emails.Queues.InvestorKycReminder, 30000)]
        public async Task HandleEmailMessage(InvestorKycReminderMessage message)
        {
            await _log.WriteInfoAsync(nameof(HandleEmailMessage),
                $"Type: {nameof(InvestorKycReminderMessage)}, Message: {message.ToJson()}",
                $"New message");

            await _emailService.SendEmail(message);
        }

        [QueueTrigger(Consts.Emails.Queues.InvestorReferralCode, 30000)]
        public async Task HandleEmailMessage(InvestorReferralCodeMessage message)
        {
            await _log.WriteInfoAsync(nameof(HandleEmailMessage),
                $"Type: {nameof(InvestorReferralCodeMessage)}, Message: {message.ToJson()}",
                $"New message");

            await _emailService.SendEmail(message);
        }
    }
}
