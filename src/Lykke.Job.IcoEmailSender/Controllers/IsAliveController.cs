using System;
using System.Linq;
using System.Net;
using Lykke.Job.IcoEmailSender.Core.Services;
using Lykke.Job.IcoEmailSender.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace Lykke.Job.IcoEmailSender.Controllers
{
    // NOTE: See https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/35755585/Add+your+app+to+Monitoring
    [Route("api/[controller]")]
    public class IsAliveController : Controller
    {
        private readonly IHealthService _healthService;
        private readonly IEmailService _emailService;

        public IsAliveController(IHealthService healthService, IEmailService emailService)
        {
            _healthService = healthService;
            _emailService = emailService;
        }

        /// <summary>
        /// Checks service is alive
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("IsAlive")]
        [ProducesResponseType(typeof(IsAliveResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public IActionResult Get()
        {
            var healthViloationMessage = _healthService.GetHealthViolationMessage();
            if (healthViloationMessage != null)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse
                {
                    ErrorMessage = $"Job is unhealthy: {healthViloationMessage}"
                });
            }

            // NOTE: Feel free to extend IsAliveResponse, to display job-specific indicators
            return Ok(new IsAliveResponse
            {
                Name = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationName,
                Version = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion,
                Env = Environment.GetEnvironmentVariable("ENV_INFO"),
#if DEBUG
                IsDebug = true,
#else
                IsDebug = false,
#endif
                IssueIndicators = _healthService.GetHealthIssues()
                    .Select(i => new IsAliveResponse.IssueIndicator
                    {
                        Type = i.Type,
                        Value = i.Value
                    })
            });
        }

        ///// <summary>
        ///// Checks service is alive
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("test")]
        //public IActionResult Test()
        //{
        //    _emailService.SendEmail(new Ico.Core.Queues.Emails.InvestorSummaryMessage
        //    {
        //        EmailTo = "akrivoshapov@gmail.com",
        //        PayInBtcAddress = "mmHTFBhVfQvSp19EanFF4kZsmc1y3VbX3R",
        //        PayInEthAddress = "0x9ab53D99c0AB522994dEe8f57c22FCC9007bF02A",
        //        TokenAddress = "0x84b9C6EAF93db2f7fbEFac2c07d49983d6c2d25E"
        //    });

        //    return Ok();
        //}
    }
}
