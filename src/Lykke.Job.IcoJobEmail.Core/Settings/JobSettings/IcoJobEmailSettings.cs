namespace Lykke.Job.IcoJobEmail.Core.Settings.JobSettings
{
    public class IcoJobEmailSettings
    {
        public DbSettings Db { get; set; }
        public AzureQueueSettings AzureQueue { get; set; }
        public SmtpSettings Smtp { get; set; }
        public string ContentUrl { get; set; }
        public string IcoSiteUrl { get; set; }
    }
}
