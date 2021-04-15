using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LDZ.Coinbase.Api.Threading.Channels
{
    public class ChannelDemux<T>
        where T : class
    {
        public ChannelDemux(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger(GetType());
        }

        private readonly ILogger _log;

        private readonly ConcurrentDictionary<Type, ImmutableList<Func<T, CancellationToken, ValueTask>>> _writersByMessageType = new ConcurrentDictionary<Type, ImmutableList<Func<T, CancellationToken, ValueTask>>>();
        private readonly ConcurrentBag<Action<Exception?>> _completeActions = new ConcurrentBag<Action<Exception?>>();

        public ChannelReader<TMessage> AddSink<TMessage>()
            where TMessage : T
        {
            var channel = Channel.CreateUnbounded<TMessage>();
            AddWriter(channel.Writer);

            return channel.Reader;
        }

        private void AddWriter<TMessage>(ChannelWriter<TMessage> writer)
            where TMessage : T
        {
            _writersByMessageType.AddOrUpdate(typeof(TMessage), _ => ImmutableList<Func<T, CancellationToken, ValueTask>>.Empty.AddWriter(writer), (_, writers) => writers.AddWriter(writer));
            _completeActions.Add(writer.Complete);
        }

        public async Task WriteAsync(ChannelReader<T> reader, CancellationToken cancellationToken)
        {
            try
            {
                await WriteAsyncCore(reader, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                foreach (var complete in _completeActions)
                {
                    complete(null);
                }
            }
        }

        private async Task WriteAsyncCore(ChannelReader<T> reader, CancellationToken cancellationToken)
        {
            while (await reader.WaitToReadAsync(cancellationToken))
            {
                T message = await reader.ReadAsync(cancellationToken);

                await Dispatch(message, cancellationToken);
            }
        }

        private async Task Dispatch(T message, CancellationToken cancellationToken)
        {
            var writers = FindWriters(message.GetType()).ToList();
            if (writers.Any())
            {
                foreach (var writeAsync in writers)
                {
                    await writeAsync(message, cancellationToken);
                }
            }
            else
            {
                _log.LogTrace($"DROP {message}");
            }
        }

        private IEnumerable<Func<T, CancellationToken, ValueTask>> FindWriters(Type messageType)
        {
            foreach (var (targetType, writers) in _writersByMessageType)
            {
                if (messageType.IsAssignableTo(targetType))
                {
                    foreach (var writeAsync in writers)
                    {
                        yield return writeAsync;
                    }
                }
            }
        }
    }
}
