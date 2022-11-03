using Lola;
using Microsoft.Extensions.Logging;

namespace LolaTests
{
    internal static class CONSTANTS
    {
        public static decimal COIN_PRICE = 20000m;
    }

    internal class MockOrderPlacer : IOrderPlacer
    {
        public List<PlacedOrder> Orders = new();

        public async Task<PlacedOrder> BuyMarket(string symbol, decimal quantity = 0, decimal quoteQuantity = 0)
        {
            PlacedOrder order;
            if (quantity != 0)
            {
                order = new PlacedOrder()
                {
                    OrderId = 1,
                    Side = TradeSide.LONG,
                    Price = CONSTANTS.COIN_PRICE,
                    Quantity = quantity
                };
            }
            else
            {
                order = new PlacedOrder()
                {
                    OrderId = 1,
                    Side = TradeSide.LONG,
                    Price = CONSTANTS.COIN_PRICE,
                    Quantity = quoteQuantity / CONSTANTS.COIN_PRICE
                };
            }

            Orders.Add(order);
            return order;
        }

        public async Task<PlacedOrder> SellMarket(string symbol, decimal quantity)
        {
            PlacedOrder order;
            order = new PlacedOrder()
            {
                OrderId = 1,
                Side = TradeSide.LONG,
                Price = CONSTANTS.COIN_PRICE,
                Quantity = quantity
            };
            Orders.Add(order);
            return order;
        }

        public Task<PlacedOrder> BuyLimit(string symbol, decimal limitPrice, decimal quantity = 0, decimal quoteQuantity = 0)
        {
            throw new NotImplementedException();
        }

        public Task<PlacedOrder> SellLimit(string symbol, decimal quantity, decimal limitPrice)
        {
            throw new NotImplementedException();
        }
    }

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        async public Task TestTrader()
        {
            var orderPlacer = new MockOrderPlacer();
            var trader = new Trader(new Logger<Trader>(new LoggerFactory()), orderPlacer);
            trader.BTC = 1.0m;
            trader.USDT = 20000m;

            await trader.NewTradeSignal(TradeSignal.LONG);
            Assert.That(orderPlacer.Orders.First().Price, Is.EqualTo(CONSTANTS.COIN_PRICE));
            Assert.That(orderPlacer.Orders.First().Quantity, Is.EqualTo(1.0m));
        }

        [Test]
        async public Task TestBuyQuoteQuantity()
        {
            var orderPlacer = new MockOrderPlacer();
            var trader = new Trader(new Logger<Trader>(new LoggerFactory()), orderPlacer);
            trader.BTC = 0m;
            trader.USDT = 30000m;

            await trader.NewTradeSignal(TradeSignal.LONG);
            Assert.That(orderPlacer.Orders.First().Quantity, Is.EqualTo(1.5m));
        }

        [Test]
        async public Task TestSellQuoteQuantity()
        {
            var orderPlacer = new MockOrderPlacer();
            var trader = new Trader(new Logger<Trader>(new LoggerFactory()), orderPlacer);
            trader.BTC = 0m;
            trader.USDT = 30000m;

            await trader.NewTradeSignal(TradeSignal.SHORT);
            Assert.That(orderPlacer.Orders.First().Quantity, Is.EqualTo(0m));
        }

        [Test]
        async public Task TestExitTradeOnSignal()
        {
            var orderPlacer = new MockOrderPlacer();
            var trader = new Trader(new Logger<Trader>(new LoggerFactory()), orderPlacer);
            trader.BTC = 0m;
            trader.USDT = 30000m;
            
            await trader.NewTradeSignal(TradeSignal.SHORT);
            await trader.NewTradeSignal(TradeSignal.LONG);
            
            Assert.That(orderPlacer.Orders[1].Side, Is.EqualTo(TradeSide.LONG));
            Assert.That(orderPlacer.Orders[1].Quantity, Is.EqualTo(1.5m));
        }
    }
}