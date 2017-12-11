using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.IcoEmailSender.Core.Services
{
    public interface IViewRenderService
    {
        Task<String> Render<T>(string templateKey, T model);
    }
}
