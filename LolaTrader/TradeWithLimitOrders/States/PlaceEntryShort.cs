namespace Lola.TradeWithLimitOrders.Stated;

public class PlaceEntryShort : IState
{
    private LimitOrderPlacer _limitOrderPlacer;
    private PlacedOrder _order;

    public PlaceEntryShort(LimitOrderPlacer limitOrderPlacer, PlacedOrder order)
    {
        _limitOrderPlacer = limitOrderPlacer;
        _order = order;
    }

    public Task<IState> NewSignal(TradeSignal signal)
    {
        throw new NotImplementedException();
    }

    public IState NewMarketTrade(MarketTrade trade)
    {
        return this;
    }

    public IState Fill(OrderFillUpdate orderFillUpdate)
    {
        if (orderFillUpdate.OrderId != _order.OrderId)
            throw new ArgumentException();
        if (orderFillUpdate.TotalFill == _order.Quantity)
            return new EntryShortFilled(_limitOrderPlacer);

        return this;
    }
}