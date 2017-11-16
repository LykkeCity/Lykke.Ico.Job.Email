﻿using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Ico.Core;
using Lykke.Job.IcoEmailSender.Core.Services;
using Common.Log;
using Lykke.Ico.Core.Contracts.Queues;

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
            await _log.WriteInfoAsync(nameof(EmailsAzureQueueHandler), nameof(HandleEmailMessage), $"Send InvestorConfirmation email to {message.EmailTo}");

            await _emailService.SendEmail(message);
        }
    }
}
