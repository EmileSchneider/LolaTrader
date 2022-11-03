using Lola;
using Lola.TradeWithLimitOrders;
using Lola.TradeWithLimitOrders.Stated;
using NUnit.Framework;

namespace LolaTests;

class OrderPlacer : IOrderPlacer
{
    public Stack<PlacedOrder> Orders { get; }

    private int counter;

    public OrderPlacer()
    {
        Orders = new();
        counter = 1;
    }

    public Task<PlacedOrder> BuyMarket(string symbol, decimal quantity = 0, decimal quoteQuantity = 0)
    {
        throw new NotImplementedException();
    }

    public Task<PlacedOrder> SellMarket(string symbol, decimal quantity)
    {
        throw new NotImplementedException();
    }

    async public Task<PlacedOrder> BuyLimit(string symbol, decimal limitPrice, decimal quantity = 0,
        decimal quoteQuantity = 0)
    {
        var o = new PlacedOrder()
        {
            OrderId = counter,
            Price = limitPrice,
            Quantity = quantity,
            Side = TradeSide.LONG
        };
        Orders.Push(o);
        return o;
    }

    async public Task<PlacedOrder> SellLimit(string symbol, decimal quantity, decimal limitPrice)
    {
        var o = new PlacedOrder()
        {
            OrderId = counter,
            Price = limitPrice,
            Quantity = quantity,
            Side = TradeSide.SHORT
        };
        Orders.Push(o);
        return o;
    }
}

[TestFixture]
public class StatefulLimitOrderTraderTests
{
    [Test]
    public async Task Test()
    {
        var lto = new LimitOrderPlacer(new OrderPlacer(), 2000m, 1.0m);
        IState state = new NoTrader(lto);
        state = await state.NewSignal(TradeSignal.LONG);
        Assert.That(state.GetType(), Is.EqualTo(typeof(PlaceEntryLong)));
    }

    [Test]
    public async Task Test1()
    {
        var lto = new LimitOrderPlacer(new OrderPlacer(), 0m, 1.0m);
        IState state = new NoTrader(lto);
        state = await state.NewSignal(TradeSignal.SHORT);
        Assert.That(state.GetType(), Is.EqualTo(typeof(PlaceEntryShort)));
    }

    [Test]
    public async Task Test2()
    {
        var op = new OrderPlacer();
        var lto = new LimitOrderPlacer(op, 0m, 1.0m);
        IState state = new NoTrader(lto);
        state = await state.NewSignal(TradeSignal.SHORT);
        state = state.Fill(new OrderFillUpdate()
        {
            OrderId = op.Orders.Peek().OrderId,
            TotalFill = 0.5m,
        });
        Assert.That(state.GetType(), Is.EqualTo(typeof(PlaceEntryShort)));
        state = state.Fill(new OrderFillUpdate()
        {
            OrderId = op.Orders.Peek().OrderId,
            TotalFill = 1.0m,
        });
        Assert.That(state.GetType(), Is.EqualTo(typeof(EntryShortFilled)));
    }

    [Test]
    public async Task Test_FillLongOrder()
    {
        var op = new OrderPlacer();
        var lto = new LimitOrderPlacer(op, 0m, 1.0m);
        IState state = new PlaceEntryLong(lto, await op.BuyLimit("BTCUSDT", 20700, 1.0m));
        state = state.Fill(new OrderFillUpdate()
        {
            OrderId = 1,
            TotalFill = 1.0m
        });
        Assert.That(state.GetType(), Is.EqualTo(typeof(EntryLongFilled)));
    }

    [Test]
    public async Task Test_ExitLongTrade()
    {
        var op = new OrderPlacer();
        var lto = new LimitOrderPlacer(op, 0m, 1.0m);
        IState state = new EntryLongFilled(lto);
        state = await state.NewSignal(TradeSignal.LONG);
        Assert.That(state.GetType(), Is.EqualTo(typeof(PlaceExitLong)));
    }

    [Test]
    public async Task Test_ExitLongFilled()
    {
        var op = new OrderPlacer();
        var lto = new LimitOrderPlacer(op, 0m, 1.0m);
        IState state = new PlaceExitLong(lto, await op.SellLimit("BTCUSDT", 1.0m, 20705m));
        state = await state.NewSignal(TradeSignal.LONG);
        Assert.That(state.GetType(), Is.EqualTo(typeof(PlaceExitLong)));
    }
}