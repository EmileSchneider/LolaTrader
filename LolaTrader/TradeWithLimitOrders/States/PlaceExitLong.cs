namespace Lola.TradeWithLimitOrders.Stated;

public class PlaceExitLong : IState
{
    private readonly LimitOrderPlacer _limitOrderPlacer;
    private readonly PlacedOrder _exitOrder;

    public PlaceExitLong(LimitOrderPlacer limitOrderPlacer, PlacedOrder order)
    {
        _limitOrderPlacer = limitOrderPlacer;
        _exitOrder = order;
    }

    public Task<IState> NewSignal(TradeSignal signal)
    {
        throw new NotImplementedException();
    }

    public IState NewMarketTrade(MarketTrade trade)
    {
        throw new NotImplementedException();
    }

    public IState Fill(OrderFillUpdate orderFillUpdate)
    {
        if (orderFillUpdate.OrderId != _exitOrder.OrderId)
            throw new ArgumentException();

        if (orderFillUpdate.TotalFill == _exitOrder.Quantity)
            return new NoTrader(_limitOrderPlacer);
        
        return this;
    }
}