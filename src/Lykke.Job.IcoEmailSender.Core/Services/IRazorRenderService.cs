using System;
using System.Threading.Tasks;

namespace Lykke.Job.IcoEmailSender.Core.Services
{
    public interface IRazorRenderService
    {
        Task<String> Render<T>(string templateKey, T model);
    }
}
