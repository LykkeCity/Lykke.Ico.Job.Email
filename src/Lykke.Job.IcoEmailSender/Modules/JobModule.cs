using System.IO;
using Autofac;
using Common.Log;
using Lykke.Ico.Core.Repositories.InvestorEmail;
using Lykke.Job.IcoEmailSender.Core.Services;
using Lykke.Job.IcoEmailSender.Core.Settings.JobSettings;
using Lykke.Job.IcoEmailSender.Services;
using Lykke.JobTriggers.Extenstions;
using Lykke.SettingsReader;
using RazorLight;

namespace Lykke.Job.IcoEmailSender.Modules
{
    public class JobModule : Module
    {
        private readonly IReloadingManager<IcoEmailSenderSettings> _settings;
        private readonly ILog _log;
        private readonly string _contentRootPath;

        public JobModule(IReloadingManager<IcoEmailSenderSettings> settings, 
            ILog log, 
            string contentRootPath)
        {
            _settings = settings;
            _log = log;
            _contentRootPath = contentRootPath;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var connectionStringManager = _settings.ConnectionString(x => x.Db.IcoDataConnString);

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.AddTriggers(
                pool =>
                {
                    pool.AddDefaultConnection(_settings.ConnectionString(x => x.AzureQueue.ConnectionString));
                });

            builder.RegisterType<InvestorEmailRepository>()
                .As<IInvestorEmailRepository>()
                .WithParameter(TypedParameter.From(connectionStringManager))
                .SingleInstance();

            builder.RegisterType<SmtpService>()
                .As<ISmtpService>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.Smtp));

            builder.RegisterType<EmailService>()
                .As<IEmailService>()
                .SingleInstance();

            builder.RegisterInstance(new EngineFactory().ForFileSystem(Path.Combine(_contentRootPath, "Templates")))
                .As<IRazorLightEngine>();

            builder.RegisterType<RazorRenderService>()
                .As<IRazorRenderService>()
                .SingleInstance();
        }
    }
}
