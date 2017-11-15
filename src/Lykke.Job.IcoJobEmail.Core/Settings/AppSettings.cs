using Lykke.Job.IcoJobEmail.Core.Settings.JobSettings;
using Lykke.Job.IcoJobEmail.Core.Settings.SlackNotifications;

namespace Lykke.Job.IcoJobEmail.Core.Settings
{
    public class AppSettings
    {
        public IcoJobEmailSettings IcoJobEmailJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}