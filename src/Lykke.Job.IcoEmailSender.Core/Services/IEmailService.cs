using Lykke.Ico.Core.Queues.Emails;
using System.Threading.Tasks;

namespace Lykke.Job.IcoEmailSender.Core.Services
{
    public interface IEmailService
    {
        Task SendEmail(InvestorConfirmationMessage message);
        Task SendEmail(InvestorSummaryMessage message);
        Task SendEmail(InvestorNewTransactionMessage message);
        Task SendEmail(InvestorKycReminderMessage message);
        Task SendEmail(InvestorReferralCodeMessage message);
    }
}
