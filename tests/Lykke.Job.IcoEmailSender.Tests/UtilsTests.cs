using Lykke.Job.IcoEmailSender.Core;
using Xunit;

namespace Lykke.Job.IcoEmailSender.Tests
{
    public class UtilsTests
    {
        [Fact]
        public void RemoveSection_Test()
        {
            // asset
            var body = @"</tr>
                         <!--TransactionLink-->
                            some content
                         <!--end:TransactionLink-->
                         <tr>";
            var section = "TransactionLink";

            // act
            var result = Utils.RemoveSection(body, section);

            // assert
            Assert.DoesNotContain("some content", result);
        }
    }
}
