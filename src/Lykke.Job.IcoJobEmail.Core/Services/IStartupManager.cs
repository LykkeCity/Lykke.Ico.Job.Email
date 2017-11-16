using System.Threading.Tasks;

namespace Lykke.Job.IcoEmailSender.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}