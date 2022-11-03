using CryptoExchange.Net.CommonObjects;
using Lola.TradeWithLimitOrders.Stated;

namespace Lola.TradeWithLimitOrders;

public class LimitOrderTrader
{
    private LimitOrderPlacer _limitOrderPlacer;
    private IState _state;

    public LimitOrderTrader(IOrderPlacer orderPlacer, decimal BTC, decimal USDT)
    {
        _limitOrderPlacer = new LimitOrderPlacer(orderPlacer, usdt: USDT, btc: BTC);
        _state = new NoTrader(_limitOrderPlacer);
    }

    public void NewClose(decimal close)
    {
        _limitOrderPlacer.LastClose = close;
    }

    async public Task NewSignal(TradeSignal signal)
    {
        _state = await _state.NewSignal(signal);
        Console.WriteLine($"{signal} {_state} {_limitOrderPlacer.BTC} {_limitOrderPlacer.USDT}");
    }

    public void NewOrderUpdate(OrderFillUpdate orderUpdate)
    {
        _state = _state.Fill(orderUpdate);
        Console.WriteLine($"{orderUpdate} {_state} {_limitOrderPlacer.BTC} {_limitOrderPlacer.USDT}");
    }

    public void NewMarketTrade(MarketTrade marketTrade)
    {
        //_state = _state.NewMarketTrade(marketTrade);
    }
}