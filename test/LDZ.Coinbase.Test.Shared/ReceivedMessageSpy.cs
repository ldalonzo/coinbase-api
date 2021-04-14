using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.Model.Feed;
using Xunit.Abstractions;

namespace LDZ.Coinbase.Test.Shared
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

        public async Task ReceiveFromChannelAsync<T>(ChannelReader<T> reader)
            where T : FeedResponseMessage
        {
            while (await reader.WaitToReadAsync())
            {
                ReceiveMessage(await reader.ReadAsync());
            }
        }
    }
}
