using Lola;
using Lola.TradeWithLimitOrders;
using Lola.TradeWithLimitOrders.Stated;

public class PlaceExitShort : IState
{
    private LimitOrderPlacer _limitOrderPlacer;
    private PlacedOrder _exitOrder;

    public PlaceExitShort(LimitOrderPlacer limitOrderPlacer, PlacedOrder order)
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