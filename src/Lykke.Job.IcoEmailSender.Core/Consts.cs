namespace Lykke.Job.IcoEmailSender.Core
{
    public class Consts
    {
        public class Emails
        {
            public class Subjects
            {
                public const string InvestorConfirmation = "Email Confirmation - VALID ICO";
                public const string InvestorSummary = "Summary - VALID ICO";
                public const string InvestorNewTransaction = "New Transaction - VALID ICO";
            }

            public class BodyTemplates
            {
                public const string InvestorConfirmation = "investor-confirmation";
                public const string InvestorSummary = "investor-summary";
                public const string InvestorNewTransaction = "investor-new-transaction";
            }
        }
    }
}
