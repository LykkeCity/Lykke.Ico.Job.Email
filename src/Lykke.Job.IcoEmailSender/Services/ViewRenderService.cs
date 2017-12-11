using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lykke.Job.IcoEmailSender.Core.Services;
using RazorLight;

namespace Lykke.Job.IcoEmailSender.Services
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorLightEngine _razorLightEngine;

        public ViewRenderService(IRazorLightEngine razorLightEngine)
        {
            _razorLightEngine = razorLightEngine;
        }

        public async Task<string> Render<T>(string templateKey, T model)
        {
            try
            {
                return await _razorLightEngine.CompileRenderAsync(templateKey, model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
