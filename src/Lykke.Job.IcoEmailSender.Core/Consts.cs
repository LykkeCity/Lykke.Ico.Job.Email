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
                public const string InvestorReferralCode = "Invite a Friend - VALID ITO";
                public const string Investor20MFix = "20M Crowd Sale Discount - VALID ITO";
            }

            public class BodyTemplates
            {
                public const string InvestorConfirmation = "investor-confirmation";
                public const string InvestorSummary = "investor-summary";
                public const string InvestorNewTransaction = "investor-new-transaction";
                public const string InvestorKycReminder = "investor-kyc-reminder";
                public const string InvestorReferralCode = "investor-referral-code";
                public const string Investor20MFix = "investor-20m-fix";
            }
        }
    }
}
