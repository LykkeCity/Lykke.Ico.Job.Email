namespace Lykke.Job.IcoEmailSender.Core.Settings.JobSettings
{
    public class IcoEmailSenderSettings
    {
        public DbSettings Db { get; set; }
        public AzureQueueSettings AzureQueue { get; set; }
        public SmtpSettings Smtp { get; set; }
        public string ContentUrl { get; set; }
    }
}
