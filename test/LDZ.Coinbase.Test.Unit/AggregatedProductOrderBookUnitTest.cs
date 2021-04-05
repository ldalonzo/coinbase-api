using System.Collections.Generic;
using LDZ.Coinbase.Api.Model.MarketData;
using Shouldly;
using Xunit;

namespace LDZ.Coinbase.Test.Unit
{
    public class AggregatedProductOrderBookUnitTest
    {
        [Fact]
        public void GetBestBid()
        {
            var orderBook = new AggregatedProductOrderBook
            {
                Bids = new List<AggregatedProductOrder>
                {
                    new AggregatedProductOrder {Price = 55_938.76m},
                    new AggregatedProductOrder {Price = 55_938.77m},
                    new AggregatedProductOrder {Price = 55_938.75m}
                }
            };

            orderBook.GetBestBid().ShouldBe(55_938.77m);
        }

        [Fact]
        public void GetWorstBid()
        {
            var orderBook = new AggregatedProductOrderBook
            {
                Bids = new List<AggregatedProductOrder>
                {
                    new AggregatedProductOrder {Price = 55_938.76m},
                    new AggregatedProductOrder {Price = 55_938.77m},
                    new AggregatedProductOrder {Price = 55_938.75m}
                }
            };

            orderBook.GetWorstBid().ShouldBe(55_938.75m);
        }
    }
}
