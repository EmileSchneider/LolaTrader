namespace Lola.TradeWithLimitOrders.Stated;

public class NoTrader : IState
{
    private LimitOrderPlacer _limitOrderPlacer;

    private Dictionary<TradeSignal, Func<Task<IState>>> placeOrders;

    public NoTrader(LimitOrderPlacer limitOrderPlacer)
    {
        _limitOrderPlacer = limitOrderPlacer;

        placeOrders = new Dictionary<TradeSignal, Func<Task<IState>>>()
        {
            {
                TradeSignal.LONG, async () =>
                {
                    if (_limitOrderPlacer.USDT < 10m)
                        return this;
                    var order = await _limitOrderPlacer.PlaceOrderLong();
                    return new PlaceEntryLong(_limitOrderPlacer, order);
                }
            },
            {
                TradeSignal.SHORT, async () =>
                {
                    if (_limitOrderPlacer.BTC < 0.005m)
                        return this;
                    var order = await _limitOrderPlacer.PlaceOrderShort();
                    return new PlaceEntryShort(_limitOrderPlacer, order);
                }
            }
        };
    }

    public async Task<IState> NewSignal(TradeSignal signal)
    {
        return await placeOrders[signal]();
    }

    public IState NewMarketTrade(MarketTrade trade)
    {
        _limitOrderPlacer.LastTrade = trade;
        return this;
    }

    public IState Fill(OrderFillUpdate orderFillUpdate)
    {
        return this;
    }
}