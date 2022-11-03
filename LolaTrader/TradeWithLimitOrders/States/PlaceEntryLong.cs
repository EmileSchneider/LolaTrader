namespace Lola.TradeWithLimitOrders.Stated;

public class PlaceEntryLong : IState
{
    private LimitOrderPlacer _limitOrderPlacer;
    private PlacedOrder _order;

    public PlaceEntryLong(LimitOrderPlacer limitOrderPlacer, PlacedOrder order)
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
            throw new ArgumentException(
                $"Fill Order Id {orderFillUpdate.OrderId} does not matched Placed Order Id {_order.OrderId}");

        if (orderFillUpdate.TotalFill == _order.Quantity)
            return new EntryLongFilled(_limitOrderPlacer);
        
        return this;
    }
}