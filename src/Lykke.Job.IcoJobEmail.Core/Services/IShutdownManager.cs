using System.Threading.Tasks;

namespace Lykke.Job.IcoJobEmail.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}