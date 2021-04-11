using System.Collections.Generic;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model.Feed;

namespace LDZ.Coinbase.Feed.Test.Unit
{
    public class ReceivedMessageSpy
    {
        private readonly List<FeedResponseMessage> _receivedMessages = new List<FeedResponseMessage>();

        public IEnumerable<FeedResponseMessage> ReceivedMessages => _receivedMessages;

        public void ReceiveMessage<T>(T message)
            where T : FeedResponseMessage
        {
            _receivedMessages.Add(message);
        }
    }
}
