using Autofac;
using Common.Log;
using Lykke.Job.IcoEmailSender.Core.Services;
using Lykke.Job.IcoEmailSender.Core.Settings.JobSettings;
using Lykke.Job.IcoEmailSender.Services;
using Lykke.SettingsReader;
using Lykke.JobTriggers.Extenstions;

namespace Lykke.Job.IcoEmailSender.Modules
{
    public class JobModule : Module
    {
        private readonly IcoEmailSenderSettings _settings;
        private readonly IReloadingManager<DbSettings> _dbSettingsManager;
        private readonly ILog _log;

        public JobModule(IcoEmailSenderSettings settings, IReloadingManager<DbSettings> dbSettingsManager, ILog log)
        {
            _settings = settings;
            _log = log;
            _dbSettingsManager = dbSettingsManager;
        }

        protected override void Load(ContainerBuilder builder)
        {
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

            builder.RegisterType<SmtpService>()
                .As<ISmtpService>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.Smtp));

            builder.RegisterType<EmailService>()
                .As<IEmailService>()
                .SingleInstance()
                .WithParameter("contentUrl", _settings.ContentUrl)
                .WithParameter("icoSiteUrl", _settings.IcoSiteUrl);
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
