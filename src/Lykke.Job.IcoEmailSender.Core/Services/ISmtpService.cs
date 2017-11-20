using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.IcoEmailSender.Core.Services
{
    public interface ISmtpService
    {
        Task Send(string to, string subject, string body, Dictionary<string, byte[]> attachments = null);
    }
}
