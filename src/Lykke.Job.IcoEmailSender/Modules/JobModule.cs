using Autofac;
using Common.Log;
using Lykke.Job.IcoEmailSender.Core.Services;
using Lykke.Job.IcoEmailSender.Core.Settings.JobSettings;
using Lykke.Job.IcoEmailSender.Services;
using Lykke.SettingsReader;
using Lykke.JobTriggers.Extenstions;
using Lykke.Ico.Core.Repositories.InvestorEmail;
using System.IO;
using RazorLight;

namespace Lykke.Job.IcoEmailSender.Modules
{
    public class JobModule : Module
    {
        private readonly IcoEmailSenderSettings _settings;
        private readonly IReloadingManager<DbSettings> _dbSettingsManager;
        private readonly ILog _log;
        private readonly string _contentRootPath;

        public JobModule(IcoEmailSenderSettings settings, IReloadingManager<DbSettings> dbSettingsManager, ILog log, string contentRootPath)
        {
            _settings = settings;
            _log = log;
            _dbSettingsManager = dbSettingsManager;
            _contentRootPath = contentRootPath;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var connectionStringManager = _dbSettingsManager.ConnectionString(x => x.IcoDataConnString);

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

            RegisterAzureQueueHandlers(builder);

            builder.RegisterType<InvestorEmailRepository>()
                .As<IInvestorEmailRepository>()
                .WithParameter(TypedParameter.From(connectionStringManager))
                .SingleInstance();

            builder.RegisterType<SmtpService>()
                .As<ISmtpService>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.Smtp));

            builder.RegisterType<EmailService>()
                .As<IEmailService>()
                .SingleInstance()
                .WithParameter("contentUrl", _settings.ContentUrl);

            builder.RegisterInstance(new EngineFactory().ForFileSystem(Path.Combine(_contentRootPath, "Templates")))
                .As<IRazorLightEngine>();

            builder.RegisterType<ViewRenderService>()
                .As<IViewRenderService>()
                .SingleInstance();
        }

        private void RegisterAzureQueueHandlers(ContainerBuilder builder)
        {
            builder.AddTriggers(
                pool =>
                {
                    pool.AddDefaultConnection(_settings.AzureQueue.ConnectionString);
                });
        }
    }
}
