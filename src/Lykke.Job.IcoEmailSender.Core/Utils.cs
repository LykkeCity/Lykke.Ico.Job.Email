namespace Lykke.Job.IcoEmailSender.Core
{
    public class Utils
    {
        public static string RemoveSection(string body, string section)
        {
            var start = $"<!--{section}-->";
            var end = $"<!--end:{section}-->";

            var index = body.IndexOf(start);
            var count = (body.IndexOf(end) + end.Length) - index;

            return body.Remove(index, count);
        }
    }
}
