using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LDZ.Coinbase.Api.DependencyInjection;
using LDZ.Coinbase.Api.Hosting;
using LDZ.Coinbase.Api.Model.Feed;
using LDZ.Coinbase.Api.Net.WebSockets;
using LDZ.Coinbase.Test.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace LDZ.Coinbase.Feed.Test.Unit
{
    public class WebSocketFeedUnitTest : IAsyncLifetime
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public WebSocketFeedUnitTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            TimeoutCancellationTokenSource = new CancellationTokenSource();
            TimeoutCancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(1_500));

            TimeoutTaskCompletionSource = new TaskCompletionSource();
            TimeoutCancellationTokenSource.Token.Register(() => TimeoutTaskCompletionSource.SetResult());

            Spy = new ReceivedMessageSpy(testOutputHelper);
        }

        private ServiceProvider? ServiceProvider { get; set; }
        private CancellationTokenSource TimeoutCancellationTokenSource { get; }
        private TaskCompletionSource TimeoutTaskCompletionSource { get; }
        private Task TimeoutTask => TimeoutTaskCompletionSource.Task;

        private ReceivedMessageSpy Spy { get; }

        [Fact]
        public async Task SubscribeToHeartbeatChannel()
        {
            ServiceProvider = ConfigureServices(builder => builder.SubscribeToHeartbeatChannel(Spy.ReceiveMessage, "ETH-EUR")).BuildServiceProvider();
            ServiceProvider
                .GetRequiredService<ClientWebSocketMock>()
                .SetupReceiveMessage("TestData/message_heartbeat.json");

            await StartAsync();
            await TimeoutTask;

            Spy.ReceivedMessages.ShouldHaveSingleItem().ShouldBeOfType<HeartbeatMessage>();
        }

        public Task InitializeAsync()
        {
            TimeoutCancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(600));

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token);
        }

        private async Task StartAsync()
        {
            if (ServiceProvider == null)
            {
                return;
            }

            var hostedServices = ServiceProvider.GetRequiredService<IEnumerable<IHostedService>>();
            foreach (var hostedService in hostedServices)
            {
                await hostedService.StartAsync(TimeoutCancellationTokenSource.Token);
            }

            _testOutputHelper.WriteLine("Started");
        }

        private async Task StopAsync(CancellationToken cancellationToken)
        {
            if (ServiceProvider == null)
            {
                return;
            }

            var hostedServices = ServiceProvider.GetRequiredService<IEnumerable<IHostedService>>();
            foreach (var hostedService in hostedServices)
            {
                await hostedService.StopAsync(cancellationToken);
            }

            _testOutputHelper.WriteLine("Stopped");
        }

        private static IServiceCollection ConfigureServices(Action<IWebSocketSubscriptionsBuilder> configure)
            => ConfigureServices(new ServiceCollection(), configure);

        private static IServiceCollection ConfigureServices(IServiceCollection services, Action<IWebSocketSubscriptionsBuilder> configure) => services
            .AddLogging()
            .AddSingleton<ClientWebSocketMock>()
            .AddTransient<IClientWebSocketFacade>(sp => sp.GetRequiredService<ClientWebSocketMock>())
            .AddCoinbaseProApi(apiBuilder => apiBuilder.ConfigureFeed(configure));
    }
}
