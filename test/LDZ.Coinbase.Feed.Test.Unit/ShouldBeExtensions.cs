using System;
using System.Text;

namespace LDZ.Coinbase.Feed.Test.Unit
{
    public static class ShouldBeExtensions
    {
        public static bool ContainsUtf8String(this ReadOnlyMemory<byte> actualBuffer, string expected)
        {
            var actual = Encoding.UTF8.GetString(actualBuffer.Span);
            return actual.Contains(expected);
        }
    }
}
