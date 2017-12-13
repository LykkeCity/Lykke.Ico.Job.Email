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

        [QueueTrigger(Consts.Emails.Queues.InvestorConfirmation)]
        public async Task HandleEmailMessage(InvestorConfirmationMessage message)
        {
            await _log.WriteInfoAsync(nameof(EmailsAzureQueueHandler), nameof(HandleEmailMessage), nameof(InvestorConfirmationMessage),
                $"Message: {message.ToJson()}");

            await _emailService.SendEmail(message);
        }

        [QueueTrigger(Consts.Emails.Queues.InvestorSummary)]
        public async Task HandleEmailMessage(InvestorSummaryMessage message)
        {
            await _log.WriteInfoAsync(nameof(EmailsAzureQueueHandler), nameof(HandleEmailMessage), nameof(InvestorSummaryMessage),
                $"Message: {message.ToJson()}");

            await _emailService.SendEmail(message);
        }

        [QueueTrigger(Consts.Emails.Queues.InvestorNewTransaction)]
        public async Task HandleEmailMessage(InvestorNewTransactionMessage message)
        {
            await _log.WriteInfoAsync(nameof(EmailsAzureQueueHandler), nameof(HandleEmailMessage), nameof(InvestorNewTransactionMessage),
                $"Message: {message.ToJson()}");

            await _emailService.SendEmail(message);
        }
    }
}
