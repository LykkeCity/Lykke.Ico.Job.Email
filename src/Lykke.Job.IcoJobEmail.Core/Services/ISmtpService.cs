using System.Threading.Tasks;

namespace Lykke.Job.IcoJobEmail.Core.Services
{
    public interface ISmtpService
    {
        Task Send(string to, string subject, string body);
    }
}
