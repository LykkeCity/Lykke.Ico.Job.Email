using System.Threading.Tasks;

namespace Lykke.Job.IcoEmailSender.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}