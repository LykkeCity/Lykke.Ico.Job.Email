using System.IO;
using System.Threading.Tasks;
using Lykke.Job.IcoEmailSender.Core.Services;
using RazorLight;

namespace Lykke.Job.IcoEmailSender.Services
{
    public class RazorRenderService : IRazorRenderService
    {
        private readonly IRazorLightEngine _razorLightEngine;

        public RazorRenderService(IRazorLightEngine razorLightEngine)
        {
            _razorLightEngine = razorLightEngine;
        }

        public async Task<string> Render<T>(string key, T model)
        {
            // Notes for RazorLight 2.0-alpha3:
            //
            // Due to known bug (https://github.com/toddams/RazorLight/issues/97)
            // to be able to use ViewBag (i.e. in layout pages) 
            // workaround with using correct override should be used:
            //
            // using (var writer = new StringWriter())
            // {
            //     await _razorLightEngine.RenderTemplateAsync(
            //         await _razorLightEngine.CompileTemplateAsync(key),
            //         model,
            //         model.GetType(),
            //         writer,
            //         viewBag);
            //
            //     return writer.ToString();
            // }
            //
            // Also expiration tokens (clearing cache on file change) 
            // had been implemented later than 2.0-alpha3, 
            // so this case does not work for now.

            return await _razorLightEngine.CompileRenderAsync(Path.ChangeExtension(key, ".cshtml"), model);
        }
    }
}
