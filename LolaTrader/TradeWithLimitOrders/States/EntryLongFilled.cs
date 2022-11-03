namespace Lola.TradeWithLimitOrders.Stated;

public class EntryLongFilled : IState
{
    private LimitOrderPlacer _limitOrderPlacer;

    public EntryLongFilled(LimitOrderPlacer limitOrderPlacer)
    {
        _limitOrderPlacer = limitOrderPlacer;
    }

    async public Task<IState> NewSignal(TradeSignal signal)
    {
        var order = await _limitOrderPlacer.PlaceOrderShort();
        return new PlaceExitLong(_limitOrderPlacer, order);
    }

    public IState NewMarketTrade(MarketTrade trade)
    {
        throw new NotImplementedException();
    }

    public IState Fill(OrderFillUpdate orderFillUpdate)
    {
        throw new NotImplementedException();
    }
}