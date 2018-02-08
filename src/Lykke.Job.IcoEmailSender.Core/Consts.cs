namespace Lykke.Job.IcoEmailSender.Core
{
    public class Consts
    {
        public class Emails
        {
            public class Subjects
            {
                public const string InvestorConfirmation = "Email Confirmation - VALID ITO";
                public const string InvestorSummary = "Summary - VALID ITO";
                public const string InvestorNewTransaction = "New Transaction - VALID ITO";
                public const string InvestorKycReminder = "KYC Reminder - VALID ITO";
            }

            public class BodyTemplates
            {
                public const string InvestorConfirmation = "investor-confirmation";
                public const string InvestorSummary = "investor-summary";
                public const string InvestorNewTransaction = "investor-new-transaction";
                public const string InvestorKycReminder = "investor-kyc-reminder";
            }
        }
    }
}
