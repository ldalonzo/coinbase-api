using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LDZ.Coinbase.Api.Threading.Channels
{
    internal static class ChannelWriterExtensions
    {
        public static ImmutableList<Func<T, CancellationToken, ValueTask>> AddWriter<T, TMessage>(this ImmutableList<Func<T, CancellationToken, ValueTask>> source, ChannelWriter<TMessage> writer)
            where T : class
            where TMessage : T
            => source.Add((message, cancellationToken) => writer.WriteAsync((TMessage) message, cancellationToken));
    }
}
