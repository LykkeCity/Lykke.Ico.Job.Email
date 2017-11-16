using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.IcoEmailSender.Core.Settings.JobSettings
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string LocalDomain { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string From { get; set; }
    }
}
