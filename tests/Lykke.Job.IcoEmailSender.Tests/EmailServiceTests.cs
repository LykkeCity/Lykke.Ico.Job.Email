using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Ico.Core.Queues.Emails;
using Lykke.Ico.Core.Repositories.InvestorEmail;
using Lykke.Job.IcoEmailSender.Core.Services;
using Lykke.Job.IcoEmailSender.Services;
using Moq;
using Xunit;
using Lykke.Job.IcoEmailSender.Core;

namespace Lykke.Job.IcoEmailSender.Tests
{
    public class EmailServiceTests
    {
        private ILog _log;
        private Mock<ISmtpService> _smtpService;
        private Mock<IInvestorEmailRepository> _investorEmailRepository;
        private Mock<IRazorRenderService> _razorRenderService;

        public IEmailService Init()
        {
            _log = new LogToMemory();
            _smtpService = new Mock<ISmtpService>();
            _investorEmailRepository = new Mock<IInvestorEmailRepository>();
            _razorRenderService = new Mock<IRazorRenderService>();

            return new EmailService(
                _log,
                _smtpService.Object, 
                _investorEmailRepository.Object,
                _razorRenderService.Object);
        }

        [Fact]
        public async Task SendEmail_ShouldSendInvestorConfirmationMessage()
        {
            // Arrange
            var emailService = Init();
            var email = "test@test.test";
            _razorRenderService
                .Setup(x => x.Render(It.IsAny<string>(), It.IsAny<InvestorConfirmationMessage>()))
                .Returns((string k, InvestorConfirmationMessage m) => Task.FromResult(k));

            // Act
            await emailService.SendEmail(new InvestorConfirmationMessage { EmailTo = email });

            // Assert
            _smtpService.Verify(x => x.Send(
                It.Is<string>(e => e == email),
                It.Is<string>(s => s == Consts.Emails.Subjects.InvestorConfirmation),
                It.Is<string>(b => b == Consts.Emails.BodyTemplates.InvestorConfirmation),
                It.Is<Dictionary<string, byte[]>>(a => a == null)
            ));
        }

        [Fact]
        public async Task SendEmail_ShouldSendInvestorSummaryMessage()
        {
            // Arrange
            var emailService = Init();
            var email = "test@test.test";
            var payInBtcAddress = "testBtc";
            var payInEthAddress = "testEth";
            _razorRenderService
                .Setup(x => x.Render(It.IsAny<string>(), It.IsAny<InvestorSummaryMessage>()))
                .Returns((string k, InvestorSummaryMessage m) => Task.FromResult(k));

            // Act
            await emailService.SendEmail(new InvestorSummaryMessage
            {
                EmailTo = email,
                PayInBtcAddress = payInBtcAddress,
                PayInEthAddress = payInEthAddress
            });

            // Assert
            _smtpService.Verify(x => x.Send(
                It.Is<string>(e => e == email),
                It.Is<string>(s => s == Consts.Emails.Subjects.InvestorSummary),
                It.Is<string>(b => b == Consts.Emails.BodyTemplates.InvestorSummary),
                It.Is<Dictionary<string, byte[]>>(a => a.Count == 2)
            ));
        }

        [Fact]
        public async Task SendEmail_ShouldSendInvestorNewTransactionMessage()
        {
            // Arrange
            var emailService = Init();
            var email = "test@test.test";
            _razorRenderService
                .Setup(x => x.Render(It.IsAny<string>(), It.IsAny<InvestorNewTransactionMessage>()))
                .Returns((string k, InvestorNewTransactionMessage m) => Task.FromResult(k));

            // Act
            await emailService.SendEmail(new InvestorNewTransactionMessage { EmailTo = email });

            // Assert
            _smtpService.Verify(x => x.Send(
                It.Is<string>(e => e == email),
                It.Is<string>(s => s == Consts.Emails.Subjects.InvestorNewTransaction),
                It.Is<string>(b => b == Consts.Emails.BodyTemplates.InvestorNewTransaction),
                It.Is<Dictionary<string, byte[]>>(a => a == null)
            ));
        }
    }
}
