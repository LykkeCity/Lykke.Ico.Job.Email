using Lykke.Job.IcoEmailSender.Core.Settings.JobSettings;
using Lykke.Job.IcoEmailSender.Core.Settings.SlackNotifications;

namespace Lykke.Job.IcoEmailSender.Core.Settings
{
    public class AppSettings
    {
        public IcoEmailSenderSettings IcoEmailSenderJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}