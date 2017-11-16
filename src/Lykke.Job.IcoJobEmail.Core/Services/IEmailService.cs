using Lykke.Ico.Core.Contracts.Queues;
using System.Threading.Tasks;

namespace Lykke.Job.IcoJobEmail.Core.Services
{
    public interface IEmailService
    {
        Task SendEmail(InvestorConfirmationMessage message);
        void SendEmail(InvestorSummaryMessage message);
        void SendEmail(InvestorKycRequestMessage message);
        void SendEmail(InvestorNewTransactionMessage message);
    }
}
