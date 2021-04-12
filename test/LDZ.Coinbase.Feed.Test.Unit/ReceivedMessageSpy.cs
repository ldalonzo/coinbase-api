using System.Collections.Generic;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model.Feed;
using Xunit.Abstractions;

namespace LDZ.Coinbase.Feed.Test.Unit
{
    public class ReceivedMessageSpy
    {
        private readonly List<FeedResponseMessage> _receivedMessages = new List<FeedResponseMessage>();
        private readonly ITestOutputHelper _testOutputHelper;

        public ReceivedMessageSpy(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public IEnumerable<FeedResponseMessage> ReceivedMessages => _receivedMessages;

        public void ReceiveMessage<T>(T message)
            where T : FeedResponseMessage
        {
            _receivedMessages.Add(message);
            _testOutputHelper.WriteLine($"Received {message}");
        }
    }
}
