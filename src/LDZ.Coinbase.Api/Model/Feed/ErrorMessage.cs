using System.Text;

namespace LDZ.Coinbase.Api.Model.Feed
{
    public class ErrorMessage : FeedResponseMessage
    {
        public string? Message { get; set; }

        public string? Reason { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder("ERR");

            if (Message != null)
            {
                sb.Append($" {Message}");
            }

            if (Reason != null)
            {
                sb.Append($" {Reason}");
            }

            return sb.ToString();
        }
    }
}