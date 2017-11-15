using Lykke.Ico.Core.Contracts.Emails;
using System.Threading.Tasks;

namespace Lykke.Job.IcoJobEmail.Core.Services
{
    public interface IEmailService
    {
        Task SendEmail(InvestorConfirmation message);
        void SendEmail(InvestorSummary message);
        void SendEmail(InvestorKycRequest message);
        void SendEmail(InvestorNewTransaction message);
    }
}
