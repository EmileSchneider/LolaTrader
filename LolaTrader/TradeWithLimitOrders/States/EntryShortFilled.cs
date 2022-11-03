namespace Lola.TradeWithLimitOrders.Stated;

public class EntryShortFilled : IState
{
    private LimitOrderPlacer _limitOrderPlacer;

    public EntryShortFilled(LimitOrderPlacer limitOrderPlacer)
    {
        _limitOrderPlacer = limitOrderPlacer;
    }

    public async Task<IState> NewSignal(TradeSignal signal)
    {
        var placedOrder = await _limitOrderPlacer.PlaceOrderShort();
        return new PlaceExitShort(_limitOrderPlacer, placedOrder);
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